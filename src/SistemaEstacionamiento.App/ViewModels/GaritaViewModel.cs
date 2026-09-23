using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using SistemaEstacionamiento.Core.Entities;
using SistemaEstacionamiento.Infrastructure.Data;

namespace SistemaEstacionamiento.App.ViewModels;

public class GaritaViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _context;
    private readonly int _eventoId;

    private string _sectorSeleccionado = "AUTOS";
    private string _dni = string.Empty;
    private string _patente = string.Empty;
    private decimal _precio = 5000;
    private string _metodoPago = "EFECTIVO";
    private string _operador = "Juan";
    private string _mensaje = "Listo para cobrar.";
    private string _lugarAsignado = string.Empty;
    private bool _cobroExitoso;
    private bool _puedeCobrar = true;

    public GaritaViewModel(AppDbContext context, int eventoId)
    {
        _context = context;
        _eventoId = eventoId;

        Sectores = new ObservableCollection<string>();
        MetodosPago = new ObservableCollection<string> { "EFECTIVO", "MERCADO_PAGO" };

        CobrarCommand = new RelayCommand(
            async _ => await CobrarAsync(),
            _ => PuedeCobrar);

        NuevoCobroCommand = new RelayCommand(_ => NuevoCobro());

        _ = CargarSectoresAsync();
    }

    public ObservableCollection<string> Sectores { get; }
    public ObservableCollection<string> MetodosPago { get; }

    public string SectorSeleccionado
    {
        get => _sectorSeleccionado;
        set
        {
            _sectorSeleccionado = value;
            OnPropertyChanged();
            _ = ActualizarPrecioAsync();
        }
    }

    public string Dni
    {
        get => _dni;
        set { _dni = value; OnPropertyChanged(); }
    }

    public string Patente
    {
        get => _patente;
        set { _patente = value; OnPropertyChanged(); }
    }

    public decimal Precio
    {
        get => _precio;
        set { _precio = value; OnPropertyChanged(); }
    }

    public string MetodoPago
    {
        get => _metodoPago;
        set { _metodoPago = value; OnPropertyChanged(); }
    }

    public string Operador
    {
        get => _operador;
        set { _operador = value; OnPropertyChanged(); }
    }

    public string Mensaje
    {
        get => _mensaje;
        set { _mensaje = value; OnPropertyChanged(); }
    }

    public string LugarAsignado
    {
        get => _lugarAsignado;
        set { _lugarAsignado = value; OnPropertyChanged(); }
    }

    public bool CobroExitoso
    {
        get => _cobroExitoso;
        set { _cobroExitoso = value; OnPropertyChanged(); }
    }

    public bool PuedeCobrar
    {
        get => _puedeCobrar;
        set { _puedeCobrar = value; OnPropertyChanged(); }
    }

    public ICommand CobrarCommand { get; }
    public ICommand NuevoCobroCommand { get; }

    private async Task CargarSectoresAsync()
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == _eventoId);
        if (evento == null) return;

        Sectores.Clear();
        foreach (var s in evento.Sectores)
            Sectores.Add(s.Nombre);

        if (Sectores.Any())
            SectorSeleccionado = Sectores.First();

        await ActualizarPrecioAsync();
    }

    private async Task ActualizarPrecioAsync()
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == _eventoId);
        if (evento == null) return;

        var sector = evento.Sectores.FirstOrDefault(s => s.Nombre == SectorSeleccionado);
        if (sector != null)
            Precio = sector.Precio;
    }

    private async Task CobrarAsync()
    {
        if (!PuedeCobrar) return;
        PuedeCobrar = false;

        try
        {
            // 1. Validar DNI
            if (string.IsNullOrWhiteSpace(Dni))
            {
                Mensaje = "⚠️ El DNI es obligatorio.";
                return;
            }

            var dniLimpio = Dni.Trim().Replace(".", "").Replace(" ", "");
            if (dniLimpio.Length < 7 || dniLimpio.Length > 8 || !dniLimpio.All(char.IsDigit))
            {
                Mensaje = "⚠️ El DNI debe tener 7 u 8 dígitos numéricos.";
                return;
            }

            // 2. Validar DNI duplicado
            var lugarExistente = await _context.Lugares
                .Where(l => l.EventoId == _eventoId
                         && l.Dni == dniLimpio
                         && l.Estado == "OCUPADO")
                .FirstOrDefaultAsync();

            if (lugarExistente != null)
            {
                Mensaje = $"⚠️ El DNI {dniLimpio} ya está en el lugar {lugarExistente.Codigo}. No se genera un nuevo ticket.";
                CobroExitoso = false;
                return;
            }

            // 3. Buscar el primer lugar libre del sector
            var lugar = await _context.Lugares
                .Where(l => l.EventoId == _eventoId
                         && l.Sector == SectorSeleccionado.ToUpper()
                         && l.Estado == "LIBRE")
                .OrderBy(l => l.Fila)
                .ThenBy(l => l.Posicion)
                .FirstOrDefaultAsync();

            if (lugar == null)
            {
                Mensaje = $"❌ No hay más lugares libres en {SectorSeleccionado}.";
                CobroExitoso = false;
                return;
            }

            // 4. Marcar lugar como ocupado
            lugar.Estado = "OCUPADO";
            lugar.Dni = dniLimpio;
            lugar.Patente = string.IsNullOrWhiteSpace(Patente) ? null : Patente.ToUpper();
            lugar.HoraIngreso = DateTime.Now;
            lugar.MetodoPago = MetodoPago;
            lugar.Monto = Precio;

            // 5. Registrar el ticket
            var ticket = new Ticket
            {
                EventoId = _eventoId,
                Fecha = DateTime.Now,
                TipoVehiculo = SectorSeleccionado.ToUpper(),
                LugarCodigo = lugar.Codigo,
                Dni = dniLimpio,
                Patente = lugar.Patente,
                Monto = Precio,
                MetodoPago = MetodoPago,
                Operador = Operador
            };
            _context.Tickets.Add(ticket);

            await _context.SaveChangesAsync();

            LugarAsignado = lugar.Codigo;
            CobroExitoso = true;
            Mensaje = $"✅ Cobrado ${Precio:N0} - DNI {dniLimpio} → Lugar {lugar.Codigo}";
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            if (ex.InnerException != null)
                msg += " → " + ex.InnerException.Message;
            if (ex.InnerException?.InnerException != null)
                msg += " → " + ex.InnerException.InnerException.Message;
            Mensaje = $"❌ Error: {msg}";
            CobroExitoso = false;
        }
        finally
        {
            PuedeCobrar = true;
        }
    }

    private void NuevoCobro()
    {
        Dni = string.Empty;
        Patente = string.Empty;
        LugarAsignado = string.Empty;
        CobroExitoso = false;
        Mensaje = "Listo para cobrar.";
        PuedeCobrar = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}