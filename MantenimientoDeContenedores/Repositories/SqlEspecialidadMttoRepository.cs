using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Data;
using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class SqlEspecialidadMttoRepository : IEspecialidadMttoRepository
{
    private readonly MantenimientoDbContext _dbContext;

    public SqlEspecialidadMttoRepository(MantenimientoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<CtEspMtto> GetAll()
    {
        return _dbContext.EspecialidadesMtto
            .AsNoTracking()
            .OrderBy(x => x.CodEspMtto)
            .ToList()
            .AsReadOnly();
    }

    public bool Exists(string codEspMtto)
    {
        var normalized = (codEspMtto ?? string.Empty).Trim().ToUpper();

        return _dbContext.EspecialidadesMtto
            .Any(x => x.CodEspMtto.ToUpper() == normalized);
    }

    public CtEspMtto? GetByCode(string codEspMtto)
    {
        var normalized = (codEspMtto ?? string.Empty).Trim().ToUpper();

        return _dbContext.EspecialidadesMtto
            .AsNoTracking()
            .FirstOrDefault(x => x.CodEspMtto.ToUpper() == normalized);
    }

    public void Add(CtEspMtto especialidad)
    {
        _dbContext.EspecialidadesMtto.Add(new CtEspMtto
        {
            CodEspMtto = especialidad.CodEspMtto.Trim(),
            NomEspMtto = especialidad.NomEspMtto.Trim(),
        });

        _dbContext.SaveChanges();
    }

    public void Update(CtEspMtto especialidad)
    {
        var normalized = (especialidad.CodEspMtto ?? string.Empty).Trim().ToUpper();
        var existing = _dbContext.EspecialidadesMtto
            .FirstOrDefault(x => x.CodEspMtto.ToUpper() == normalized);

        if (existing is null)
        {
            throw new InvalidOperationException("No se encontró la especialidad a actualizar.");
        }

        existing.NomEspMtto = especialidad.NomEspMtto.Trim();

        _dbContext.SaveChanges();
    }
}
