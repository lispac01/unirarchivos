using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public interface IIngresoContenedorRepository
{
    IReadOnlyCollection<TrIngresoContenedor> GetAll();
    TrIngresoContenedor? GetById(int numIngreso);
    void Add(TrIngresoContenedor ingreso);
    void Update(TrIngresoContenedor ingreso);
}
