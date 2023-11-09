using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using VideoGenerator.ViewModels;

namespace VideoGenerator.Tests;

//[SimpleJob(RuntimeMoniker.Net60, baseline: true)]
//[SimpleJob(RuntimeMoniker.Net70)]
//[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class Benchmarks
{
    private List<string> _files;
    private string _fileName = @".\uv_test.png";

    [Params(10, 50)]
    public int numFiles;

    [GlobalSetup]
    public void Setup ()
    {
        _files = Enumerable.Repeat(_fileName, numFiles).ToList();
    }

    [Benchmark]
    public void LoadFiles()
    {
        var mainWindowVM = new MainWindowVM();
        mainWindowVM.OpenFiles(_files);
    }
}

public class Program
{
    public static void Main (string[] args)
    {
        var config = DefaultConfig.Instance
                .AddJob(
                    Job.Default.WithToolchain(
                        CsProjCoreToolchain.From(
                            new NetCoreAppSettings(
                                targetFrameworkMoniker: "net5.0-windows",  // the key to make it work
                                runtimeFrameworkVersion: null,
                                name: "5.0"))))
                .AddJob(
                    Job.Default.WithToolchain(
                        CsProjCoreToolchain.From(
                            new NetCoreAppSettings(
                                targetFrameworkMoniker: "net6.0-windows",  // the key to make it work
                                runtimeFrameworkVersion: null,
                                name: "6.0"))))
                .AddJob(
                    Job.Default.WithToolchain(
                        CsProjCoreToolchain.From(
                            new NetCoreAppSettings(
                                targetFrameworkMoniker: "net7.0-windows",  // the key to make it work
                                runtimeFrameworkVersion: null,
                                name: "7.0"))))
                .AddJob(
                    Job.Default.WithToolchain(
                        CsProjCoreToolchain.From(
                            new NetCoreAppSettings(
                                targetFrameworkMoniker: "net8.0-windows",  // the key to make it work
                                runtimeFrameworkVersion: null,
                                name: "8.0")))
                                .AsDefault());

        //var config = DefaultConfig.Instance
        //        .AddJob(
        //            Job.Default.WithToolchain(
        //                CsProjCoreToolchain.From(
        //                    new NetCoreAppSettings(
        //                        targetFrameworkMoniker: "net6.0-windows",  // the key to make it work
        //                        runtimeFrameworkVersion: null,
        //                        name: "6.0")))
        //                        .AsDefault());

        var summary = BenchmarkRunner.Run<Benchmarks>(config);
    }
}