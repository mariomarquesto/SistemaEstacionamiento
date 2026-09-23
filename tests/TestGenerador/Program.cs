using SistemaEstacionamiento.Core.Configuration;
using SistemaEstacionamiento.Core.Services;

Console.WriteLine("=== TEST DEL GENERADOR DE LUGARES ===\n");

Probar(20, 5);
Probar(100, 10);
Probar(300, 15);
Probar(7, 10);

static void Probar(int capacidad, int porFila)
{
    var config = new ConfiguracionEvento
    {
        Id = 1,
        Nombre = $"Test {capacidad}",
        Sectores = new List<SectorConfig>
        {
            new SectorConfig
            {
                Nombre = "AUTOS",
                Capacidad = capacidad,
                LugaresPorFila = porFila,
                Precio = 5000
            }
        }
    };

    var lugares = GeneradorLugares.Generar(config);

    Console.WriteLine($"Capacidad: {capacidad}, Por fila: {porFila}");
    Console.WriteLine($"  Generados: {lugares.Count}");
    Console.WriteLine($"  Primero:   {lugares.First().Codigo}");
    Console.WriteLine($"  Último:    {lugares.Last().Codigo}");
    Console.WriteLine($"  Filas:     {lugares.Select(l => l.Fila).Distinct().Count()}");
    Console.WriteLine();
}
