using MantenimientoDeContenedores.Models;

namespace MantenimientoDeContenedores.Repositories;

public class InMemoryTareaMttoRepository : ITareaMttoRepository
{
    private readonly List<CtTareaDeMtto> _tareas = [];
    private readonly object _lock = new();

    public IReadOnlyCollection<CtTareaDeMtto> GetAll()
    {
        lock (_lock)
        {
            return _tareas
                .OrderBy(x => x.NroTarea)
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public CtTareaDeMtto? GetById(int nroTarea)
    {
        lock (_lock)
        {
            var tarea = _tareas.FirstOrDefault(x => x.NroTarea == nroTarea);
            return tarea is null ? null : Clone(tarea);
        }
    }

    public bool Exists(int nroTarea)
    {
        lock (_lock)
        {
            return _tareas.Any(x => x.NroTarea == nroTarea);
        }
    }

    public IReadOnlyCollection<CtTareaDeMtto> GetActiveByEspecialidad(string codEspMtto)
    {
        lock (_lock)
        {
            return _tareas
                .Where(x => x.Activo && string.Equals(x.CodEspMtto, codEspMtto, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.NroTarea)
                .Select(Clone)
                .ToList()
                .AsReadOnly();
        }
    }

    public void Add(CtTareaDeMtto tarea)
    {
        lock (_lock)
        {
            _tareas.Add(new CtTareaDeMtto
            {
                NroTarea = tarea.NroTarea,
                NombreTarea = tarea.NombreTarea.Trim(),
                CodEspMtto = tarea.CodEspMtto.Trim(),
                TiempoEstimado = tarea.TiempoEstimado,
                Activo = tarea.Activo,
            });
        }
    }

    public void Update(CtTareaDeMtto tarea)
    {
        lock (_lock)
        {
            var existing = _tareas.FirstOrDefault(x => x.NroTarea == tarea.NroTarea);

            if (existing is null)
            {
                throw new InvalidOperationException("No se encontró la tarea a actualizar.");
            }

            existing.NombreTarea = tarea.NombreTarea.Trim();
            existing.CodEspMtto = tarea.CodEspMtto.Trim();
            existing.TiempoEstimado = tarea.TiempoEstimado;
            existing.Activo = tarea.Activo;
        }
    }

    private static CtTareaDeMtto Clone(CtTareaDeMtto tarea)
    {
        return new CtTareaDeMtto
        {
            NroTarea = tarea.NroTarea,
            NombreTarea = tarea.NombreTarea,
            CodEspMtto = tarea.CodEspMtto,
            TiempoEstimado = tarea.TiempoEstimado,
            Activo = tarea.Activo,
        };
    }
}
