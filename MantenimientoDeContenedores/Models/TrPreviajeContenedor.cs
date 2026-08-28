using System.ComponentModel.DataAnnotations;

namespace MantenimientoDeContenedores.Models;

public class TrPreviajeContenedor
{
    [Display(Name = "Nro. previaje")]
    public int NroPreviaje { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un ingreso.")]
    [Display(Name = "Ingreso")]
    public int NumIngreso { get; set; }

    [Required(ErrorMessage = "Debes seleccionar una especialidad.")]
    [StringLength(20, ErrorMessage = "La especialidad no puede exceder 20 caracteres.")]
    [Display(Name = "Tipo de mantenimiento")]
    public string CodEspMtto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes seleccionar un técnico.")]
    [StringLength(20, ErrorMessage = "El código del técnico no puede exceder 20 caracteres.")]
    [Display(Name = "Técnico")]
    public string CodTit { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha del previaje es obligatoria.")]
    [Display(Name = "Fecha")]
    public DateTime FecPreviaje { get; set; }

    [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres.")]
    [Display(Name = "Observaciones")]
    public string Observaciones { get; set; } = string.Empty;

    [Display(Name = "Habilitado")]
    public bool Habilitado { get; set; }

    public TrIngresoContenedor? Ingreso { get; set; }
    public CtEspMtto? Especialidad { get; set; }
    public CtEspDelEmp? Tecnico { get; set; }
}
