using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MantenimientoDeContenedores.Models;
using MantenimientoDeContenedores.Repositories;

namespace MantenimientoDeContenedores.Controllers;

public class TareasMttoController : Controller
{
    private readonly ITareaMttoRepository _tareaMttoRepository;
    private readonly IEspecialidadMttoRepository _especialidadMttoRepository;

    public TareasMttoController(ITareaMttoRepository tareaMttoRepository, IEspecialidadMttoRepository especialidadMttoRepository)
    {
        _tareaMttoRepository = tareaMttoRepository;
        _especialidadMttoRepository = especialidadMttoRepository;
    }

    public IActionResult Index()
    {
        var tareas = _tareaMttoRepository.GetAll();
        return View(tareas);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadEspecialidades();
        return View(new CtTareaDeMtto { Activo = true, TiempoEstimado = 0.50m });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CtTareaDeMtto tarea)
    {
        if (_tareaMttoRepository.Exists(tarea.NroTarea))
        {
            ModelState.AddModelError(nameof(tarea.NroTarea), "Ya existe una tarea con ese número.");
        }

        ValidateEspecialidad(tarea.CodEspMtto);

        if (!ModelState.IsValid)
        {
            LoadEspecialidades(tarea.CodEspMtto);
            return View(tarea);
        }

        _tareaMttoRepository.Add(tarea);
        TempData["SuccessMessage"] = "Tarea creada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var tarea = _tareaMttoRepository.GetById(id);

        if (tarea is null)
        {
            return NotFound();
        }

        LoadEspecialidades(tarea.CodEspMtto);
        return View(tarea);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, CtTareaDeMtto tarea)
    {
        if (id != tarea.NroTarea)
        {
            return BadRequest();
        }

        if (_tareaMttoRepository.GetById(id) is null)
        {
            return NotFound();
        }

        ValidateEspecialidad(tarea.CodEspMtto);

        if (!ModelState.IsValid)
        {
            LoadEspecialidades(tarea.CodEspMtto);
            return View(tarea);
        }

        _tareaMttoRepository.Update(tarea);
        TempData["SuccessMessage"] = "Tarea actualizada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private void ValidateEspecialidad(string codEspMtto)
    {
        if (_especialidadMttoRepository.GetByCode(codEspMtto) is null)
        {
            ModelState.AddModelError(nameof(CtTareaDeMtto.CodEspMtto), "Debes seleccionar una especialidad válida.");
        }
    }

    private void LoadEspecialidades(string? selectedCodEspMtto = null)
    {
        var especialidades = _especialidadMttoRepository.GetAll();

        ViewBag.Especialidades = especialidades.Select(x => new SelectListItem
        {
            Value = x.CodEspMtto,
            Text = $"{x.CodEspMtto} - {x.NomEspMtto}",
            Selected = string.Equals(x.CodEspMtto, selectedCodEspMtto, StringComparison.OrdinalIgnoreCase),
        }).ToList();

        ViewBag.HasEspecialidades = especialidades.Count > 0;
    }
}
