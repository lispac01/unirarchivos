using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Data;
using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class SqlTareaMttoRepository : ITareaMttoRepository
{
    private readonly MantenimientoDbContext _dbContext;

    public SqlTareaMttoRepository(MantenimientoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<CtTareaDeMtto> GetAll()
    {
        return _dbContext.TareasMtto
            .AsNoTracking()
            .Include(x => x.Especialidad)
            .OrderBy(x => x.NroTarea)
            .ToList()
            .AsReadOnly();
    }

    public CtTareaDeMtto? GetById(int nroTarea)
    {
        return _dbContext.TareasMtto
            .AsNoTracking()
            .Include(x => x.Especialidad)
            .FirstOrDefault(x => x.NroTarea == nroTarea);
    }

    public bool Exists(int nroTarea)
    {
        return _dbContext.TareasMtto.Any(x => x.NroTarea == nroTarea);
    }

    public IReadOnlyCollection<CtTareaDeMtto> GetActiveByEspecialidad(string codEspMtto)
    {
        return _dbContext.TareasMtto
            .AsNoTracking()
            .Include(x => x.Especialidad)
            .Where(x => x.Activo && x.CodEspMtto == codEspMtto)
            .OrderBy(x => x.NroTarea)
            .ToList()
            .AsReadOnly();
    }

    public void Add(CtTareaDeMtto tarea)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.ct_tareademtto ON");
        _dbContext.Database.ExecuteSqlInterpolated($@"
INSERT INTO dbo.ct_tareademtto (nro_tarea, nombre_tarea, cod_esp_mtto, tiempo_estimado, activo)
VALUES ({tarea.NroTarea}, {tarea.NombreTarea.Trim()}, {tarea.CodEspMtto.Trim()}, {tarea.TiempoEstimado}, {tarea.Activo})");
        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.ct_tareademtto OFF");

        transaction.Commit();
    }

    public void Update(CtTareaDeMtto tarea)
    {
        var existing = _dbContext.TareasMtto.FirstOrDefault(x => x.NroTarea == tarea.NroTarea);

        if (existing is null)
        {
            throw new InvalidOperationException("No se encontró la tarea a actualizar.");
        }

        existing.NombreTarea = tarea.NombreTarea.Trim();
        existing.CodEspMtto = tarea.CodEspMtto.Trim();
        existing.TiempoEstimado = tarea.TiempoEstimado;
        existing.Activo = tarea.Activo;

        _dbContext.SaveChanges();
    }
}
