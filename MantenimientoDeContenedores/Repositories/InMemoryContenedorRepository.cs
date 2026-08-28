using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class InMemoryContenedorRepository : IContenedorRepository
{
    private readonly List<CtMContenedor> _contenedores = [];
    private readonly object _lock = new();

    public IReadOnlyCollection<CtMContenedor> GetAll()
    {
        lock (_lock)
        {
            return _contenedores
                .OrderBy(x => x.CodContenedor)
                .ToList()
                .AsReadOnly();
        }
    }

    public bool Exists(string codContenedor)
    {
        lock (_lock)
        {
            return _contenedores.Any(x => string.Equals(x.CodContenedor, codContenedor, StringComparison.OrdinalIgnoreCase));
        }
    }

    public CtMContenedor? GetByCode(string codContenedor)
    {
        lock (_lock)
        {
            return _contenedores
                .FirstOrDefault(x => string.Equals(x.CodContenedor, codContenedor, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void Add(CtMContenedor contenedor)
    {
        lock (_lock)
        {
            _contenedores.Add(new CtMContenedor
            {
                CodContenedor = contenedor.CodContenedor.Trim(),
                Nombre = contenedor.Nombre.Trim(),
                CodCliente = contenedor.CodCliente.Trim(),
                Activo = contenedor.Activo,
            });
        }
    }

    public void Update(CtMContenedor contenedor)
    {
        lock (_lock)
        {
            var existing = _contenedores
                .FirstOrDefault(x => string.Equals(x.CodContenedor, contenedor.CodContenedor, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                throw new InvalidOperationException("No se encontró el contenedor a actualizar.");
            }

            existing.Nombre = contenedor.Nombre.Trim();
            existing.CodCliente = contenedor.CodCliente.Trim();
            existing.Activo = contenedor.Activo;
        }
    }
}
