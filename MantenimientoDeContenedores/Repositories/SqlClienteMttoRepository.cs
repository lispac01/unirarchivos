using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Data;
using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class SqlClienteMttoRepository : IClienteMttoRepository
{
    private readonly MantenimientoDbContext _dbContext;

    public SqlClienteMttoRepository(MantenimientoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<CtClienteMtto> GetAll()
    {
        return _dbContext.ClientesMtto
            .AsNoTracking()
            .OrderBy(x => x.CodCliente)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyCollection<CtClienteMtto> GetActive()
    {
        return _dbContext.ClientesMtto
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.CodCliente)
            .ToList()
            .AsReadOnly();
    }

    public bool Exists(string codCliente)
    {
        var normalized = (codCliente ?? string.Empty).Trim().ToUpper();

        return _dbContext.ClientesMtto
            .Any(x => x.CodCliente.ToUpper() == normalized);
    }

    public CtClienteMtto? GetByCode(string codCliente)
    {
        var normalized = (codCliente ?? string.Empty).Trim().ToUpper();

        return _dbContext.ClientesMtto
            .AsNoTracking()
            .FirstOrDefault(x => x.CodCliente.ToUpper() == normalized);
    }

    public void Add(CtClienteMtto cliente)
    {
        _dbContext.ClientesMtto.Add(new CtClienteMtto
        {
            CodCliente = cliente.CodCliente.Trim(),
            NombreCliente = cliente.NombreCliente.Trim(),
            CodDpto = cliente.CodDpto.Trim(),
            ImpMovMo = cliente.ImpMovMo,
            ImpMovMo2 = cliente.ImpMovMo2,
            Activo = cliente.Activo,
        });

        _dbContext.SaveChanges();
    }

    public void Update(CtClienteMtto cliente)
    {
        var normalized = (cliente.CodCliente ?? string.Empty).Trim().ToUpper();
        var existing = _dbContext.ClientesMtto
            .FirstOrDefault(x => x.CodCliente.ToUpper() == normalized);

        if (existing is null)
        {
            throw new InvalidOperationException("No se encontró el cliente a actualizar.");
        }

        existing.NombreCliente = cliente.NombreCliente.Trim();
        existing.CodDpto = cliente.CodDpto.Trim();
        existing.ImpMovMo = cliente.ImpMovMo;
        existing.ImpMovMo2 = cliente.ImpMovMo2;
        existing.Activo = cliente.Activo;

        _dbContext.SaveChanges();
    }
}
