namespace SistemaEstacionamiento.Core.Configuration;

public class ConfiguracionEvento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "Evento sin nombre";
    public DateTime Fecha { get; set; } = DateTime.Today;
    public List<SectorConfig> Sectores { get; set; } = new();
}

public class SectorConfig
{
    public string Nombre { get; set; } = string.Empty;       // "AUTOS", "CAMIONETAS"
    public int Capacidad { get; set; }                       // 100, 300, 20...
    public int LugaresPorFila { get; set; } = 10;            // 10 por defecto
    public decimal Precio { get; set; }                      // 5000, 8000...
}