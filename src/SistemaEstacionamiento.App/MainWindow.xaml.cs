using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using SistemaEstacionamiento.App.ViewModels;
using SistemaEstacionamiento.Infrastructure.Data;

namespace SistemaEstacionamiento.App;

public partial class MainWindow : Window
{
    private static readonly string DbPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "estacionamiento.db");

    public MainWindow()
    {
        InitializeComponent();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={DbPath}")
            .Options;

        var context = new AppDbContext(options);
        context.Database.Migrate();

        DataContext = new ConfiguracionViewModel(context);
    }

    private void IrAGarita_Click(object sender, RoutedEventArgs e)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={DbPath}")
            .Options;
        var context = new AppDbContext(options);

        var eventoId = context.Eventos.OrderByDescending(ev => ev.Id).FirstOrDefault()?.Id ?? 0;
        if (eventoId == 0)
        {
            MessageBox.Show("Primero generá un evento con lugares.");
            return;
        }

        var garita = new GaritaWindow(context, eventoId);
        garita.ShowDialog();
    }
}
