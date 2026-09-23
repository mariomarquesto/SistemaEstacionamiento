using SistemaEstacionamiento.Core.Entities;

namespace SistemaEstacionamiento.Core.Services;

public class AsignadorService
{
    /// <summary>
    /// Dada una lista de lugares y un sector, devuelve el primer lugar libre
    /// en orden (fila 1, posición 1, luego posición 2, etc.).
    /// Devuelve null si el sector está completo.
    /// </summary>
    public Lugar? AsignarSiguiente(List<Lugar> lugares, string sector)
    {
        return lugares
            .Where(l => l.Sector == sector.ToUpper() && l.EstaLibre)
            .OrderBy(l => l.Fila)
            .ThenBy(l => l.Posicion)
            .FirstOrDefault();
    }
}