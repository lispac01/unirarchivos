const fs = require('fs');
const path = require('path');
const {
    Document,
    Packer,
    Paragraph,
    TextRun,
    Table,
    TableRow,
    TableCell,
    HeadingLevel,
    AlignmentType,
    WidthType,
    ShadingType,
    PageBreak
} = require('docx');

const COLOR_PRIMARY = "1E3A8A";
const COLOR_SECONDARY = "2563EB";
const COLOR_BG_LIGHT = "F3F4F6";

function createTitle(text) {
    return new Paragraph({
        alignment: AlignmentType.CENTER,
        spacing: { before: 300, after: 150 },
        children: [new TextRun({ text, bold: true, size: 40, color: COLOR_PRIMARY, font: "Calibri" })]
    });
}

function createSubtitle(text) {
    return new Paragraph({
        alignment: AlignmentType.CENTER,
        spacing: { before: 50, after: 300 },
        children: [new TextRun({ text, italic: true, size: 24, color: "4B5563", font: "Calibri" })]
    });
}

function createMetaLine(label, value) {
    return new Paragraph({
        spacing: { before: 40, after: 40 },
        children: [
            new TextRun({ text: label + ": ", bold: true, size: 20, font: "Calibri" }),
            new TextRun({ text: value, size: 20, font: "Calibri" })
        ]
    });
}

function createHeading1(text) {
    return new Paragraph({
        heading: HeadingLevel.HEADING_1,
        spacing: { before: 320, after: 120 },
        children: [new TextRun({ text, bold: true, size: 30, color: COLOR_PRIMARY, font: "Calibri" })]
    });
}

function createHeading2(text) {
    return new Paragraph({
        heading: HeadingLevel.HEADING_2,
        spacing: { before: 220, after: 100 },
        children: [new TextRun({ text, bold: true, size: 24, color: COLOR_SECONDARY, font: "Calibri" })]
    });
}

function createHeading3(text) {
    return new Paragraph({
        spacing: { before: 160, after: 80 },
        children: [new TextRun({ text, bold: true, size: 22, color: "1F2937", font: "Calibri" })]
    });
}

function createParagraph(text, boldPrefix = "") {
    const children = [];
    if (boldPrefix) {
        children.push(new TextRun({ text: boldPrefix + " ", bold: true, size: 22, font: "Calibri", color: "1F2937" }));
    }
    children.push(new TextRun({ text, size: 22, font: "Calibri", color: "374151" }));
    return new Paragraph({ spacing: { before: 50, after: 50 }, lineSpacing: { line: 276 }, children });
}

function createBullet(text, boldPrefix = "") {
    const children = [];
    if (boldPrefix) {
        children.push(new TextRun({ text: boldPrefix + " ", bold: true, size: 22, font: "Calibri" }));
    }
    children.push(new TextRun({ text, size: 22, font: "Calibri" }));
    return new Paragraph({ bullet: { level: 0 }, spacing: { before: 30, after: 30 }, children });
}

function createReqBox(code, name, description, actor, preconditions, inputs, outputs, rules) {
    const cellStyleHeader = {
        fill: { type: ShadingType.CLEAR, color: "000000", fill: COLOR_PRIMARY },
        margins: { top: 80, bottom: 80, left: 120, right: 120 }
    };
    const cellStyleLabel = {
        fill: { type: ShadingType.CLEAR, color: "000000", fill: COLOR_BG_LIGHT },
        margins: { top: 60, bottom: 60, left: 100, right: 100 }
    };

    const rows = [
        ["Actor Principal", actor],
        ["Descripción", description],
        ["Precondiciones", preconditions],
        ["Datos de Entrada", inputs],
        ["Salida / Resultado", outputs]
    ].map(([label, value]) => new TableRow({
        children: [
            new TableCell({
                shading: cellStyleLabel.fill,
                width: { size: 28, type: WidthType.PERCENTAGE },
                children: [new Paragraph({ children: [new TextRun({ text: label + ":", bold: true, size: 19, font: "Calibri" })] })]
            }),
            new TableCell({
                width: { size: 72, type: WidthType.PERCENTAGE },
                children: [new Paragraph({ children: [new TextRun({ text: value, size: 19, font: "Calibri" })] })]
            })
        ]
    }));

    rows.push(new TableRow({
        children: [
            new TableCell({
                shading: cellStyleLabel.fill,
                width: { size: 28, type: WidthType.PERCENTAGE },
                children: [new Paragraph({ children: [new TextRun({ text: "Reglas de Negocio:", bold: true, size: 19, font: "Calibri" })] })]
            }),
            new TableCell({
                width: { size: 72, type: WidthType.PERCENTAGE },
                children: rules.map(r => new Paragraph({ bullet: { level: 0 }, children: [new TextRun({ text: r, size: 19, font: "Calibri" })] }))
            })
        ]
    }));

    return new Table({
        width: { size: 100, type: WidthType.PERCENTAGE },
        spacing: { before: 120, after: 200 },
        rows: [
            new TableRow({
                children: [
                    new TableCell({
                        shading: cellStyleHeader.fill,
                        columnSpan: 2,
                        children: [new Paragraph({ children: [new TextRun({ text: `${code}: ${name}`, bold: true, color: "FFFFFF", size: 21, font: "Calibri" })] })]
                    })
                ]
            }),
            ...rows
        ]
    });
}

function createDataTable(headers, rowsData) {
    const headerRow = new TableRow({
        children: headers.map(h => new TableCell({
            shading: { type: ShadingType.CLEAR, color: "000000", fill: COLOR_PRIMARY },
            margins: { top: 80, bottom: 80, left: 80, right: 80 },
            children: [new Paragraph({ children: [new TextRun({ text: h, bold: true, color: "FFFFFF", size: 18, font: "Calibri" })] })]
        }))
    });

    const bodyRows = rowsData.map((row, idx) => {
        const bg = (idx % 2 === 0) ? "FFFFFF" : COLOR_BG_LIGHT;
        return new TableRow({
            children: row.map(cell => new TableCell({
                shading: { type: ShadingType.CLEAR, color: "000000", fill: bg },
                margins: { top: 50, bottom: 50, left: 60, right: 60 },
                children: [new Paragraph({ children: [new TextRun({ text: cell, size: 17, font: "Calibri" })] })]
            }))
        });
    });

    return new Table({
        width: { size: 100, type: WidthType.PERCENTAGE },
        spacing: { before: 120, after: 200 },
        rows: [headerRow, ...bodyRows]
    });
}

async function generateReport() {
    const children = [
        createTitle("DOCUMENTO DE REQUERIMIENTOS DE SOFTWARE"),
        createSubtitle("Sistema de Mantenimiento e Inspección Previaje (PTI) de Contenedores"),
        createMetaLine("Proyecto origen analizado", "MantenimientoDeContenedores"),
        createMetaLine("Versión del documento", "1.0"),
        createMetaLine("Fecha de elaboración", "28 de agosto de 2026"),
        createMetaLine("Propósito", "Especificar funcionalidades del módulo actual para el desarrollo de una nueva aplicación"),

        createHeading1("1. INTRODUCCIÓN"),
        createHeading2("1.1 Propósito del documento"),
        createParagraph("Este documento describe, en formato de requerimientos de software, todas las funcionalidades identificadas en el proyecto MantenimientoDeContenedores. Su objetivo es servir como base funcional para diseñar e implementar una nueva aplicación que replique y extienda el comportamiento del sistema actual."),
        createHeading2("1.2 Alcance del sistema"),
        createParagraph("El sistema gestiona el ciclo operativo de mantenimiento de contenedores en un patio/taller logístico, desde la configuración de catálogos maestros hasta el registro de ingresos al patio y las inspecciones previaje (PTI — Pre-Trip Inspection) por especialidad técnica."),
        createHeading2("1.3 Contexto del proyecto analizado"),
        createBullet("Aplicación web ASP.NET Core MVC 8.0 con Entity Framework Core 8.0.8.", "Tecnología:"),
        createBullet("SQL Server, base de datos 'mantenimiento', con creación automática de esquema al iniciar.", "Persistencia:"),
        createBullet("Patrón Repositorio con inyección de dependencias.", "Arquitectura:"),
        createBullet("Bootstrap 5, jQuery Validation, vistas Razor.", "Interfaz:"),
        createBullet("No implementa autenticación, autorización, reportes ni eliminación de registros.", "Limitaciones actuales:"),

        createHeading1("2. DESCRIPCIÓN GENERAL DEL NEGOCIO"),
        createHeading2("2.1 Problema que resuelve"),
        createParagraph("Las empresas de logística y talleres de contenedores necesitan registrar qué contenedores ingresan al patio, quién los inspecciona, en qué rubro técnico (especialidad) y si el contenedor queda habilitado o no para operación tras la inspección previaje."),
        createHeading2("2.2 Flujo operativo principal"),
        createParagraph("El flujo de negocio del sistema se resume en cuatro etapas secuenciales:"),
        createBullet("Configuración de maestros: clientes, contenedores, especialidades, técnicos y tareas."),
        createBullet("Ingreso a patio: registro de entrada del contenedor con número consecutivo y fecha/hora."),
        createBullet("Inspección previaje (PTI): evaluación técnica por especialidad, asignando un técnico calificado."),
        createBullet("Resultado: cada ingreso puede tener múltiples previajes (uno por especialidad), cada uno con estado Habilitado (aprobado) o No habilitado (rechazado)."),
        createHeading2("2.3 Diagrama de flujo de negocio"),
        createParagraph("Maestros (Clientes → Contenedores → Especialidades → Técnicos → Tareas) → Ingreso a patio → Previaje PTI por especialidad pendiente → Resultado Habilitado/No habilitado."),

        createHeading1("3. ACTORES DEL SISTEMA"),
        createDataTable(
            ["Actor", "Rol", "Responsabilidades"],
            [
                ["Administrador", "Configuración", "Gestiona clientes, especialidades, contenedores y parámetros del sistema."],
                ["Operador de patio / Gate", "Operaciones de entrada", "Registra ingresos de contenedores al patio."],
                ["Inspector PTI / Técnico", "Inspección", "Ejecuta y registra inspecciones previaje por especialidad."],
                ["Jefe de taller", "Supervisión técnica", "Administra técnicos, sus competencias y catálogo de tareas."],
                ["Jefe de operaciones", "Planificación", "Define tareas estandarizadas por especialidad con tiempos estimados."]
            ]
        ),
        createParagraph("Nota: En el sistema actual estos roles no están implementados como permisos; cualquier usuario con acceso a la URL puede operar todos los módulos. La nueva aplicación debe considerar su implementación.", ""),

        createHeading1("4. ESTRUCTURA DE MÓDULOS"),
        createDataTable(
            ["Grupo", "Módulo", "Tipo", "Descripción breve"],
            [
                ["Maestros", "Clientes de mantenimiento", "Catálogo", "Clientes del taller con tarifas de mano de obra"],
                ["Maestros", "Contenedores", "Catálogo", "Unidades físicas vinculadas a un cliente"],
                ["Maestros", "Especialidades de mantenimiento", "Catálogo", "Rubros/tipos de inspección técnica"],
                ["Maestros", "Especialidades del técnico", "Catálogo", "Personal técnico y sus competencias"],
                ["Maestros", "Tareas de mantenimiento", "Catálogo", "Tareas estandarizadas por especialidad"],
                ["Transacciones", "Ingreso de contenedor", "Operación", "Entrada del contenedor al patio"],
                ["Transacciones", "Previaje de contenedor", "Operación", "Inspección PTI por especialidad"],
                ["General", "Inicio / Dashboard", "Navegación", "Punto de acceso con menú y accesos rápidos"]
            ]
        ),

        createHeading1("5. REQUERIMIENTOS FUNCIONALES"),

        createHeading2("5.1 Módulo: Inicio / Dashboard (RF-INICIO)"),
        createReqBox("RF-INICIO-001", "Pantalla principal con menú de navegación", "El sistema debe presentar una pantalla de inicio con menú lateral tipo explorador que organice los módulos en dos grupos: Maestros y Transacciones.", "Todos los actores", "Usuario accede a la aplicación.", "Ninguno.", "Menú navegable con acceso a todos los módulos del sistema.", ["Mostrar grupo Maestros: Clientes, Especialidades, Especialidades técnico, Tareas mtto, Contenedores.", "Mostrar grupo Transacciones: Ingreso contenedor, Previaje.", "Incluir accesos rápidos: Nuevo ingreso, Ver ingresos, Nuevo previaje, Nueva tarea, Ver contenedores."]),
        createReqBox("RF-INICIO-002", "Panel informativo de transacciones", "La pantalla principal debe mostrar información contextual sobre el módulo de ingreso de contenedores y tareas de mantenimiento.", "Operador de patio", "Usuario en pantalla de inicio.", "Ninguno.", "Panel descriptivo con botones de acción directa.", ["Describir el propósito del ingreso de contenedores.", "Describir el propósito del catálogo de tareas de mantenimiento."]),

        createHeading2("5.2 Módulo: Clientes de Mantenimiento (RF-CLI)"),
        createReqBox("RF-CLI-001", "Consulta de clientes", "El sistema debe listar todos los clientes de mantenimiento registrados con sus datos principales y totales de activos/inactivos.", "Administrador", "Existen clientes registrados o la lista está vacía.", "Filtros opcionales (no implementados en sistema actual).", "Tabla con: código, nombre, centro de costo, tarifa MO sin garantía, tarifa MO con garantía, estado activo/inactivo.", ["Mostrar conteo de clientes activos e inactivos.", "Permitir navegar a creación y edición desde la lista."]),
        createReqBox("RF-CLI-002", "Alta de cliente", "El sistema debe permitir registrar un nuevo cliente de mantenimiento con sus tarifas de mano de obra.", "Administrador", "No existe otro cliente con el mismo código.", "Código de cliente (máx. 20 caracteres), nombre (máx. 150), centro de costo (máx. 20, opcional), tarifa MO sin garantía, tarifa MO con garantía, estado activo.", "Cliente creado y visible en listado.", ["Código de cliente obligatorio y único.", "Nombre obligatorio.", "Tarifas de mano de obra >= 0.", "Estado activo por defecto al crear.", "Mostrar mensaje de confirmación al guardar."]),
        createReqBox("RF-CLI-003", "Edición de cliente", "El sistema debe permitir modificar los datos de un cliente existente, excepto su código.", "Administrador", "Cliente existe en el sistema.", "Código (solo lectura), nombre, centro de costo, tarifas MO, estado activo.", "Cliente actualizado.", ["El código de cliente no es editable.", "Validaciones iguales al alta.", "Un cliente inactivo no puede recibir contenedores nuevos (regla aplicada en módulo Contenedores).", "Mostrar mensaje de confirmación al guardar."]),
        createReqBox("RF-CLI-004", "Restricción por estado inactivo", "Los clientes inactivos deben quedar excluidos de operaciones que asignen nuevos contenedores.", "Sistema", "Cliente marcado como inactivo.", "Estado activo = false.", "Cliente no disponible en listas de selección para nuevos contenedores.", ["Clientes inactivos no aparecen en dropdown de alta de contenedor.", "Si un contenedor ya tenía asignado un cliente inactivo, debe preservarse en edición."]),

        createHeading2("5.3 Módulo: Contenedores (RF-CON)"),
        createReqBox("RF-CON-001", "Consulta de contenedores", "El sistema debe listar todos los contenedores con código, nombre, cliente asociado y estado.", "Administrador / Operador", "Existen contenedores registrados.", "Ninguno.", "Tabla con datos del contenedor y su cliente.", ["Permitir navegar a creación y edición."]),
        createReqBox("RF-CON-002", "Alta de contenedor", "El sistema debe registrar un contenedor vinculado a un cliente activo.", "Administrador", "Existe al menos un cliente activo.", "Código contenedor (máx. 20), nombre (máx. 150), cliente (dropdown), estado activo.", "Contenedor creado.", ["Código único.", "Cliente obligatorio y debe estar activo.", "Estado activo por defecto.", "Mostrar mensaje si no hay clientes activos disponibles."]),
        createReqBox("RF-CON-003", "Edición de contenedor", "El sistema debe permitir modificar nombre, cliente y estado de un contenedor existente.", "Administrador", "Contenedor existe.", "Código (solo lectura), nombre, cliente, estado activo.", "Contenedor actualizado.", ["No permitir cambiar a un cliente inactivo salvo que ya estuviera asignado.", "Código no editable."]),
        createReqBox("RF-CON-004", "Contenedores activos en ingreso", "Solo contenedores activos deben estar disponibles para registrar ingresos al patio.", "Operador de patio", "Contenedor con estado activo/inactivo.", "Estado del contenedor.", "Contenedor disponible o excluido del dropdown de ingreso.", ["En edición de ingreso, preservar contenedor inactivo si ya estaba asignado."]),

        createHeading2("5.4 Módulo: Especialidades de Mantenimiento (RF-ESP)"),
        createReqBox("RF-ESP-001", "Consulta de especialidades", "Listar todas las especialidades/rubros técnicos de mantenimiento.", "Administrador", "Ninguna.", "Ninguno.", "Tabla con código y nombre de especialidad.", ["Permitir crear y editar desde la lista."]),
        createReqBox("RF-ESP-002", "Alta de especialidad", "Registrar un nuevo rubro de inspección técnica.", "Administrador", "Código no duplicado.", "Código especialidad (máx. 20), nombre especialidad (máx. 150).", "Especialidad registrada.", ["Código obligatorio y único.", "Nombre obligatorio."]),
        createReqBox("RF-ESP-003", "Edición de especialidad", "Modificar el nombre de una especialidad existente.", "Administrador", "Especialidad existe.", "Código (solo lectura), nombre.", "Especialidad actualizada.", ["Código no editable.", "No eliminar si tiene previajes o tareas asociadas (validación recomendada para nueva app)."]),

        createHeading2("5.5 Módulo: Especialidades del Técnico (RF-TEC)"),
        createReqBox("RF-TEC-001", "Consulta de técnicos", "Listar técnicos/empleados con su usuario y especialidad asignada.", "Jefe de taller", "Ninguna.", "Ninguno.", "Tabla con código técnico, nombre, usuario y especialidad.", ["Mostrar la especialidad principal asignada a cada técnico."]),
        createReqBox("RF-TEC-002", "Alta de técnico con especialidad", "Registrar un técnico y asignarle una especialidad de mantenimiento.", "Jefe de taller", "Especialidad existe.", "Código técnico (máx. 20), nombre (máx. 150), usuario (máx. 50, opcional), especialidad.", "Técnico creado y relación guardada en tabla puente técnico-especialidad.", ["Código técnico único.", "Debe seleccionarse una especialidad válida.", "La relación se almacena en cpp_espdelemp.", "En la UI actual solo se permite una especialidad por técnico."]),
        createReqBox("RF-TEC-003", "Edición de técnico", "Modificar datos del técnico y reasignar especialidad.", "Jefe de taller", "Técnico existe.", "Código (solo lectura), nombre, usuario, especialidad.", "Técnico y relación actualizados.", ["Al cambiar especialidad, actualizar registro en cpp_espdelemp.", "El técnico solo puede ser asignado a previajes de su especialidad."]),
        createReqBox("RF-TEC-004", "Validación de competencia en previaje", "El técnico seleccionado en un previaje debe tener la especialidad del previaje asignada.", "Sistema", "Registro de previaje en curso.", "Código técnico, código especialidad del previaje.", "Validación exitosa o mensaje de error.", ["Si el técnico no tiene la especialidad, rechazar el registro.", "Filtrar técnicos disponibles según especialidad seleccionada en formulario."]),

        createHeading2("5.6 Módulo: Tareas de Mantenimiento (RF-TAR)"),
        createReqBox("RF-TAR-001", "Consulta de tareas", "Listar tareas estandarizadas con número, nombre, especialidad, tiempo estimado y estado.", "Jefe de operaciones", "Ninguna.", "Ninguno.", "Tabla de tareas de mantenimiento.", ["Mostrar especialidad asociada y tiempo en horas."]),
        createReqBox("RF-TAR-002", "Alta de tarea", "Registrar una tarea de mantenimiento vinculada a una especialidad.", "Jefe de operaciones", "Especialidad existe.", "Número de tarea (entero, manual), nombre (máx. 200), especialidad, tiempo estimado (decimal, default 0.50 h), estado activo.", "Tarea creada.", ["Número de tarea único y mayor a 0.", "Especialidad obligatoria y válida.", "Tiempo estimado en horas con 2 decimales.", "Estado activo por defecto."]),
        createReqBox("RF-TAR-003", "Edición de tarea", "Modificar datos de una tarea existente.", "Jefe de operaciones", "Tarea existe.", "Número (solo lectura), nombre, especialidad, tiempo estimado, estado.", "Tarea actualizada.", ["Número de tarea no editable."]),
        createReqBox("RF-TAR-004", "Uso futuro en previaje (PENDIENTE)", "Las tareas deben poder asociarse al detalle de un previaje para cotización/presupuesto.", "Inspector PTI", "Previaje registrado, tareas activas por especialidad.", "Tareas seleccionadas, cantidad, garantía, técnico, precio unitario.", "Detalle de tareas persistido con totales calculados.", ["Modelo TrPreviajeTareaDetalle definido pero NO implementado en sistema actual.", "Debe calcular total usando tarifas del cliente (imp_mov_mo / imp_mov_mo2 según garantía).", "Campo garantía determina qué tarifa de mano de obra aplicar."]),

        createHeading2("5.7 Módulo: Ingreso de Contenedor (RF-ING)"),
        createReqBox("RF-ING-001", "Consulta de ingresos", "Listar historial de ingresos con número, contenedor y fecha.", "Operador de patio", "Ninguna.", "Ninguno.", "Tabla de ingresos al patio.", ["Mostrar número consecutivo autogenerado.", "Mostrar código/nombre del contenedor."]),
        createReqBox("RF-ING-002", "Registro de ingreso a patio", "Registrar la entrada de un contenedor al proceso de mantenimiento.", "Operador de patio / Gate", "Contenedor activo existe.", "Contenedor (dropdown), fecha/hora de ingreso.", "Ingreso creado con NumIngreso autogenerado (IDENTITY).", ["NumIngreso es consecutivo autogenerado por el sistema.", "Fecha de ingreso obligatoria; default = fecha/hora actual.", "Solo contenedores activos en dropdown (salvo edición con contenedor ya asignado).", "Mostrar mensaje de confirmación."]),
        createReqBox("RF-ING-003", "Edición de ingreso", "Corregir contenedor o fecha de un ingreso existente.", "Operador de patio", "Ingreso existe.", "Número ingreso (solo lectura), contenedor, fecha.", "Ingreso actualizado.", ["NumIngreso no editable.", "Validar contenedor activo igual que en alta."]),
        createReqBox("RF-ING-004", "Prerequisito para previaje", "Un ingreso registrado es prerequisito para crear inspecciones previaje.", "Sistema", "Ingreso registrado.", "NumIngreso.", "Ingreso disponible en formulario de previaje.", ["Solo ingresos con especialidades pendientes aparecen en alta de previaje."]),

        createHeading2("5.8 Módulo: Previaje de Contenedor — PTI (RF-PRV)"),
        createReqBox("RF-PRV-001", "Consulta de previajes", "Listar inspecciones previaje con datos del ingreso, contenedor, cliente, especialidad, técnico, fecha y estado habilitado.", "Inspector PTI / Supervisor", "Ninguna.", "Ninguno.", "Tabla completa de previajes.", ["Estado Habilitado visible como Aprobado/Rechazado.", "Mostrar relaciones: ingreso → contenedor → cliente."]),
        createReqBox("RF-PRV-002", "Registro de inspección previaje", "Registrar una inspección PTI para un ingreso y especialidad específicos.", "Inspector PTI", "Ingreso existe; especialidad no registrada previamente para ese ingreso; técnico calificado.", "Ingreso, especialidad, técnico, fecha previaje, observaciones (máx. 1000), habilitado (switch).", "Previaje creado con NroPreviaje autogenerado.", ["NroPreviaje consecutivo autogenerado.", "Unicidad: máximo 1 previaje por combinación ingreso + especialidad.", "Técnico debe tener la especialidad seleccionada.", "Fecha previaje obligatoria; default = ahora.", "Habilitado default = false (no habilitado).", "Observaciones opcionales hasta 1000 caracteres."]),
        createReqBox("RF-PRV-003", "Edición de previaje", "Modificar datos de una inspección previaje existente.", "Inspector PTI", "Previaje existe.", "Nro previaje (solo lectura), ingreso, especialidad, técnico, fecha, observaciones, habilitado.", "Previaje actualizado.", ["Validaciones iguales al alta.", "En edición, permitir el ingreso y especialidad actuales aunque ya estén 'ocupados'."]),
        createReqBox("RF-PRV-004", "Filtrado dinámico de ingresos", "Al registrar previaje, solo mostrar ingresos con especialidades pendientes de inspección.", "Sistema", "Catálogo de especialidades e ingresos con previajes parciales.", "Lista de especialidades e ingresos.", "Dropdown filtrado de ingresos.", ["Un ingreso desaparece del dropdown cuando ya tiene previaje para todas las especialidades.", "En edición, incluir siempre el ingreso actual aunque esté completo."]),
        createReqBox("RF-PRV-005", "Filtrado dinámico de especialidades", "Al seleccionar un ingreso, mostrar solo especialidades aún no registradas para ese ingreso.", "Sistema", "Ingreso seleccionado.", "NumIngreso.", "Dropdown de especialidades filtrado.", ["Excluir especialidades ya inspeccionadas para el ingreso.", "En edición, incluir la especialidad actual."]),
        createReqBox("RF-PRV-006", "Filtrado dinámico de técnicos", "Al seleccionar especialidad, filtrar técnicos que tengan esa especialidad asignada.", "Sistema", "Especialidad seleccionada.", "CodEspMtto.", "Dropdown de técnicos filtrado.", ["Solo técnicos con competencia en la especialidad.", "Comportamiento implementado con JavaScript en cliente."]),
        createReqBox("RF-PRV-007", "Visualización contextual del ingreso", "Al seleccionar un ingreso, mostrar contenedor y cliente en campos de solo lectura.", "Inspector PTI", "Ingreso seleccionado.", "NumIngreso.", "Campos readonly: código/nombre contenedor, código/nombre cliente.", ["Datos obtenidos de la relación ingreso → contenedor → cliente."]),
        createReqBox("RF-PRV-008", "Determinación de habilitación", "El campo Habilitado indica si el contenedor aprueba la inspección en ese rubro técnico.", "Inspector PTI", "Previaje en registro/edición.", "Valor booleano Habilitado.", "Estado persistido: 1 = Habilitado/Aprobado, 0 = No habilitado/Rechazado.", ["Default al crear: No habilitado.", "No existe cálculo automático de estado global del contenedor en sistema actual."]),

        new PageBreak(),

        createHeading1("6. REQUERIMIENTOS NO FUNCIONALES"),
        createHeading2("6.1 Requerimientos de interfaz (RNF-UI)"),
        createBullet("Interfaz web responsive con Bootstrap 5.", "RNF-UI-001:"),
        createBullet("Menú de navegación persistente en layout principal con acceso a todos los módulos.", "RNF-UI-002:"),
        createBullet("Formularios con validación en cliente (jQuery Validation) y servidor (DataAnnotations).", "RNF-UI-003:"),
        createBullet("Mensajes de éxito mediante TempData tras operaciones de guardado.", "RNF-UI-004:"),
        createBullet("Pantalla de inicio con menú tipo árbol de archivos y accesos rápidos.", "RNF-UI-005:"),

        createHeading2("6.2 Requerimientos de datos (RNF-DAT)"),
        createBullet("Base de datos relacional SQL Server.", "RNF-DAT-001:"),
        createBullet("Nomenclatura de tablas: ct_ (catálogo), tr_ (transaccional), cpp_ (relación N:N).", "RNF-DAT-002:"),
        createBullet("Integridad referencial con claves foráneas.", "RNF-DAT-003:"),
        createBullet("Índice UNIQUE en previaje por (num_ingreso, cod_esp_mtto).", "RNF-DAT-004:"),
        createBullet("Campos IDENTITY para NumIngreso y NroPreviaje.", "RNF-DAT-005:"),
        createBullet("Creación automática de base de datos y tablas si no existen (bootstrap DDL).", "RNF-DAT-006:"),

        createHeading2("6.3 Requerimientos de seguridad (RNF-SEG) — Recomendados para nueva app"),
        createBullet("Autenticación de usuarios (no implementada en sistema actual).", "RNF-SEG-001:"),
        createBullet("Autorización por rol según actores definidos.", "RNF-SEG-002:"),
        createBullet("Protección CSRF con token anti-falsificación en formularios POST.", "RNF-SEG-003:"),
        createBullet("Auditoría de cambios en registros transaccionales.", "RNF-SEG-004:"),

        createHeading2("6.4 Requerimientos de arquitectura (RNF-ARQ)"),
        createBullet("Separación en capas: Controladores, Modelos, Repositorios, DbContext.", "RNF-ARQ-001:"),
        createBullet("Inyección de dependencias para repositorios.", "RNF-ARQ-002:"),
        createBullet("Patrón Repositorio con interfaces desacopladas.", "RNF-ARQ-003:"),
        createBullet("Implementaciones alternativas InMemory para pruebas (opcional).", "RNF-ARQ-004:"),

        createHeading1("7. REGLAS DE NEGOCIO TRANSVERSALES"),
        createDataTable(
            ["ID", "Regla", "Módulos afectados"],
            [
                ["RN-001", "No existe funcionalidad de eliminación en ningún módulo", "Todos"],
                ["RN-002", "Códigos de catálogo son claves primarias definidas por el usuario (no autogeneradas)", "Clientes, Contenedores, Especialidades, Técnicos"],
                ["RN-003", "Números transaccionales son autogenerados (IDENTITY)", "Ingresos, Previajes"],
                ["RN-004", "Estado activo/inactivo controla disponibilidad en dropdowns", "Clientes, Contenedores, Tareas"],
                ["RN-005", "Validación de unicidad en servidor antes de persistir", "Todos los catálogos"],
                ["RN-006", "Mensajes de error descriptivos en español", "Todos los formularios"],
                ["RN-007", "Tarifas MO del cliente almacenadas pero no usadas en cálculos actuales", "Clientes, Previaje (futuro)"],
                ["RN-008", "Un técnico tiene una especialidad principal en la UI actual", "Técnicos, Previaje"]
            ]
        ),

        createHeading1("8. DICCIONARIO DE DATOS"),
        createHeading2("8.1 Tablas de catálogo (ct_)"),
        createDataTable(
            ["Tabla", "Campo", "Tipo", "Restricción", "Descripción"],
            [
                ["ct_clientemtto", "cod_cliente", "NVARCHAR(20)", "PK, NOT NULL", "Código único del cliente"],
                ["ct_clientemtto", "nombre_cliente", "NVARCHAR(150)", "NOT NULL", "Nombre o razón social"],
                ["ct_clientemtto", "cod_dpto", "NVARCHAR(20)", "NULL", "Centro de costo"],
                ["ct_clientemtto", "imp_mov_mo", "DECIMAL(18,2)", "NOT NULL, DEFAULT 0", "Tarifa mano de obra sin garantía"],
                ["ct_clientemtto", "imp_mov_mo2", "DECIMAL(18,2)", "NOT NULL, DEFAULT 0", "Tarifa mano de obra con garantía"],
                ["ct_clientemtto", "activo", "BIT", "NOT NULL, DEFAULT 1", "Estado activo/inactivo"],
                ["ct_mcontenedor", "cod_contenedor", "NVARCHAR(20)", "PK, NOT NULL", "Código único del contenedor"],
                ["ct_mcontenedor", "nombre", "NVARCHAR(150)", "NOT NULL", "Nombre/descripción del contenedor"],
                ["ct_mcontenedor", "cod_cliente", "NVARCHAR(20)", "FK, NOT NULL", "Cliente propietario"],
                ["ct_mcontenedor", "activo", "BIT", "NOT NULL, DEFAULT 1", "Estado activo/inactivo"],
                ["ct_espmtto", "cod_esp_mtto", "NVARCHAR(20)", "PK, NOT NULL", "Código de especialidad"],
                ["ct_espmtto", "nom_esp_mtto", "NVARCHAR(150)", "NOT NULL", "Nombre de la especialidad"],
                ["ct_espdelemp", "cod_tit", "NVARCHAR(20)", "PK, NOT NULL", "Código del técnico"],
                ["ct_espdelemp", "nom_tit", "NVARCHAR(150)", "NOT NULL", "Nombre del técnico"],
                ["ct_espdelemp", "usuario", "NVARCHAR(50)", "NULL", "Referencia a usuario del sistema"],
                ["ct_tareademtto", "nro_tarea", "INT", "PK, NOT NULL", "Número de tarea (manual)"],
                ["ct_tareademtto", "nombre_tarea", "NVARCHAR(200)", "NOT NULL", "Descripción de la tarea"],
                ["ct_tareademtto", "cod_esp_mtto", "NVARCHAR(20)", "FK, NOT NULL", "Especialidad asociada"],
                ["ct_tareademtto", "tiempo_estimado", "DECIMAL(6,2)", "NOT NULL, DEFAULT 0", "Tiempo en horas"],
                ["ct_tareademtto", "activo", "BIT", "NOT NULL, DEFAULT 1", "Estado activo/inactivo"]
            ]
        ),
        createHeading2("8.2 Tablas de relación (cpp_)"),
        createDataTable(
            ["Tabla", "Campo", "Tipo", "Restricción", "Descripción"],
            [
                ["cpp_espdelemp", "cod_tit", "NVARCHAR(20)", "PK, FK", "Código del técnico"],
                ["cpp_espdelemp", "cod_esp_mtto", "NVARCHAR(20)", "PK, FK", "Especialidad asignada al técnico"]
            ]
        ),
        createHeading2("8.3 Tablas transaccionales (tr_)"),
        createDataTable(
            ["Tabla", "Campo", "Tipo", "Restricción", "Descripción"],
            [
                ["tr_ingresocontenedor", "num_ingreso", "INT", "PK, IDENTITY", "Número consecutivo de ingreso"],
                ["tr_ingresocontenedor", "cod_contenedor", "NVARCHAR(20)", "FK, NOT NULL", "Contenedor que ingresa"],
                ["tr_ingresocontenedor", "fec_ingreso", "DATETIME2", "NOT NULL", "Fecha y hora de ingreso"],
                ["tr_previajecontenedor", "nro_previaje", "INT", "PK, IDENTITY", "Número consecutivo de previaje"],
                ["tr_previajecontenedor", "num_ingreso", "INT", "FK, NOT NULL", "Ingreso asociado"],
                ["tr_previajecontenedor", "cod_esp_mtto", "NVARCHAR(20)", "FK, NOT NULL, UNIQUE con num_ingreso", "Especialidad inspeccionada"],
                ["tr_previajecontenedor", "cod_tit", "NVARCHAR(20)", "FK, NOT NULL", "Técnico inspector"],
                ["tr_previajecontenedor", "fec_previaje", "DATETIME2", "NOT NULL", "Fecha de la inspección"],
                ["tr_previajecontenedor", "observaciones", "NVARCHAR(1000)", "NULL", "Notas de la inspección"],
                ["tr_previajecontenedor", "habilitado", "BIT", "NOT NULL, DEFAULT 0", "Resultado: habilitado o no"]
            ]
        ),
        createHeading2("8.4 Entidad preparada no implementada"),
        createDataTable(
            ["Entidad", "Campo", "Tipo", "Descripción"],
            [
                ["TrPreviajeTareaDetalle", "nro_previaje", "INT", "FK al previaje"],
                ["TrPreviajeTareaDetalle", "nro_tarea", "INT", "FK a tarea de mantenimiento"],
                ["TrPreviajeTareaDetalle", "cantidad", "DECIMAL", "Cantidad de la tarea"],
                ["TrPreviajeTareaDetalle", "garantia", "BIT", "Indica si aplica tarifa con garantía"],
                ["TrPreviajeTareaDetalle", "cod_tit", "NVARCHAR(20)", "Técnico que ejecuta la tarea"]
            ]
        ),

        createHeading1("9. DIAGRAMA DE RELACIONES ENTRE ENTIDADES"),
        createParagraph("ct_clientemtto (1) ──< ct_mcontenedor (N) ──< tr_ingresocontenedor (N) ──< tr_previajecontenedor (N)"),
        createParagraph("ct_espmtto (1) ──< ct_tareademtto (N)"),
        createParagraph("ct_espmtto (1) ──< cpp_espdelemp (N) >── ct_espdelemp (1)"),
        createParagraph("ct_espmtto (1) ──< tr_previajecontenedor (N)"),
        createParagraph("ct_espdelemp (1) ──< tr_previajecontenedor (N)"),

        createHeading1("10. FUNCIONALIDADES NO IMPLEMENTADAS — REQUERIMIENTOS FUTUROS"),
        createHeading2("10.1 Detalle de tareas en previaje (RF-FUT-001)"),
        createParagraph("El sistema actual define el modelo TrPreviajeTareaDetalle y el ViewModel PreviajeTareaEditorRowViewModel con campos para selección de tarea, cantidad, garantía, precio unitario y totales, pero no tiene UI ni persistencia. La nueva aplicación debe implementar:"),
        createBullet("Grilla editable de tareas filtradas por especialidad del previaje."),
        createBullet("Cálculo de precio unitario según tarifa del cliente (con/sin garantía)."),
        createBullet("Cálculo de total por línea y total general del previaje."),
        createBullet("Persistencia en tabla tr_previajetarea_detalle (a crear)."),

        createHeading2("10.2 Autenticación y control de acceso (RF-FUT-002)"),
        createParagraph("Implementar login, roles (Administrador, Operador, Inspector, Jefe) y restricción de módulos por permiso."),

        createHeading2("10.3 Reportes e impresión (RF-FUT-003)"),
        createParagraph("Generar reportes de ingresos, previajes por período, contenedores habilitados/rechazados, cotizaciones por previaje. Exportación PDF/Excel."),

        createHeading2("10.4 Estado global del contenedor (RF-FUT-004)"),
        createParagraph("Calcular automáticamente si un contenedor está completamente habilitado cuando todos sus previajes por especialidad están aprobados."),

        createHeading2("10.5 Ciclo completo de mantenimiento (RF-FUT-005)"),
        createParagraph("Extender el flujo más allá del previaje: orden de trabajo, ejecución de reparaciones, salida de patio."),

        createHeading2("10.6 Eliminación lógica/física (RF-FUT-006)"),
        createParagraph("Implementar baja lógica o eliminación controlada con validación de dependencias."),

        createHeading1("11. CASOS DE USO RESUMIDOS"),
        createDataTable(
            ["Caso de uso", "Actor", "Flujo principal", "Resultado"],
            [
                ["CU-01 Configurar cliente", "Administrador", "Crear cliente con tarifas → Guardar", "Cliente disponible para contenedores"],
                ["CU-02 Registrar contenedor", "Administrador", "Crear contenedor → Asignar cliente activo → Guardar", "Contenedor disponible para ingreso"],
                ["CU-03 Ingreso a patio", "Operador", "Seleccionar contenedor → Registrar fecha → Guardar", "NumIngreso generado"],
                ["CU-04 Inspección previaje", "Inspector", "Seleccionar ingreso → Especialidad → Técnico → Evaluar → Guardar", "Previaje registrado con estado"],
                ["CU-05 Completar inspecciones", "Inspector", "Repetir CU-04 por cada especialidad pendiente", "Ingreso con todos los rubros inspeccionados"],
                ["CU-06 Gestionar tareas", "Jefe operaciones", "Crear tareas por especialidad con tiempo estimado", "Catálogo listo para cotización futura"]
            ]
        ),

        createHeading1("12. CRITERIOS DE ACEPTACIÓN GENERALES"),
        createBullet("Todos los formularios validan campos obligatorios antes de guardar.", "CA-001:"),
        createBullet("Los códigos duplicados son rechazados con mensaje claro.", "CA-002:"),
        createBullet("Las relaciones FK impiden datos huérfanos.", "CA-003:"),
        createBullet("Los dropdowns respetan reglas de activo/inactivo.", "CA-004:"),
        createBullet("El previaje no permite duplicar especialidad por ingreso.", "CA-005:"),
        createBullet("El técnico debe coincidir con la especialidad del previaje.", "CA-006:"),
        createBullet("Los números transaccionales son autogenerados e inmutables.", "CA-007:"),
        createBullet("La navegación permite acceder a todos los módulos desde menú principal.", "CA-008:"),

        createHeading1("13. GLOSARIO"),
        createDataTable(
            ["Término", "Definición"],
            [
                ["PTI", "Pre-Trip Inspection. Inspección técnica previa al viaje/operación del contenedor."],
                ["Previaje", "Registro de inspección PTI asociado a un ingreso y una especialidad."],
                ["Ingreso", "Evento de entrada de un contenedor al patio de mantenimiento."],
                ["Especialidad", "Rubro técnico de inspección (ej. estructura, refrigeración)."],
                ["Habilitado", "Indicador de que el contenedor aprueba la inspección en un rubro."],
                ["MO", "Mano de obra. Tarifa aplicable por hora o unidad de trabajo."],
                ["Garantía", "Indicador de si una tarea/reparación está cubierta por garantía del cliente."],
                ["Maestro", "Catálogo de datos de referencia del sistema."],
                ["Transacción", "Registro operativo que modifica el estado del negocio."]
            ]
        ),

        createHeading1("14. CONCLUSIÓN"),
        createParagraph("El proyecto MantenimientoDeContenedores implementa un MVP funcional para la gestión de mantenimiento de contenedores con 7 módulos operativos (5 maestros + 2 transaccionales) y una pantalla de inicio. El núcleo del negocio es el flujo Ingreso → Previaje PTI, con validaciones robustas de integridad y reglas de negocio. Para la nueva aplicación se recomienda implementar todos los requerimientos funcionales descritos en la sección 5, los requerimientos no funcionales de la sección 6, y priorizar las funcionalidades futuras de la sección 10 — especialmente el detalle de tareas con cotización, autenticación por roles y reportes operativos.")
    ];

    const doc = new Document({
        sections: [{
            properties: {
                page: { margin: { top: 1440, bottom: 1440, left: 1440, right: 1440 } }
            },
            children
        }]
    });

    const outputPath = path.join(__dirname, '..', 'Documento_Requerimientos_Sistema_Mantenimiento_Contenedores.docx');
    const buffer = await Packer.toBuffer(doc);
    fs.writeFileSync(outputPath, buffer);
    console.log(`Documento Word generado: ${outputPath}`);
}

generateReport().catch(err => {
    console.error("Error al generar el documento Word:", err);
    process.exit(1);
});
