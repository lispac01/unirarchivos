using System.ComponentModel.DataAnnotations;

namespace MantenimientoDeContenedores.Models;

public class CtEspDelEmp
{
    [Required(ErrorMessage = "El código del técnico es obligatorio.")]
    [StringLength(20, ErrorMessage = "El código del técnico no puede exceder 20 caracteres.")]
    [Display(Name = "Código del técnico")]
    public string CodTit { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del técnico es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre del técnico no puede exceder 150 caracteres.")]
    [Display(Name = "Nombre del técnico")]
    public string NomTit { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El usuario no puede exceder 50 caracteres.")]
    [Display(Name = "Usuario")]
    public string Usuario { get; set; } = string.Empty;

    public ICollection<CppEspDelEmp> EspecialidadesAsignadas { get; set; } = [];
}
