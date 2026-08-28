using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Data;
using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class SqlIngresoContenedorRepository : IIngresoContenedorRepository
{
    private readonly MantenimientoDbContext _dbContext;

    public SqlIngresoContenedorRepository(MantenimientoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<TrIngresoContenedor> GetAll()
    {
        return _dbContext.IngresosContenedor
            .AsNoTracking()
            .Include(x => x.Contenedor)
                .ThenInclude(x => x!.Cliente)
            .OrderByDescending(x => x.NumIngreso)
            .ToList()
            .AsReadOnly();
    }

    public TrIngresoContenedor? GetById(int numIngreso)
    {
        return _dbContext.IngresosContenedor
            .AsNoTracking()
            .Include(x => x.Contenedor)
                .ThenInclude(x => x!.Cliente)
            .FirstOrDefault(x => x.NumIngreso == numIngreso);
    }

    public void Add(TrIngresoContenedor ingreso)
    {
        _dbContext.IngresosContenedor.Add(new TrIngresoContenedor
        {
            CodContenedor = ingreso.CodContenedor.Trim(),
            FecIngreso = ingreso.FecIngreso,
        });

        _dbContext.SaveChanges();
    }

    public void Update(TrIngresoContenedor ingreso)
    {
        var existing = _dbContext.IngresosContenedor
            .FirstOrDefault(x => x.NumIngreso == ingreso.NumIngreso);

        if (existing is null)
        {
            throw new InvalidOperationException("No se encontró el ingreso a actualizar.");
        }

        existing.CodContenedor = ingreso.CodContenedor.Trim();
        existing.FecIngreso = ingreso.FecIngreso;

        _dbContext.SaveChanges();
    }
}
