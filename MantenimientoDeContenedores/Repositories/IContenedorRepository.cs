using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public interface IContenedorRepository
{
    IReadOnlyCollection<CtMContenedor> GetAll();
    bool Exists(string codContenedor);
    CtMContenedor? GetByCode(string codContenedor);
    void Add(CtMContenedor contenedor);
    void Update(CtMContenedor contenedor);
}
