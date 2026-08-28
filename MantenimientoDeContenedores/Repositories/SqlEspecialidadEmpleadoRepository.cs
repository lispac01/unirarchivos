using Microsoft.EntityFrameworkCore;
using MantenimientoDeContenedores.Data;
using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class SqlEspecialidadEmpleadoRepository : IEspecialidadEmpleadoRepository
{
    private readonly MantenimientoDbContext _dbContext;

    public SqlEspecialidadEmpleadoRepository(MantenimientoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<CtEspDelEmp> GetAll()
    {
        return _dbContext.EspecialidadesEmpleado
            .AsNoTracking()
            .Include(x => x.EspecialidadesAsignadas)
            .ThenInclude(x => x.Especialidad)
            .OrderBy(x => x.CodTit)
            .ToList()
            .AsReadOnly();
    }

    public CtEspDelEmp? GetByCode(string codTit)
    {
        var normalized = Normalize(codTit);

        return _dbContext.EspecialidadesEmpleado
            .AsNoTracking()
            .Include(x => x.EspecialidadesAsignadas)
            .ThenInclude(x => x.Especialidad)
            .FirstOrDefault(x => x.CodTit.ToUpper() == normalized);
    }

    public bool Exists(string codTit)
    {
        var normalized = Normalize(codTit);

        return _dbContext.EspecialidadesEmpleado
            .Any(x => x.CodTit.ToUpper() == normalized);
    }

    public void Add(EspecialidadEmpleadoFormViewModel tecnico)
    {
        var entity = new CtEspDelEmp
        {
            CodTit = tecnico.CodTit.Trim(),
            NomTit = tecnico.NomTit.Trim(),
            Usuario = tecnico.Usuario.Trim(),
            EspecialidadesAsignadas = BuildDetalle(tecnico.CodTit, tecnico.CodEspMttoSeleccionado),
        };

        _dbContext.EspecialidadesEmpleado.Add(entity);
        _dbContext.SaveChanges();
    }

    public void Update(EspecialidadEmpleadoFormViewModel tecnico)
    {
        var normalized = Normalize(tecnico.CodTit);
        var existing = _dbContext.EspecialidadesEmpleado
            .Include(x => x.EspecialidadesAsignadas)
            .FirstOrDefault(x => x.CodTit.ToUpper() == normalized);

        if (existing is null)
        {
            throw new InvalidOperationException("No se encontró el técnico a actualizar.");
        }

        existing.NomTit = tecnico.NomTit.Trim();
        existing.Usuario = tecnico.Usuario.Trim();

        _dbContext.CppEspecialidadesEmpleado.RemoveRange(existing.EspecialidadesAsignadas);
        existing.EspecialidadesAsignadas = BuildDetalle(existing.CodTit, tecnico.CodEspMttoSeleccionado);

        _dbContext.SaveChanges();
    }

    private static string Normalize(string value)
    {
        return (value ?? string.Empty).Trim().ToUpper();
    }

    private static List<CppEspDelEmp> BuildDetalle(string codTit, string? codEspMtto)
    {
        if (string.IsNullOrWhiteSpace(codEspMtto))
        {
            return [];
        }

        return
        [
            new CppEspDelEmp
            {
                CodTit = codTit.Trim(),
                CodEspMtto = codEspMtto.Trim(),
            }
        ];
    }
}
