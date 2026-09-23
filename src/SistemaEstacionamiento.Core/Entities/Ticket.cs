namespace SistemaEstacionamiento.Core.Entities;

public class Ticket
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string TipoVehiculo { get; set; } = string.Empty;  // "AUTOS" o "CAMIONETAS"
    public string LugarCodigo { get; set; } = string.Empty;
    public string? Dni { get; set; }                          // ← NUEVO
    public string? Patente { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = "EFECTIVO";      // EFECTIVO / MERCADO_PAGO
    public string Operador { get; set; } = string.Empty;
}