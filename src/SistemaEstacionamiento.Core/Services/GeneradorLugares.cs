using SistemaEstacionamiento.Core.Configuration;
using SistemaEstacionamiento.Core.Entities;

namespace SistemaEstacionamiento.Core.Services;

public static class GeneradorLugares
{
    /// <summary>
    /// Genera la lista completa de lugares a partir de la configuración.
    /// Se adapta automáticamente a cualquier capacidad.
    /// </summary>
    public static List<Lugar> Generar(ConfiguracionEvento config)
    {
        var lugares = new List<Lugar>();

        foreach (var sector in config.Sectores)
        {
            if (sector.Capacidad <= 0) continue;

            var porFila = sector.LugaresPorFila <= 0 ? 10 : sector.LugaresPorFila;
            var letra = sector.Nombre.Trim().ToUpper()[0];
            var totalFilas = (int)Math.Ceiling((double)sector.Capacidad / porFila);

            var contador = 0;
            for (var fila = 1; fila <= totalFilas; fila++)
            {
                for (var pos = 1; pos <= porFila; pos++)
                {
                    if (contador >= sector.Capacidad) break;

                    lugares.Add(new Lugar
                    {
                        Codigo = $"{letra}-F{fila:00}-{pos:00}",
                        Sector = sector.Nombre.ToUpper(),
                        Fila = $"F{fila:00}",
                        Posicion = pos,
                        Estado = "LIBRE",
                        EventoId = config.Id
                    });

                    contador++;
                }
            }
        }

        return lugares;
    }
}