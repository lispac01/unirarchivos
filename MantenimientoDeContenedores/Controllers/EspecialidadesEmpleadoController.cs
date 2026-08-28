using Microsoft.AspNetCore.Mvc;
using MantenimientoDeContenedores.Models;
using MantenimientoDeContenedores.Repositories;

namespace MantenimientoDeContenedores.Controllers;

public class EspecialidadesEmpleadoController : Controller
{
    private readonly IEspecialidadEmpleadoRepository _especialidadEmpleadoRepository;
    private readonly IEspecialidadMttoRepository _especialidadMttoRepository;

    public EspecialidadesEmpleadoController(
        IEspecialidadEmpleadoRepository especialidadEmpleadoRepository,
        IEspecialidadMttoRepository especialidadMttoRepository)
    {
        _especialidadEmpleadoRepository = especialidadEmpleadoRepository;
        _especialidadMttoRepository = especialidadMttoRepository;
    }

    public IActionResult Index()
    {
        var tecnicos = _especialidadEmpleadoRepository.GetAll();
        return View(tecnicos);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(BuildFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EspecialidadEmpleadoFormViewModel tecnico)
    {
        if (_especialidadEmpleadoRepository.Exists(tecnico.CodTit))
        {
            ModelState.AddModelError(nameof(tecnico.CodTit), "Ya existe un técnico con ese código.");
        }

        ValidateEspecialidadesSeleccionadas(tecnico);

        if (!ModelState.IsValid)
        {
            return View(BuildFormViewModel(tecnico));
        }

        _especialidadEmpleadoRepository.Add(tecnico);
        TempData["SuccessMessage"] = "Técnico creado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(string id)
    {
        var tecnico = _especialidadEmpleadoRepository.GetByCode(id);

        if (tecnico is null)
        {
            return NotFound();
        }

        var model = BuildFormViewModel(new EspecialidadEmpleadoFormViewModel
        {
            CodTit = tecnico.CodTit,
            NomTit = tecnico.NomTit,
            Usuario = tecnico.Usuario,
            CodEspMttoSeleccionado = tecnico.EspecialidadesAsignadas.Select(x => x.CodEspMtto).FirstOrDefault() ?? string.Empty,
        });

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, EspecialidadEmpleadoFormViewModel tecnico)
    {
        if (!string.Equals(id, tecnico.CodTit, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        if (_especialidadEmpleadoRepository.GetByCode(id) is null)
        {
            return NotFound();
        }

        ValidateEspecialidadesSeleccionadas(tecnico);

        if (!ModelState.IsValid)
        {
            return View(BuildFormViewModel(tecnico));
        }

        _especialidadEmpleadoRepository.Update(tecnico);
        TempData["SuccessMessage"] = "Técnico actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private EspecialidadEmpleadoFormViewModel BuildFormViewModel(EspecialidadEmpleadoFormViewModel? source = null)
    {
        return new EspecialidadEmpleadoFormViewModel
        {
            CodTit = source?.CodTit ?? string.Empty,
            NomTit = source?.NomTit ?? string.Empty,
            Usuario = source?.Usuario ?? string.Empty,
            CodEspMttoSeleccionado = source?.CodEspMttoSeleccionado ?? string.Empty,
            EspecialidadesDisponibles = _especialidadMttoRepository.GetAll(),
        };
    }

    private void ValidateEspecialidadesSeleccionadas(EspecialidadEmpleadoFormViewModel tecnico)
    {
        var especialesValidas = _especialidadMttoRepository.GetAll()
            .Select(x => x.CodEspMtto)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(tecnico.CodEspMttoSeleccionado)
            && !especialesValidas.Contains(tecnico.CodEspMttoSeleccionado))
        {
            ModelState.AddModelError(nameof(tecnico.CodEspMttoSeleccionado), "La especialidad seleccionada no existe en el maestro.");
        }
    }
}
