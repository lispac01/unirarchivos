using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public interface IPreviajeContenedorRepository
{
    IReadOnlyCollection<TrPreviajeContenedor> GetAll();
    TrPreviajeContenedor? GetById(int nroPreviaje);
    bool ExistsForIngresoEspecialidad(int numIngreso, string codEspMtto, int? excludeNroPreviaje = null);
    int Add(TrPreviajeContenedor previaje);
    void Update(TrPreviajeContenedor previaje);
    IReadOnlyCollection<TrPreviajeTareaDetalle> GetTaskDetails(int nroPreviaje);
    void ReplaceTaskDetails(int nroPreviaje, IReadOnlyCollection<TrPreviajeTareaDetalle> detalles);
}
