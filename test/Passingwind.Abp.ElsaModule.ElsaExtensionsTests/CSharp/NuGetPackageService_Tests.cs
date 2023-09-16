using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class NuGetPackageService_Tests : ElsaModuleExtensionTestBase
{
    private readonly INuGetPackageService _service;

    public NuGetPackageService_Tests()
    {
        _service = GetRequiredService<INuGetPackageService>();
    }

    [Fact]
    public async Task GetReferencesAsyncTest_01()
    {
        var files = await _service.GetReferencesAsync("Newtonsoft.Json", "13.0.2", "net6.0", false, false);

        files.ShouldNotBeEmpty();
        files.Count().ShouldBe(1);
    }

    [Fact]
    public async Task GetReferencesAsyncTest_02()
    {
        var files = await _service.GetReferencesAsync("System.Text.Json", "7.0.2", "net6.0", true, false);

        files.ShouldNotBeEmpty();
        files.Count().ShouldBe(3);
    }

    [Fact]
    public async Task GetReferencesAsyncTest_03()
    {
        var files = await _service.GetReferencesAsync("Newtonsoft.Json", "13.0.2", "net6.0", false, true);

        files.ShouldNotBeEmpty();

        files.ShouldAllBe(x => File.Exists(x));
    }

    [Fact]
    public async Task GetReferencesAsyncTest_04()
    {
        var files = await _service.GetReferencesAsync("MongoDB.Driver", "2.19.0", "net6.0", resolveDependency: true, downloadPackage: true);

        files.ShouldNotBeEmpty();

        files.ShouldAllBe(x => File.Exists(x));
    }

    [Fact]
    public async Task DownloadAsyncTest_01()
    {
        await _service.DownloadAsync("Newtonsoft.Json", "13.0.2", "net6.0");
    }

    [Fact]
    public async Task DownloadAsyncTest_02()
    {
        await _service.DownloadAsync("System.Text.Json", "7.0.2", "net6.0", true);
    }

    [Fact]
    public async Task SearchAsyncTest()
    {
        var packages = await _service.SearchAsync("json");

        packages.Count().ShouldBe(10);
    }

    [Fact]
    public async Task GetPackageVersionsAsyncTest()
    {
        var version = await _service.GetPackageVersionsAsync("Newtonsoft.Json");

        version.Count().ShouldBeGreaterThan(0);
    }
}
