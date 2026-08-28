using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public interface IClienteMttoRepository
{
    IReadOnlyCollection<CtClienteMtto> GetAll();
    IReadOnlyCollection<CtClienteMtto> GetActive();
    bool Exists(string codCliente);
    CtClienteMtto? GetByCode(string codCliente);
    void Add(CtClienteMtto cliente);
    void Update(CtClienteMtto cliente);
}
