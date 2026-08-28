using System.ComponentModel.DataAnnotations;

namespace MantenimientoDeContenedores.Models;

public class CtMContenedor
{
    [Required(ErrorMessage = "El código de contenedor es obligatorio.")]
    [StringLength(20, ErrorMessage = "El código de contenedor no puede exceder 20 caracteres.")]
    [Display(Name = "Código de contenedor")]
    public string CodContenedor { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código de cliente es obligatorio.")]
    [StringLength(20, ErrorMessage = "El código de cliente no puede exceder 20 caracteres.")]
    [Display(Name = "Código de cliente")]
    public string CodCliente { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    public CtClienteMtto? Cliente { get; set; }
}
