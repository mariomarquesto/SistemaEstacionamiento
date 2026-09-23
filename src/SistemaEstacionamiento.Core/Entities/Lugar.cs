namespace SistemaEstacionamiento.Core.Entities;

public class Lugar
{
    public string Codigo { get; set; } = string.Empty;      // Ej: "A-F03-07"
    public string Sector { get; set; } = string.Empty;      // Ej: "AUTOS"
    public string Fila { get; set; } = string.Empty;        // Ej: "F03"
    public int Posicion { get; set; }                       // Ej: 7
    public string Estado { get; set; } = "LIBRE";           // LIBRE / OCUPADO
    public string? Dni { get; set; }                        // ← NUEVO
    public string? Patente { get; set; }
    public DateTime? HoraIngreso { get; set; }
    public string? MetodoPago { get; set; }                 // EFECTIVO / MERCADO_PAGO
    public decimal? Monto { get; set; }
    public int EventoId { get; set; }

    public bool EstaLibre => Estado == "LIBRE";
}