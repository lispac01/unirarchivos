namespace MantenimientoDeContenedores.Models;

public class TrPreviajeTareaDetalle
{
    public int NroPreviaje { get; set; }
    public int NroTarea { get; set; }
    public decimal Cantidad { get; set; }
    public bool Garantia { get; set; }
    public string CodTit { get; set; } = string.Empty;

    public TrPreviajeContenedor? Previaje { get; set; }
    public CtTareaDeMtto? Tarea { get; set; }
    public CtEspDelEmp? Tecnico { get; set; }
}
