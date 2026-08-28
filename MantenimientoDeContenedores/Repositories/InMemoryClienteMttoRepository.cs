using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class InMemoryClienteMttoRepository : IClienteMttoRepository
{
    private readonly List<CtClienteMtto> _clientes = [];
    private readonly object _lock = new();

    public IReadOnlyCollection<CtClienteMtto> GetAll()
    {
        lock (_lock)
        {
            return _clientes
                .OrderBy(x => x.CodCliente)
                .ToList()
                .AsReadOnly();
        }
    }

    public IReadOnlyCollection<CtClienteMtto> GetActive()
    {
        lock (_lock)
        {
            return _clientes
                .Where(x => x.Activo)
                .OrderBy(x => x.CodCliente)
                .ToList()
                .AsReadOnly();
        }
    }

    public bool Exists(string codCliente)
    {
        lock (_lock)
        {
            return _clientes.Any(x => string.Equals(x.CodCliente, codCliente, StringComparison.OrdinalIgnoreCase));
        }
    }

    public CtClienteMtto? GetByCode(string codCliente)
    {
        lock (_lock)
        {
            return _clientes
                .FirstOrDefault(x => string.Equals(x.CodCliente, codCliente, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void Add(CtClienteMtto cliente)
    {
        lock (_lock)
        {
            _clientes.Add(new CtClienteMtto
            {
                CodCliente = cliente.CodCliente.Trim(),
                NombreCliente = cliente.NombreCliente.Trim(),
                CodDpto = cliente.CodDpto.Trim(),
                ImpMovMo = cliente.ImpMovMo,
                ImpMovMo2 = cliente.ImpMovMo2,
                Activo = cliente.Activo,
            });
        }
    }

    public void Update(CtClienteMtto cliente)
    {
        lock (_lock)
        {
            var existing = _clientes
                .FirstOrDefault(x => string.Equals(x.CodCliente, cliente.CodCliente, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                throw new InvalidOperationException("No se encontró el cliente a actualizar.");
            }

            existing.NombreCliente = cliente.NombreCliente.Trim();
            existing.CodDpto = cliente.CodDpto.Trim();
            existing.ImpMovMo = cliente.ImpMovMo;
            existing.ImpMovMo2 = cliente.ImpMovMo2;
            existing.Activo = cliente.Activo;
        }
    }
}
