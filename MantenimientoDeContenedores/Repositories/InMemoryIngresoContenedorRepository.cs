using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class InMemoryIngresoContenedorRepository : IIngresoContenedorRepository
{
    private readonly List<TrIngresoContenedor> _ingresos = [];
    private readonly object _lock = new();
    private int _nextId = 1;

    public IReadOnlyCollection<TrIngresoContenedor> GetAll()
    {
        lock (_lock)
        {
            return _ingresos
                .OrderByDescending(x => x.NumIngreso)
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public TrIngresoContenedor? GetById(int numIngreso)
    {
        lock (_lock)
        {
            var ingreso = _ingresos.FirstOrDefault(x => x.NumIngreso == numIngreso);
            return ingreso is null ? null : Clone(ingreso);
        }
    }

    public void Add(TrIngresoContenedor ingreso)
    {
        lock (_lock)
        {
            _ingresos.Add(new TrIngresoContenedor
            {
                NumIngreso = _nextId++,
                CodContenedor = ingreso.CodContenedor.Trim(),
                FecIngreso = ingreso.FecIngreso,
            });
        }
    }

    public void Update(TrIngresoContenedor ingreso)
    {
        lock (_lock)
        {
            var existing = _ingresos.FirstOrDefault(x => x.NumIngreso == ingreso.NumIngreso);

            if (existing is null)
            {
                throw new InvalidOperationException("No se encontró el ingreso a actualizar.");
            }

            existing.CodContenedor = ingreso.CodContenedor.Trim();
            existing.FecIngreso = ingreso.FecIngreso;
        }
    }

    private static TrIngresoContenedor Clone(TrIngresoContenedor ingreso)
    {
        return new TrIngresoContenedor
        {
            NumIngreso = ingreso.NumIngreso,
            CodContenedor = ingreso.CodContenedor,
            FecIngreso = ingreso.FecIngreso,
        };
    }
}
