using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class InMemoryEspecialidadMttoRepository : IEspecialidadMttoRepository
{
    private readonly List<CtEspMtto> _especialidades = [];
    private readonly object _lock = new();

    public IReadOnlyCollection<CtEspMtto> GetAll()
    {
        lock (_lock)
        {
            return _especialidades
                .OrderBy(x => x.CodEspMtto)
                .ToList()
                .AsReadOnly();
        }
    }

    public bool Exists(string codEspMtto)
    {
        lock (_lock)
        {
            return _especialidades.Any(x => string.Equals(x.CodEspMtto, codEspMtto, StringComparison.OrdinalIgnoreCase));
        }
    }

    public CtEspMtto? GetByCode(string codEspMtto)
    {
        lock (_lock)
        {
            return _especialidades
                .FirstOrDefault(x => string.Equals(x.CodEspMtto, codEspMtto, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void Add(CtEspMtto especialidad)
    {
        lock (_lock)
        {
            _especialidades.Add(new CtEspMtto
            {
                CodEspMtto = especialidad.CodEspMtto.Trim(),
                NomEspMtto = especialidad.NomEspMtto.Trim(),
            });
        }
    }

    public void Update(CtEspMtto especialidad)
    {
        lock (_lock)
        {
            var existing = _especialidades
                .FirstOrDefault(x => string.Equals(x.CodEspMtto, especialidad.CodEspMtto, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                throw new InvalidOperationException("No se encontró la especialidad a actualizar.");
            }

            existing.NomEspMtto = especialidad.NomEspMtto.Trim();
        }
    }
}
