using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MantenimientoDeContenedores.Models;
using MantenimientoDeContenedores.Repositories;

namespace MantenimientoDeContenedores.Controllers;

public class IngresosContenedorController : Controller
{
    private readonly IIngresoContenedorRepository _ingresoContenedorRepository;
    private readonly IContenedorRepository _contenedorRepository;

    public IngresosContenedorController(
        IIngresoContenedorRepository ingresoContenedorRepository,
        IContenedorRepository contenedorRepository)
    {
        _ingresoContenedorRepository = ingresoContenedorRepository;
        _contenedorRepository = contenedorRepository;
    }

    public IActionResult Index()
    {
        var ingresos = _ingresoContenedorRepository.GetAll();
        return View(ingresos);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadContenedores();
        return View(new TrIngresoContenedor
        {
            FecIngreso = DateTime.Now,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TrIngresoContenedor ingreso)
    {
        ValidateContenedor(ingreso.CodContenedor);

        if (!ModelState.IsValid)
        {
            LoadContenedores(ingreso.CodContenedor);
            return View(ingreso);
        }

        _ingresoContenedorRepository.Add(ingreso);
        TempData["SuccessMessage"] = "Ingreso registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var ingreso = _ingresoContenedorRepository.GetById(id);

        if (ingreso is null)
        {
            return NotFound();
        }

        LoadContenedores(ingreso.CodContenedor);
        return View(ingreso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TrIngresoContenedor ingreso)
    {
        if (id != ingreso.NumIngreso)
        {
            return BadRequest();
        }

        if (_ingresoContenedorRepository.GetById(id) is null)
        {
            return NotFound();
        }

        ValidateContenedor(ingreso.CodContenedor);

        if (!ModelState.IsValid)
        {
            LoadContenedores(ingreso.CodContenedor);
            return View(ingreso);
        }

        _ingresoContenedorRepository.Update(ingreso);
        TempData["SuccessMessage"] = "Ingreso actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private void ValidateContenedor(string codContenedor)
    {
        var contenedor = _contenedorRepository.GetByCode(codContenedor);

        if (contenedor is null)
        {
            ModelState.AddModelError(nameof(TrIngresoContenedor.CodContenedor), "Debes seleccionar un contenedor válido.");
        }
    }

    private void LoadContenedores(string? selectedCodContenedor = null)
    {
        var contenedores = _contenedorRepository.GetAll()
            .Where(x => x.Activo)
            .ToList();

        if (!string.IsNullOrWhiteSpace(selectedCodContenedor)
            && contenedores.All(x => !string.Equals(x.CodContenedor, selectedCodContenedor, StringComparison.OrdinalIgnoreCase)))
        {
            var selected = _contenedorRepository.GetByCode(selectedCodContenedor);

            if (selected is not null)
            {
                contenedores.Add(selected);
            }
        }

        ViewBag.Contenedores = contenedores
            .OrderBy(x => x.CodContenedor)
            .Select(x => new SelectListItem
            {
                Value = x.CodContenedor,
                Text = $"{x.CodContenedor} - {x.Nombre}",
                Selected = string.Equals(x.CodContenedor, selectedCodContenedor, StringComparison.OrdinalIgnoreCase),
            })
            .ToList();

        ViewBag.HasContenedores = contenedores.Count > 0;
    }
}
