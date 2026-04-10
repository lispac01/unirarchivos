require 'roo'
require 'roo-xls' # Requerido para leer archivos .xls antiguos
require 'write_xlsx'

# Ruta local desde la que se toman los archivos a consolidar.
carpeta = 'C:/cps' # Cambia esta ruta

# Busca archivos Excel y evita posibles archivos consolidados para no duplicar datos
archivos = ['*.xls', '*.xlsx']
           .flat_map { |patron| Dir.glob(File.join(carpeta, patron)) }
           .reject { |archivo| File.basename(archivo).downcase.include?('consolidado') }
           .sort

filas_totales = []
encabezado = nil
encabezado_base_normalizado = nil
encabezado_base_indices = nil
archivos_procesados = []
archivos_con_error = []
archivos_omitidos_por_encabezado = []
detalles_columnas = []

if archivos.empty?
  warn "No se encontraron archivos .xls o .xlsx en #{carpeta}."
  exit 1
end

def normalizar_texto(valor)
  texto = valor.to_s.strip.downcase
  texto = begin
    texto.unicode_normalize(:nfkd).encode('ASCII', replace: '', undef: :replace, invalid: :replace)
  rescue StandardError
    texto
  end
  texto.gsub(/[^a-z0-9]+/, '')
end

def normalizar_fila(fila)
  # Convierte a texto comparable para detectar encabezados duplicados
  valores = fila.map do |celda|
    normalizar_texto(celda)
  end

  # Quita vacíos al final para evitar falsos negativos por columnas sobrantes
  valores.pop while valores.any? && valores.last.empty?
  valores
end

def fila_vacia?(fila)
  fila.all? { |celda| celda.nil? || celda.to_s.strip.empty? }
end

def celdas_con_letras(valores)
  valores.count { |valor| valor.match?(/[a-z]/) }
end

def encontrar_fila_encabezado(hoja, encabezado_base_normalizado = nil)
  limite = [hoja.last_row.to_i, 20].min
  return nil if limite.zero?

  mejor_candidato = nil
  columnas_base = encabezado_base_normalizado&.reject(&:empty?)&.uniq || []

  (1..limite).each do |indice|
    fila = hoja.row(indice)
    fila_normalizada = normalizar_fila(fila)
    next if fila_normalizada.empty?

    columnas_presentes = fila_normalizada.reject(&:empty?)
    next if columnas_presentes.length < 2

    columnas_texto = celdas_con_letras(columnas_presentes)
    next if columnas_texto < 3

    if encabezado_base_normalizado
      if fila_normalizada == encabezado_base_normalizado
        return { fila_numero: indice, fila: fila,
                 fila_normalizada: fila_normalizada }
      end

      coincidencias = (columnas_presentes.uniq & columnas_base).length
      next if coincidencias.zero?

      candidato = {
        fila_numero: indice,
        fila: fila,
        fila_normalizada: fila_normalizada,
        puntaje: coincidencias,
        columnas_texto: columnas_texto,
        columnas: columnas_presentes.length
      }
    else
      candidato = {
        fila_numero: indice,
        fila: fila,
        fila_normalizada: fila_normalizada,
        puntaje: columnas_texto,
        columnas_texto: columnas_texto,
        columnas: columnas_presentes.length
      }
    end

    next unless mejor_candidato.nil? ||
                candidato[:puntaje] > mejor_candidato[:puntaje] ||
                (candidato[:puntaje] == mejor_candidato[:puntaje] && candidato[:columnas_texto] > mejor_candidato[:columnas_texto]) ||
                (candidato[:puntaje] == mejor_candidato[:puntaje] && candidato[:columnas_texto] == mejor_candidato[:columnas_texto] && candidato[:columnas] > mejor_candidato[:columnas])

    mejor_candidato = candidato
  end

  return mejor_candidato if encabezado_base_normalizado.nil?
  return nil if mejor_candidato.nil?

  minimo_coincidencias = [3, (columnas_base.length * 0.5).ceil].max
  return nil if mejor_candidato[:puntaje] < minimo_coincidencias

  mejor_candidato
end

def construir_mapa_encabezado(fila_normalizada)
  indices = {}
  duplicadas = []

  fila_normalizada.each_with_index do |columna, indice|
    next if columna.nil? || columna.empty?

    if indices.key?(columna)
      duplicadas << columna
    else
      indices[columna] = indice
    end
  end

  { indices: indices, duplicadas: duplicadas.uniq }
end

def construir_mapeo_columnas(encabezado_base_normalizado, encabezado_base_indices, encabezado_actual_normalizado)
  mapa_actual = construir_mapa_encabezado(encabezado_actual_normalizado)
  duplicadas = mapa_actual[:duplicadas]
  return { duplicadas: duplicadas, indices_por_base: [], faltantes: [], extras: [] } unless duplicadas.empty?

  indices_actuales = mapa_actual[:indices]
  indices_por_base = Array.new(encabezado_base_normalizado.length)
  faltantes = []

  encabezado_base_normalizado.each_with_index do |columna, indice_base|
    next if columna.nil? || columna.empty?

    indice_actual = indices_actuales[columna]
    if indice_actual.nil?
      faltantes << columna
    else
      indices_por_base[indice_base] = indice_actual
    end
  end

  extras = indices_actuales.keys.reject { |columna| encabezado_base_indices.key?(columna) }

  {
    duplicadas: [],
    indices_por_base: indices_por_base,
    faltantes: faltantes,
    extras: extras
  }
end

def es_encabezado_repetido?(fila, encabezado_normalizado)
  normalizar_fila(fila) == encabezado_normalizado
end

def reordenar_fila(fila, indices_por_base)
  indices_por_base.map do |indice_actual|
    indice_actual.nil? ? nil : fila[indice_actual]
  end
end

archivos.each do |archivo|
  xlsx = Roo::Spreadsheet.open(archivo)
  hoja = xlsx.sheet(0)

  if hoja.last_row.nil? || hoja.last_row.zero?
    archivos_omitidos_por_encabezado << {
      archivo: archivo,
      motivo: 'sin filas para leer'
    }
    next
  end

  encabezado_detectado = encontrar_fila_encabezado(hoja, encabezado_base_normalizado)

  if encabezado_detectado.nil?
    archivos_omitidos_por_encabezado << {
      archivo: archivo,
      motivo: 'no se pudo detectar el encabezado'
    }
    next
  end

  fila_encabezado = encabezado_detectado[:fila]
  encabezado_actual_normalizado = encabezado_detectado[:fila_normalizada]
  fila_encabezado_numero = encabezado_detectado[:fila_numero]

  if encabezado_actual_normalizado.empty?
    archivos_omitidos_por_encabezado << {
      archivo: archivo,
      motivo: 'encabezado vacio'
    }
    next
  end

  if encabezado.nil?
    mapa_base = construir_mapa_encabezado(encabezado_actual_normalizado)

    unless mapa_base[:duplicadas].empty?
      archivos_omitidos_por_encabezado << {
        archivo: archivo,
        motivo: 'encabezado con columnas duplicadas',
        columnas: mapa_base[:duplicadas]
      }
      next
    end

    encabezado = fila_encabezado.take(encabezado_actual_normalizado.length)
    encabezado_base_normalizado = encabezado_actual_normalizado
    encabezado_base_indices = mapa_base[:indices]
  end

  mapeo = construir_mapeo_columnas(
    encabezado_base_normalizado,
    encabezado_base_indices,
    encabezado_actual_normalizado
  )

  unless mapeo[:duplicadas].empty?
    archivos_omitidos_por_encabezado << {
      archivo: archivo,
      motivo: 'encabezado con columnas duplicadas',
      columnas: mapeo[:duplicadas]
    }
    next
  end

  detalles = []
  detalles << "faltantes: #{mapeo[:faltantes].join(', ')}" unless mapeo[:faltantes].empty?
  detalles << "extras ignoradas: #{mapeo[:extras].join(', ')}" unless mapeo[:extras].empty?
  detalles_columnas << { archivo: archivo, detalles: detalles } unless detalles.empty?

  filas_agregadas = 0

  # Leer todas las filas desde la fila siguiente al encabezado hasta el final
  ((fila_encabezado_numero + 1)..hoja.last_row).each do |i|
    fila = hoja.row(i)

    # Salta filas vacías
    next if fila_vacia?(fila)

    # Salta esta fila si repite el encabezado del archivo actual
    next if es_encabezado_repetido?(fila, encabezado_actual_normalizado)

    # Añadir fila reordenada al consolidado usando el esquema base
    filas_totales << reordenar_fila(fila, mapeo[:indices_por_base])
    filas_agregadas += 1
  end

  archivos_procesados << {
    archivo: archivo,
    filas_agregadas: filas_agregadas
  }
rescue StandardError => e
  archivos_con_error << {
    archivo: archivo,
    error: e.message
  }
end

if encabezado.nil?
  warn 'No se pudo obtener un encabezado base valido desde los archivos de entrada.'
  exit 1
end

# Crear archivo consolidado
output_file = File.join(carpeta, 'consolidado.xlsx')
workbook = WriteXLSX.new(output_file)
worksheet = workbook.add_worksheet

# Escribir encabezado
worksheet.write_row(0, 0, encabezado) if encabezado

# Escribir filas
filas_totales.each_with_index do |fila, i|
  worksheet.write_row(i + 1, 0, fila)
end

workbook.close

puts "Consolidacion terminada en: #{output_file}"
puts "Archivos procesados: #{archivos_procesados.length}"
puts "Archivos omitidos por encabezado: #{archivos_omitidos_por_encabezado.length}"
puts "Archivos con error: #{archivos_con_error.length}"
puts "Filas consolidadas: #{filas_totales.length}"

unless detalles_columnas.empty?
  puts 'Detalles de columnas detectados:'
  detalles_columnas.each do |detalle|
    puts "- #{File.basename(detalle[:archivo])}: #{detalle[:detalles].join(' | ')}"
  end
end

unless archivos_omitidos_por_encabezado.empty?
  puts 'Archivos omitidos por encabezado:'
  archivos_omitidos_por_encabezado.each do |detalle|
    descripcion = detalle[:motivo]
    descripcion += " (#{detalle[:columnas].join(', ')})" if detalle[:columnas]
    puts "- #{File.basename(detalle[:archivo])}: #{descripcion}"
  end
end

unless archivos_con_error.empty?
  puts 'Archivos con error:'
  archivos_con_error.each do |detalle|
    puts "- #{File.basename(detalle[:archivo])}: #{detalle[:error]}"
  end
end
