using Microsoft.AspNetCore.Mvc;
using MantenimientoDeContenedores.Models;
using MantenimientoDeContenedores.Repositories;

namespace MantenimientoDeContenedores.Controllers;

public class EspecialidadesMttoController : Controller
{
    private readonly IEspecialidadMttoRepository _especialidadMttoRepository;

    public EspecialidadesMttoController(IEspecialidadMttoRepository especialidadMttoRepository)
    {
        _especialidadMttoRepository = especialidadMttoRepository;
    }

    public IActionResult Index()
    {
        var especialidades = _especialidadMttoRepository.GetAll();
        return View(especialidades);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CtEspMtto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CtEspMtto especialidad)
    {
        if (_especialidadMttoRepository.Exists(especialidad.CodEspMtto))
        {
            ModelState.AddModelError(nameof(especialidad.CodEspMtto), "Ya existe una especialidad con ese código.");
        }

        if (!ModelState.IsValid)
        {
            return View(especialidad);
        }

        _especialidadMttoRepository.Add(especialidad);
        TempData["SuccessMessage"] = "Especialidad creada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(string id)
    {
        var especialidad = _especialidadMttoRepository.GetByCode(id);

        if (especialidad is null)
        {
            return NotFound();
        }

        return View(especialidad);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, CtEspMtto especialidad)
    {
        if (!string.Equals(id, especialidad.CodEspMtto, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        if (_especialidadMttoRepository.GetByCode(id) is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(especialidad);
        }

        _especialidadMttoRepository.Update(especialidad);
        TempData["SuccessMessage"] = "Especialidad actualizada correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
