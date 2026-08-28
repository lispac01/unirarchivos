using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public interface IEspecialidadEmpleadoRepository
{
    IReadOnlyCollection<CtEspDelEmp> GetAll();
    CtEspDelEmp? GetByCode(string codTit);
    bool Exists(string codTit);
    void Add(EspecialidadEmpleadoFormViewModel tecnico);
    void Update(EspecialidadEmpleadoFormViewModel tecnico);
}
