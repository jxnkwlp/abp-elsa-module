using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Passingwind.Abp.ElsaModule.Roslyn;

public class RoslynHost_Test : ElsaModuleExtensionTestBase
{
    private readonly IRoslynHost _roslynHost;

    public RoslynHost_Test()
    {
        _roslynHost = GetRequiredService<IRoslynHost>();
    }

    [Fact]
    public async Task Diagnostics_Test_01()
    {
        string id = Guid.NewGuid().ToString();

        var project = _roslynHost.GetOrCreateProject(id);

        _roslynHost.CreateOrUpdateDocument(project.Name, "Program", @"
        var a = Guid.NewGuid();
        ", true);

        var diagnostics = await _roslynHost.GetDiagnosticsAsync(project.Name);

        diagnostics.Count().ShouldBe(0);
    }

    [Fact]
    public async Task Diagnostics_Test_02()
    {
        string id = Guid.NewGuid().ToString();

        var project = _roslynHost.GetOrCreateProject(id);

        _roslynHost.CreateOrUpdateDocument(project.Name, "Class1", @"
public class MyClass1 {
   public int Property1 { get; set; }
   public Guid Guid2 { get; set; }
}
        ");
        _roslynHost.CreateOrUpdateDocument(project.Name, "Class2", @"
namespace MyNamespace;
public class MyClass2 {
   public int Property1 { get; set; }
   public Guid Guid2 { get; set; }
}
        ");
        _roslynHost.CreateOrUpdateDocument(project.Name, "Program", @"
using MyNamespace;

var a = Guid.NewGuid();
var tmp1 = new MyClass1();
_ = tmp1.Property1;
_ = tmp1.Guid2;

var tmp2 = new MyClass2();

        ", true);

        var diagnostics = await _roslynHost.GetDiagnosticsAsync(project.Name);

        diagnostics.Count().ShouldBe(0);
    }

    [Fact]
    public async Task Diagnostics_Test_03()
    {
        string id = Guid.NewGuid().ToString();

        var project = _roslynHost.GetOrCreateProject(id);

        _roslynHost.CreateOrUpdateDocument(project.Name, "Program", @"
#r ""nuget: Newtonsoft.Json, 13.0.2""
using Newtonsoft.Json;

_ = JsonConvert.SerializeObject(new { a = 1, b = DateTime.Now});

        ", true);

        var diagnostics = await _roslynHost.GetDiagnosticsAsync(project.Name);

        diagnostics.Count().ShouldBe(0);
    }
}
