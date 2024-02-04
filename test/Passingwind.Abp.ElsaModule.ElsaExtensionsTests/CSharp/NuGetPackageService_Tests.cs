using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Passingwind.CSharpScriptEngine;
using Shouldly;
using Xunit;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class NuGetPackageServiceTests : ElsaModuleExtensionTestBase
{
    private readonly INuGetPackageService _service;

    public NuGetPackageServiceTests()
    {
        _service = GetRequiredService<INuGetPackageService>();
    }

    [Fact]
    public async Task GetReferencesAsyncTest01()
    {
        var files = await _service.GetReferencesAsync("Newtonsoft.Json", "13.0.2", "net6.0", false);

        files.ShouldNotBeEmpty();
        files.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetReferencesAsyncTest02()
    {
        var files = await _service.GetReferencesAsync("System.Text.Json", "7.0.2", "net6.0", true);

        files.ShouldNotBeEmpty();
        files.Count.ShouldBe(3);
    }

    [Fact]
    public async Task GetReferencesAsyncTest03()
    {
        var files = await _service.GetReferencesAsync("Newtonsoft.Json", "13.0.2", "net6.0", false);

        files.ShouldNotBeEmpty();

        files.ShouldAllBe(x => File.Exists(x));
    }

    [Fact]
    public async Task GetReferencesAsyncTest04()
    {
        var files = await _service.GetReferencesAsync("MongoDB.Driver", "2.19.0", "net6.0", true);

        files.ShouldNotBeEmpty();

        files.ShouldAllBe(x => File.Exists(x));
    }

    [Fact]
    public async Task DownloadAsyncTest01()
    {
        await _service.DownloadAsync("Newtonsoft.Json", "13.0.2");
        await _service.DownloadAsync("System.Text.Json", "7.0.2");
    }

    [Fact]
    public async Task SearchAsyncTest()
    {
        var packages = await _service.SearchAsync("json");

        packages.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetPackageVersionsAsyncTest()
    {
        var version = await _service.GetVersionsAsync("Newtonsoft.Json");

        version.Count.ShouldBeGreaterThan(0);
    }
}
