using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class InMemoryEspecialidadEmpleadoRepository : IEspecialidadEmpleadoRepository
{
    private readonly List<CtEspDelEmp> _tecnicos = [];
    private readonly object _lock = new();

    public IReadOnlyCollection<CtEspDelEmp> GetAll()
    {
        lock (_lock)
        {
            return _tecnicos
                .OrderBy(x => x.CodTit)
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public CtEspDelEmp? GetByCode(string codTit)
    {
        lock (_lock)
        {
            var tecnico = _tecnicos.FirstOrDefault(x => string.Equals(x.CodTit, codTit, StringComparison.OrdinalIgnoreCase));
            return tecnico is null ? null : Clone(tecnico);
        }
    }

    public bool Exists(string codTit)
    {
        lock (_lock)
        {
            return _tecnicos.Any(x => string.Equals(x.CodTit, codTit, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void Add(EspecialidadEmpleadoFormViewModel tecnico)
    {
        lock (_lock)
        {
            _tecnicos.Add(new CtEspDelEmp
            {
                CodTit = tecnico.CodTit.Trim(),
                NomTit = tecnico.NomTit.Trim(),
                Usuario = tecnico.Usuario.Trim(),
                EspecialidadesAsignadas = BuildDetalle(tecnico.CodTit, tecnico.CodEspMttoSeleccionado),
            });
        }
    }

    public void Update(EspecialidadEmpleadoFormViewModel tecnico)
    {
        lock (_lock)
        {
            var existing = _tecnicos.FirstOrDefault(x => string.Equals(x.CodTit, tecnico.CodTit, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                throw new InvalidOperationException("No se encontró el técnico a actualizar.");
            }

            existing.NomTit = tecnico.NomTit.Trim();
            existing.Usuario = tecnico.Usuario.Trim();
            existing.EspecialidadesAsignadas = BuildDetalle(tecnico.CodTit, tecnico.CodEspMttoSeleccionado);
        }
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

    private static CtEspDelEmp Clone(CtEspDelEmp tecnico)
    {
        return new CtEspDelEmp
        {
            CodTit = tecnico.CodTit,
            NomTit = tecnico.NomTit,
            Usuario = tecnico.Usuario,
            EspecialidadesAsignadas = tecnico.EspecialidadesAsignadas
                .Select(x => new CppEspDelEmp
                {
                    CodTit = x.CodTit,
                    CodEspMtto = x.CodEspMtto,
                })
                .ToList(),
        };
    }
}
