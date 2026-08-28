using System.ComponentModel.DataAnnotations;

namespace MantenimientoDeContenedores.Models;

public class CtEspMtto
{
    [Required(ErrorMessage = "El código de especialidad es obligatorio.")]
    [StringLength(20, ErrorMessage = "El código de especialidad no puede exceder 20 caracteres.")]
    [Display(Name = "Código de especialidad")]
    public string CodEspMtto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de especialidad es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre de especialidad no puede exceder 150 caracteres.")]
    [Display(Name = "Nombre de especialidad")]
    public string NomEspMtto { get; set; } = string.Empty;
}
