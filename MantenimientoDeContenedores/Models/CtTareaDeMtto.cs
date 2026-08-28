using System.ComponentModel.DataAnnotations;

namespace MantenimientoDeContenedores.Models;

public class CtTareaDeMtto
{
    [Range(1, int.MaxValue, ErrorMessage = "El número de tarea debe ser mayor que 0.")]
    [Display(Name = "Nro.")]
    public int NroTarea { get; set; }

    [Required(ErrorMessage = "El nombre de la tarea es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre de la tarea no puede exceder 200 caracteres.")]
    [Display(Name = "Nombre de tarea")]
    public string NombreTarea { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes seleccionar una especialidad.")]
    [StringLength(20, ErrorMessage = "La especialidad no puede exceder 20 caracteres.")]
    [Display(Name = "Especialidad")]
    public string CodEspMtto { get; set; } = string.Empty;

    [Range(0, 9999.99, ErrorMessage = "El tiempo estimado debe ser mayor o igual a 0.")]
    [Display(Name = "Tiempo estimado")]
    public decimal TiempoEstimado { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    public CtEspMtto? Especialidad { get; set; }
}
