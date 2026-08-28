using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Data;
using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class SqlPreviajeContenedorRepository : IPreviajeContenedorRepository
{
    private readonly MantenimientoDbContext _dbContext;

    public SqlPreviajeContenedorRepository(MantenimientoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<TrPreviajeContenedor> GetAll()
    {
        return _dbContext.PreviajesContenedor
            .AsNoTracking()
            .Include(x => x.Ingreso)
                .ThenInclude(x => x!.Contenedor)
                    .ThenInclude(x => x!.Cliente)
            .Include(x => x.Especialidad)
            .Include(x => x.Tecnico)
            .OrderByDescending(x => x.NroPreviaje)
            .ToList()
            .AsReadOnly();
    }

    public TrPreviajeContenedor? GetById(int nroPreviaje)
    {
        return _dbContext.PreviajesContenedor
            .AsNoTracking()
            .Include(x => x.Ingreso)
                .ThenInclude(x => x!.Contenedor)
                    .ThenInclude(x => x!.Cliente)
            .Include(x => x.Especialidad)
            .Include(x => x.Tecnico)
            .FirstOrDefault(x => x.NroPreviaje == nroPreviaje);
    }

    public bool ExistsForIngresoEspecialidad(int numIngreso, string codEspMtto, int? excludeNroPreviaje = null)
    {
        var normalized = Normalize(codEspMtto);

        return _dbContext.PreviajesContenedor.Any(x =>
            x.NumIngreso == numIngreso
            && x.CodEspMtto.ToUpper() == normalized
            && (!excludeNroPreviaje.HasValue || x.NroPreviaje != excludeNroPreviaje.Value));
    }

    public int Add(TrPreviajeContenedor previaje)
    {
        var entity = new TrPreviajeContenedor
        {
            NumIngreso = previaje.NumIngreso,
            CodEspMtto = previaje.CodEspMtto.Trim(),
            CodTit = previaje.CodTit.Trim(),
            FecPreviaje = previaje.FecPreviaje,
            Observaciones = previaje.Observaciones.Trim(),
            Habilitado = previaje.Habilitado,
        };

        _dbContext.PreviajesContenedor.Add(entity);
        _dbContext.SaveChanges();

        return entity.NroPreviaje;
    }

    public void Update(TrPreviajeContenedor previaje)
    {
        var existing = _dbContext.PreviajesContenedor.FirstOrDefault(x => x.NroPreviaje == previaje.NroPreviaje);

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

        _dbContext.SaveChanges();
    }

    public IReadOnlyCollection<TrPreviajeTareaDetalle> GetTaskDetails(int nroPreviaje)
    {
        return Array.Empty<TrPreviajeTareaDetalle>();
    }

    public void ReplaceTaskDetails(int nroPreviaje, IReadOnlyCollection<TrPreviajeTareaDetalle> detalles)
    {
    }

    private static string Normalize(string value)
    {
        return (value ?? string.Empty).Trim().ToUpper();
    }
}
