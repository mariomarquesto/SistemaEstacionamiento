using System.Windows;
using Microsoft.EntityFrameworkCore;
using SistemaEstacionamiento.App.ViewModels;
using SistemaEstacionamiento.Infrastructure.Data;

namespace SistemaEstacionamiento.App;

public partial class GaritaWindow : Window
{
    public GaritaWindow(AppDbContext context, int eventoId)
    {
        System.Windows.Application.LoadComponent(
            this,
            new System.Uri("GaritaWindow.xaml", System.UriKind.Relative));
        DataContext = new GaritaViewModel(context, eventoId);
    }
}
