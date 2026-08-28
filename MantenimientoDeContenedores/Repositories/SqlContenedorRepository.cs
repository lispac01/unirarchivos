using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Data;
using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class SqlContenedorRepository : IContenedorRepository
{
    private readonly MantenimientoDbContext _dbContext;

    public SqlContenedorRepository(MantenimientoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<CtMContenedor> GetAll()
    {
        return _dbContext.Contenedores
            .AsNoTracking()
            .OrderBy(x => x.CodContenedor)
            .ToList()
            .AsReadOnly();
    }

    public bool Exists(string codContenedor)
    {
        var normalized = (codContenedor ?? string.Empty).Trim().ToUpper();

        return _dbContext.Contenedores
            .Any(x => x.CodContenedor.ToUpper() == normalized);
    }

    public CtMContenedor? GetByCode(string codContenedor)
    {
        var normalized = (codContenedor ?? string.Empty).Trim().ToUpper();

        return _dbContext.Contenedores
            .AsNoTracking()
            .FirstOrDefault(x => x.CodContenedor.ToUpper() == normalized);
    }

    public void Add(CtMContenedor contenedor)
    {
        _dbContext.Contenedores.Add(new CtMContenedor
        {
            CodContenedor = contenedor.CodContenedor.Trim(),
            Nombre = contenedor.Nombre.Trim(),
            CodCliente = contenedor.CodCliente.Trim(),
            Activo = contenedor.Activo,
        });

        _dbContext.SaveChanges();
    }

    public void Update(CtMContenedor contenedor)
    {
        var normalized = (contenedor.CodContenedor ?? string.Empty).Trim().ToUpper();
        var existing = _dbContext.Contenedores
            .FirstOrDefault(x => x.CodContenedor.ToUpper() == normalized);

        if (existing is null)
        {
            throw new InvalidOperationException("No se encontró el contenedor a actualizar.");
        }

        existing.Nombre = contenedor.Nombre.Trim();
        existing.CodCliente = contenedor.CodCliente.Trim();
        existing.Activo = contenedor.Activo;

        _dbContext.SaveChanges();
    }
}
