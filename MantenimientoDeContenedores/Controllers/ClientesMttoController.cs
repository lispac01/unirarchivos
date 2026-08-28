using Microsoft.AspNetCore.Mvc;
using MantenimientoDeContenedores.Models;
using MantenimientoDeContenedores.Repositories;

namespace MantenimientoDeContenedores.Controllers;

public class ClientesMttoController : Controller
{
    private readonly IClienteMttoRepository _clienteMttoRepository;

    public ClientesMttoController(IClienteMttoRepository clienteMttoRepository)
    {
        _clienteMttoRepository = clienteMttoRepository;
    }

    public IActionResult Index()
    {
        var clientes = _clienteMttoRepository.GetAll();
        return View(clientes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CtClienteMtto { Activo = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CtClienteMtto cliente)
    {
        if (_clienteMttoRepository.Exists(cliente.CodCliente))
        {
            ModelState.AddModelError(nameof(cliente.CodCliente), "Ya existe un cliente con ese código.");
        }

        if (!ModelState.IsValid)
        {
            return View(cliente);
        }

        _clienteMttoRepository.Add(cliente);
        TempData["SuccessMessage"] = "Cliente creado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(string id)
    {
        var cliente = _clienteMttoRepository.GetByCode(id);

        if (cliente is null)
        {
            return NotFound();
        }

        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, CtClienteMtto cliente)
    {
        if (!string.Equals(id, cliente.CodCliente, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        if (_clienteMttoRepository.GetByCode(id) is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(cliente);
        }

        _clienteMttoRepository.Update(cliente);
        TempData["SuccessMessage"] = "Cliente actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
