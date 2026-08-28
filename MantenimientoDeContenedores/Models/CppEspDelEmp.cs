namespace MantenimientoDeContenedores.Models;

public class CppEspDelEmp
{
    public string CodTit { get; set; } = string.Empty;
    public string CodEspMtto { get; set; } = string.Empty;

    public CtEspDelEmp? Tecnico { get; set; }
    public CtEspMtto? Especialidad { get; set; }
}
