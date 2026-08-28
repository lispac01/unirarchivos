using System.ComponentModel.DataAnnotations;

namespace MantenimientoDeContenedores.Models;

public class CtClienteMtto
{
    [Required(ErrorMessage = "El código de cliente es obligatorio.")]
    [StringLength(20, ErrorMessage = "El código de cliente no puede exceder 20 caracteres.")]
    [Display(Name = "Código de cliente")]
    public string CodCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre del cliente no puede exceder 150 caracteres.")]
    [Display(Name = "Nombre del cliente")]
    public string NombreCliente { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El centro de costo no puede exceder 20 caracteres.")]
    [Display(Name = "Centro de costo")]
    public string CodDpto { get; set; } = string.Empty;

    [Range(0, 999999999.99, ErrorMessage = "La mano de obra sin garantía debe ser mayor o igual a 0.")]
    [Display(Name = "Mano de obra sin garantía")]
    public decimal ImpMovMo { get; set; }

    [Range(0, 999999999.99, ErrorMessage = "La mano de obra con garantía debe ser mayor o igual a 0.")]
    [Display(Name = "Mano de obra con garantía")]
    public decimal ImpMovMo2 { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
