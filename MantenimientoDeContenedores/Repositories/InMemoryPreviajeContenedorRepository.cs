using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class InMemoryPreviajeContenedorRepository : IPreviajeContenedorRepository
{
    private readonly List<TrPreviajeContenedor> _previajes = [];
    private readonly object _lock = new();
    private int _nextId = 1;

    public IReadOnlyCollection<TrPreviajeContenedor> GetAll()
    {
        lock (_lock)
        {
            return _previajes
                .OrderByDescending(x => x.NroPreviaje)
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public TrPreviajeContenedor? GetById(int nroPreviaje)
    {
        lock (_lock)
        {
            var previaje = _previajes.FirstOrDefault(x => x.NroPreviaje == nroPreviaje);
            return previaje is null ? null : Clone(previaje);
        }
    }

    public bool ExistsForIngresoEspecialidad(int numIngreso, string codEspMtto, int? excludeNroPreviaje = null)
    {
        lock (_lock)
        {
            return _previajes.Any(x =>
                x.NumIngreso == numIngreso
                && string.Equals(x.CodEspMtto, codEspMtto, StringComparison.OrdinalIgnoreCase)
                && (!excludeNroPreviaje.HasValue || x.NroPreviaje != excludeNroPreviaje.Value));
        }
    }

    public int Add(TrPreviajeContenedor previaje)
    {
        lock (_lock)
        {
            var entity = new TrPreviajeContenedor
            {
                NroPreviaje = _nextId++,
                NumIngreso = previaje.NumIngreso,
                CodEspMtto = previaje.CodEspMtto.Trim(),
                CodTit = previaje.CodTit.Trim(),
                FecPreviaje = previaje.FecPreviaje,
                Observaciones = previaje.Observaciones.Trim(),
                Habilitado = previaje.Habilitado,
            };

            _previajes.Add(entity);

            return entity.NroPreviaje;
        }
    }

    public IReadOnlyCollection<TrPreviajeTareaDetalle> GetTaskDetails(int nroPreviaje)
    {
        return Array.Empty<TrPreviajeTareaDetalle>();
    }

    public void ReplaceTaskDetails(int nroPreviaje, IReadOnlyCollection<TrPreviajeTareaDetalle> detalles)
    {
    }

    public void Update(TrPreviajeContenedor previaje)
    {
        lock (_lock)
        {
            var existing = _previajes.FirstOrDefault(x => x.NroPreviaje == previaje.NroPreviaje);

            if (existing is null)
            {
                throw new InvalidOperationException("No se encontró el previaje a actualizar.");
            }

            existing.NumIngreso = previaje.NumIngreso;
            existing.CodEspMtto = previaje.CodEspMtto.Trim();
            existing.CodTit = previaje.CodTit.Trim();
            existing.FecPreviaje = previaje.FecPreviaje;
            existing.Observaciones = previaje.Observaciones.Trim();
            existing.Habilitado = previaje.Habilitado;
        }
    }

    private static TrPreviajeContenedor Clone(TrPreviajeContenedor previaje)
    {
        return new TrPreviajeContenedor
        {
            NroPreviaje = previaje.NroPreviaje,
            NumIngreso = previaje.NumIngreso,
            CodEspMtto = previaje.CodEspMtto,
            CodTit = previaje.CodTit,
            FecPreviaje = previaje.FecPreviaje,
            Observaciones = previaje.Observaciones,
            Habilitado = previaje.Habilitado,
        };
    }
}
