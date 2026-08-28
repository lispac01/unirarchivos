using System.ComponentModel.DataAnnotations;

namespace MantenimientoDeContenedores.Models;

public class PreviajeContenedorFormViewModel
{
    [Display(Name = "Nro. previaje")]
    public int NroPreviaje { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un ingreso.")]
    [Display(Name = "Ingreso")]
    public int? NumIngreso { get; set; }

    [Required(ErrorMessage = "Debes seleccionar una especialidad.")]
    [Display(Name = "Tipo de mantenimiento")]
    public string CodEspMtto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes seleccionar un técnico.")]
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

    public IReadOnlyCollection<IngresoContenedorLookupItem> IngresosDisponibles { get; set; } = Array.Empty<IngresoContenedorLookupItem>();
    public IReadOnlyCollection<CtEspMtto> EspecialidadesDisponibles { get; set; } = Array.Empty<CtEspMtto>();
    public IReadOnlyCollection<TecnicoEspecialidadLookupItem> TecnicosDisponibles { get; set; } = Array.Empty<TecnicoEspecialidadLookupItem>();
    public List<PreviajeTareaEditorRowViewModel> TareasEditor { get; set; } = [];
    public decimal TotalGeneral { get; set; }
}

public class IngresoContenedorLookupItem
{
    public int NumIngreso { get; set; }
    public string CodContenedor { get; set; } = string.Empty;
    public string NombreContenedor { get; set; } = string.Empty;
    public string CodCliente { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public string CodigosEspecialidadesRegistradas { get; set; } = string.Empty;
}

public class TecnicoEspecialidadLookupItem
{
    public string CodTit { get; set; } = string.Empty;
    public string NomTit { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string CodEspMtto { get; set; } = string.Empty;
}

public class PreviajeTareaEditorRowViewModel
{
    public bool Seleccionada { get; set; }
    public int NroTarea { get; set; }
    public string NombreTarea { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public bool Garantia { get; set; }
    public string CodTit { get; set; } = string.Empty;
    public decimal TiempoEstimado { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal TotalRep { get; set; }
    public decimal Total { get; set; }
}
