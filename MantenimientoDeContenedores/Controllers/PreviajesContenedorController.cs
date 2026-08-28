using Microsoft.AspNetCore.Mvc;
using MantenimientoDeContenedores.Models;
using MantenimientoDeContenedores.Repositories;

namespace MantenimientoDeContenedores.Controllers;

public class PreviajesContenedorController : Controller
{
    private readonly IPreviajeContenedorRepository _previajeContenedorRepository;
    private readonly IIngresoContenedorRepository _ingresoContenedorRepository;
    private readonly IEspecialidadMttoRepository _especialidadMttoRepository;
    private readonly IEspecialidadEmpleadoRepository _especialidadEmpleadoRepository;

    public PreviajesContenedorController(
        IPreviajeContenedorRepository previajeContenedorRepository,
        IIngresoContenedorRepository ingresoContenedorRepository,
        IEspecialidadMttoRepository especialidadMttoRepository,
        IEspecialidadEmpleadoRepository especialidadEmpleadoRepository)
    {
        _previajeContenedorRepository = previajeContenedorRepository;
        _ingresoContenedorRepository = ingresoContenedorRepository;
        _especialidadMttoRepository = especialidadMttoRepository;
        _especialidadEmpleadoRepository = especialidadEmpleadoRepository;
    }

    public IActionResult Index()
    {
        var previajes = _previajeContenedorRepository.GetAll();
        return View(previajes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(BuildFormViewModel(new PreviajeContenedorFormViewModel
        {
            FecPreviaje = DateTime.Now,
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PreviajeContenedorFormViewModel previaje)
    {
        ValidatePreviaje(previaje);

        if (!ModelState.IsValid)
        {
            return View(BuildFormViewModel(previaje));
        }

        _previajeContenedorRepository.Add(MapToEntity(previaje));
        TempData["SuccessMessage"] = "Previaje registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var previaje = _previajeContenedorRepository.GetById(id);

        if (previaje is null)
        {
            return NotFound();
        }

        return View(BuildFormViewModel(new PreviajeContenedorFormViewModel
        {
            NroPreviaje = previaje.NroPreviaje,
            NumIngreso = previaje.NumIngreso,
            CodEspMtto = previaje.CodEspMtto,
            CodTit = previaje.CodTit,
            FecPreviaje = previaje.FecPreviaje,
            Observaciones = previaje.Observaciones,
            Habilitado = previaje.Habilitado,
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, PreviajeContenedorFormViewModel previaje)
    {
        if (id != previaje.NroPreviaje)
        {
            return BadRequest();
        }

        if (_previajeContenedorRepository.GetById(id) is null)
        {
            return NotFound();
        }

        ValidatePreviaje(previaje, id);

        if (!ModelState.IsValid)
        {
            return View(BuildFormViewModel(previaje));
        }

        _previajeContenedorRepository.Update(MapToEntity(previaje));
        TempData["SuccessMessage"] = "Previaje actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private PreviajeContenedorFormViewModel BuildFormViewModel(PreviajeContenedorFormViewModel source)
    {
        var especialidades = _especialidadMttoRepository.GetAll();
        var codigosEspecialidad = especialidades
            .Select(x => x.CodEspMtto)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var previajesRegistrados = _previajeContenedorRepository.GetAll();
        var especialidadesPorIngreso = previajesRegistrados
            .GroupBy(x => x.NumIngreso)
            .ToDictionary(
                x => x.Key,
                x => x.Select(y => y.CodEspMtto).ToHashSet(StringComparer.OrdinalIgnoreCase));

        var ingresos = _ingresoContenedorRepository.GetAll()
            .Where(x =>
            {
                var especialidadesRegistradas = especialidadesPorIngreso.GetValueOrDefault(x.NumIngreso) ?? [];
                var tienePendientes = codigosEspecialidad.Count == 0 || especialidadesRegistradas.Count < codigosEspecialidad.Count;

                return tienePendientes || source.NumIngreso == x.NumIngreso;
            })
            .Select(x => new IngresoContenedorLookupItem
            {
                NumIngreso = x.NumIngreso,
                CodContenedor = x.CodContenedor,
                NombreContenedor = x.Contenedor?.Nombre ?? string.Empty,
                CodCliente = x.Contenedor?.CodCliente ?? string.Empty,
                NombreCliente = x.Contenedor?.Cliente?.NombreCliente ?? string.Empty,
                CodigosEspecialidadesRegistradas = string.Join(",", (especialidadesPorIngreso.GetValueOrDefault(x.NumIngreso) ?? []).OrderBy(y => y)),
            })
            .ToList();

        var especialidadesRegistradasDelIngreso = source.NumIngreso.HasValue
            ? especialidadesPorIngreso.GetValueOrDefault(source.NumIngreso.Value) ?? []
            : [];

        var especialidadesDisponibles = especialidades
            .Where(x =>
                !especialidadesRegistradasDelIngreso.Contains(x.CodEspMtto)
                || string.Equals(source.CodEspMtto, x.CodEspMtto, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var tecnicos = _especialidadEmpleadoRepository.GetAll()
            .Select(x => new TecnicoEspecialidadLookupItem
            {
                CodTit = x.CodTit,
                NomTit = x.NomTit,
                Usuario = x.Usuario,
                CodEspMtto = x.EspecialidadesAsignadas.Select(y => y.CodEspMtto).FirstOrDefault() ?? string.Empty,
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.CodEspMtto))
            .OrderBy(x => x.NomTit)
            .ToList();

        return new PreviajeContenedorFormViewModel
        {
            NroPreviaje = source.NroPreviaje,
            NumIngreso = source.NumIngreso,
            CodEspMtto = source.CodEspMtto,
            CodTit = source.CodTit,
            FecPreviaje = source.FecPreviaje,
            Observaciones = source.Observaciones,
            Habilitado = source.Habilitado,
            IngresosDisponibles = ingresos,
            EspecialidadesDisponibles = especialidadesDisponibles,
            TecnicosDisponibles = tecnicos,
        };
    }

    private void ValidatePreviaje(PreviajeContenedorFormViewModel previaje, int? excludeNroPreviaje = null)
    {
        var ingreso = previaje.NumIngreso.HasValue ? _ingresoContenedorRepository.GetById(previaje.NumIngreso.Value) : null;
        var especialidad = _especialidadMttoRepository.GetByCode(previaje.CodEspMtto);
        var tecnico = _especialidadEmpleadoRepository.GetByCode(previaje.CodTit);

        if (ingreso is null)
        {
            ModelState.AddModelError(nameof(previaje.NumIngreso), "Debes seleccionar un ingreso válido.");
        }

        if (especialidad is null)
        {
            ModelState.AddModelError(nameof(previaje.CodEspMtto), "Debes seleccionar una especialidad válida.");
        }

        if (tecnico is null)
        {
            ModelState.AddModelError(nameof(previaje.CodTit), "Debes seleccionar un técnico válido.");
        }
        else
        {
            var especialidadTecnico = tecnico.EspecialidadesAsignadas.Select(x => x.CodEspMtto).FirstOrDefault();

            if (!string.Equals(especialidadTecnico, previaje.CodEspMtto, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(previaje.CodTit), "El técnico seleccionado no corresponde a la especialidad elegida.");
            }
        }

        if (previaje.NumIngreso.HasValue
            && !string.IsNullOrWhiteSpace(previaje.CodEspMtto)
            && _previajeContenedorRepository.ExistsForIngresoEspecialidad(previaje.NumIngreso.Value, previaje.CodEspMtto, excludeNroPreviaje))
        {
            ModelState.AddModelError(nameof(previaje.CodEspMtto), "Ya existe un previaje registrado para ese ingreso y especialidad.");
        }
    }

    private static TrPreviajeContenedor MapToEntity(PreviajeContenedorFormViewModel previaje)
    {
        return new TrPreviajeContenedor
        {
            NroPreviaje = previaje.NroPreviaje,
            NumIngreso = previaje.NumIngreso ?? 0,
            CodEspMtto = previaje.CodEspMtto,
            CodTit = previaje.CodTit,
            FecPreviaje = previaje.FecPreviaje,
            Observaciones = previaje.Observaciones,
            Habilitado = previaje.Habilitado,
        };
    }
}
