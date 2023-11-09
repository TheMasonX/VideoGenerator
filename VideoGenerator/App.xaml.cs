using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace VideoGenerator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    ILogger Logger { get; set; }
    private string _logFile = $"./VideoGenerator.Log";
    private LogEventLevel _traceLogLevel = LogEventLevel.Verbose;

    public App () : base ()
    {
        Logger = new LoggerConfiguration()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithEnvironmentUserName()
            .WriteTo.File(_logFile)
            .WriteTo.Trace(_traceLogLevel)
            .CreateLogger();

        Ioc.Default.ConfigureServices(
            new ServiceCollection()
            .AddSingleton(Logger)
            .BuildServiceProvider());
    }
}