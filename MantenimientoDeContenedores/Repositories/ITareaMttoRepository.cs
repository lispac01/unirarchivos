using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public interface ITareaMttoRepository
{
    IReadOnlyCollection<CtTareaDeMtto> GetAll();
    CtTareaDeMtto? GetById(int nroTarea);
    bool Exists(int nroTarea);
    IReadOnlyCollection<CtTareaDeMtto> GetActiveByEspecialidad(string codEspMtto);
    void Add(CtTareaDeMtto tarea);
    void Update(CtTareaDeMtto tarea);
}
