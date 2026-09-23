using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using SistemaEstacionamiento.Core.Configuration;
using SistemaEstacionamiento.Core.Entities;
using SistemaEstacionamiento.Core.Services;
using SistemaEstacionamiento.Infrastructure.Data;

namespace SistemaEstacionamiento.App.ViewModels;

public class ConfiguracionViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _context;
    private string _nombreEvento = "Recital";
    private string _mensaje = "Configurá los sectores y presioná Generar.";

    public ConfiguracionViewModel(AppDbContext context)
    {
        _context = context;
        Sectores = new ObservableCollection<SectorConfig>
        {
            new() { Nombre = "AUTOS", Capacidad = 100, LugaresPorFila = 10, Precio = 5000 },
            new() { Nombre = "CAMIONETAS", Capacidad = 30, LugaresPorFila = 10, Precio = 8000 }
        };

        AgregarSectorCommand = new RelayCommand(_ => AgregarSector());
        QuitarSectorCommand = new RelayCommand(s => QuitarSector(s as SectorConfig));
        GenerarCommand = new RelayCommand(async _ => await GenerarAsync());
    }

    public ObservableCollection<SectorConfig> Sectores { get; }

    public string NombreEvento
    {
        get => _nombreEvento;
        set { _nombreEvento = value; OnPropertyChanged(); }
    }

    public string Mensaje
    {
        get => _mensaje;
        set { _mensaje = value; OnPropertyChanged(); }
    }

    public ICommand AgregarSectorCommand { get; }
    public ICommand QuitarSectorCommand { get; }
    public ICommand GenerarCommand { get; }

    private void AgregarSector()
    {
        Sectores.Add(new SectorConfig
        {
            Nombre = "NUEVO",
            Capacidad = 10,
            LugaresPorFila = 5,
            Precio = 1000
        });
    }

    private void QuitarSector(SectorConfig? sector)
    {
        if (sector != null && Sectores.Contains(sector))
            Sectores.Remove(sector);
    }

    private async Task GenerarAsync()
    {
        try
        {
            if (Sectores.Count == 0)
            {
                Mensaje = "Agregá al menos un sector.";
                return;
            }

            foreach (var s in Sectores)
            {
                if (string.IsNullOrWhiteSpace(s.Nombre))
                {
                    Mensaje = "Todos los sectores deben tener nombre.";
                    return;
                }
                if (s.Capacidad <= 0)
                {
                    Mensaje = $"El sector {s.Nombre} debe tener capacidad mayor a 0.";
                    return;
                }
            }

            var evento = new ConfiguracionEvento
            {
                Nombre = NombreEvento,
                Fecha = DateTime.Today,
                Sectores = Sectores.ToList()
            };

            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            var lugares = GeneradorLugares.Generar(evento);
            await _context.Lugares.AddRangeAsync(lugares);
            await _context.SaveChangesAsync();

            var totalEnDb = await _context.Lugares.CountAsync(l => l.EventoId == evento.Id);
            Mensaje = $"✅ Evento '{evento.Nombre}' creado con {lugares.Count} lugares (en DB: {totalEnDb}).";
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            if (ex.InnerException != null)
                msg += " → " + ex.InnerException.Message;
            if (ex.InnerException?.InnerException != null)
                msg += " → " + ex.InnerException.InnerException.Message;
            Mensaje = $"❌ Error: {msg}";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}