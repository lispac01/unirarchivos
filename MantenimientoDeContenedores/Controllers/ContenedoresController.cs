using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MantenimientoDeContenedores.Models;
using MantenimientoDeContenedores.Repositories;

namespace MantenimientoDeContenedores.Controllers;

public class ContenedoresController : Controller
{
    private readonly IContenedorRepository _contenedorRepository;
    private readonly IClienteMttoRepository _clienteMttoRepository;

    public ContenedoresController(IContenedorRepository contenedorRepository, IClienteMttoRepository clienteMttoRepository)
    {
        _contenedorRepository = contenedorRepository;
        _clienteMttoRepository = clienteMttoRepository;
    }

    public IActionResult Index()
    {
        var contenedores = _contenedorRepository.GetAll();
        return View(contenedores);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadClientes();
        return View(new CtMContenedor { Activo = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CtMContenedor contenedor)
    {
        var cliente = _clienteMttoRepository.GetByCode(contenedor.CodCliente);

        if (_contenedorRepository.Exists(contenedor.CodContenedor))
        {
            ModelState.AddModelError(nameof(contenedor.CodContenedor), "Ya existe un contenedor con ese código.");
        }

        if (cliente is null)
        {
            ModelState.AddModelError(nameof(contenedor.CodCliente), "Debes seleccionar un cliente válido.");
        }
        else if (!cliente.Activo)
        {
            ModelState.AddModelError(nameof(contenedor.CodCliente), "Solo puedes asociar contenedores a clientes activos.");
        }

        if (!ModelState.IsValid)
        {
            LoadClientes(contenedor.CodCliente);
            return View(contenedor);
        }

        _contenedorRepository.Add(contenedor);
        TempData["SuccessMessage"] = "Contenedor creado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(string id)
    {
        var contenedor = _contenedorRepository.GetByCode(id);

        if (contenedor is null)
        {
            return NotFound();
        }

        LoadClientes(contenedor.CodCliente);
        return View(contenedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, CtMContenedor contenedor)
    {
        if (!string.Equals(id, contenedor.CodContenedor, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        var existingContenedor = _contenedorRepository.GetByCode(id);

        if (existingContenedor is null)
        {
            return NotFound();
        }

        var cliente = _clienteMttoRepository.GetByCode(contenedor.CodCliente);

        if (cliente is null)
        {
            ModelState.AddModelError(nameof(contenedor.CodCliente), "Debes seleccionar un cliente válido.");
        }
        else if (!cliente.Activo && !string.Equals(existingContenedor.CodCliente, cliente.CodCliente, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(contenedor.CodCliente), "Solo puedes asociar contenedores a clientes activos.");
        }

        if (!ModelState.IsValid)
        {
            LoadClientes(contenedor.CodCliente);
            return View(contenedor);
        }

        _contenedorRepository.Update(contenedor);
        TempData["SuccessMessage"] = "Contenedor actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private void LoadClientes(string? selectedCodCliente = null)
    {
        var clientes = _clienteMttoRepository.GetActive().ToList();

        if (!string.IsNullOrWhiteSpace(selectedCodCliente)
            && clientes.All(x => !string.Equals(x.CodCliente, selectedCodCliente, StringComparison.OrdinalIgnoreCase)))
        {
            var selectedCliente = _clienteMttoRepository.GetByCode(selectedCodCliente);

            if (selectedCliente is not null)
            {
                clientes.Add(selectedCliente);
            }
        }

        ViewBag.Clientes = clientes.Select(cliente => new SelectListItem
        {
            Value = cliente.CodCliente,
            Text = $"{cliente.CodCliente} - {cliente.NombreCliente}",
            Selected = string.Equals(cliente.CodCliente, selectedCodCliente, StringComparison.OrdinalIgnoreCase),
        })
        .OrderBy(x => x.Value)
        .ToList();

        ViewBag.HasClientesActivos = clientes.Count > 0;
    }
}
