using System.ComponentModel.DataAnnotations;

namespace MantenimientoDeContenedores.Models;

public class TrIngresoContenedor
{
    [Display(Name = "Número de ingreso")]
    public int NumIngreso { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un contenedor.")]
    [StringLength(20, ErrorMessage = "El código de contenedor no puede exceder 20 caracteres.")]
    [Display(Name = "Contenedor")]
    public string CodContenedor { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
    [Display(Name = "Fecha de ingreso")]
    public DateTime FecIngreso { get; set; }

    public CtMContenedor? Contenedor { get; set; }
}
