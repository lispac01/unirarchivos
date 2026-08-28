using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public interface IEspecialidadMttoRepository
{
    IReadOnlyCollection<CtEspMtto> GetAll();
    bool Exists(string codEspMtto);
    CtEspMtto? GetByCode(string codEspMtto);
    void Add(CtEspMtto especialidad);
    void Update(CtEspMtto especialidad);
}
