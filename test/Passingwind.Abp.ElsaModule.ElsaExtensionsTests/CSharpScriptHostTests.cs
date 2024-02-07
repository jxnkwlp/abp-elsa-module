using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Passingwind.CSharpScriptEngine;
using Shouldly;
using Xunit;

namespace Passingwind.Abp.ElsaModule;

public class CSharpScriptHostTests : ElsaModuleExtensionTestBase
{
    private readonly ILogger _logger;
    private readonly ICSharpScriptHost _cSharpScriptHost;

    public CSharpScriptHostTests()
    {
        _logger = GetRequiredService<ILogger<CSharpScriptHostTests>>();
        _cSharpScriptHost = GetRequiredService<ICSharpScriptHost>();
    }

    [Fact]
    public async Task CompileTest1()
    {
        const string code = @"using System;

Console.WriteLine(""Hello, World!"");"
;
        var result = await _cSharpScriptHost.CompileAsync(new CSharpScriptCompileContext(_logger, code));

        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task CompileTest2()
    {
        const string code = @" 
Console.WriteLine(""Hello, World!"");"
;
        var result = await _cSharpScriptHost.CompileAsync(new CSharpScriptCompileContext(_logger, code));

        result.Success.ShouldBeFalse();
    }

    [Fact]
    public async Task CompileTest3()
    {
        const string code = @" 
#r ""nuget: Newtonsoft.Json, 13.0.3""

Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(new { a = 1 }));

"
;
        var result = await _cSharpScriptHost.CompileAsync(new CSharpScriptCompileContext(_logger, code));

        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task CompileTest4()
    {
        const string code = @" 
#r ""System.Private.Uri""

using System.IO;
using System.Net;

var url = ""https://download.microsoft.com/download/4/C/8/4C830C0C-101F-4BF2-8FCB-32D9A8BA906A/Import_User_Sample_en.csv"";
var request = WebRequest.Create(url);
var response = request.GetResponse();
var dataStream = response.GetResponseStream();
var reader = new StreamReader(dataStream);
var csv = await reader.ReadToEndAsync();
reader.Close();
dataStream.Close();
response.Close();
var users = csv.Split('\n').Skip(1)
                .Select(line => line.Split(','))
                .Where(values => values.Length == 15)
                .Select(values => new {
                    firstName = values[1],
                    lastName = values[2],
                    officeNumber = int.Parse(values[6])
                });

foreach (var u in users)
     Console.WriteLine(u);
"
;
        var result = await _cSharpScriptHost.CompileAsync(new CSharpScriptCompileContext(_logger, code));

        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task RunTest1()
    {
        const string code = @" 
#r ""System.Private.Uri""

using System.IO;
using System.Net;

var url = ""https://download.microsoft.com/download/4/C/8/4C830C0C-101F-4BF2-8FCB-32D9A8BA906A/Import_User_Sample_en.csv"";
var request = WebRequest.Create(url);
var response = request.GetResponse();
var dataStream = response.GetResponseStream();
var reader = new StreamReader(dataStream);
var csv = await reader.ReadToEndAsync();
reader.Close();
dataStream.Close();
response.Close();
var users = csv.Split('\n').Skip(1)
                .Select(line => line.Split(','))
                .Where(values => values.Length == 15)
                .Select(values => new {
                    firstName = values[1],
                    lastName = values[2],
                    officeNumber = int.Parse(values[6])
                });

foreach (var u in users)
     Console.WriteLine(u);
"
;
        var _ = await _cSharpScriptHost.RunAsync(new CSharpScriptCompileContext(_logger, code));
    }

    [Fact]
    public async Task RunTest2()
    {
        const string code = @" 
#r ""nuget: Newtonsoft.Json, 13.0.3""

return Newtonsoft.Json.JsonConvert.SerializeObject(new { a = 1 });

"
;
        var result = await _cSharpScriptHost.RunAsync(new CSharpScriptCompileContext(_logger, code));

        result.GetType().ShouldBe(typeof(string));
    }

    [Fact]
    public async Task RunTest3()
    {
        const string code = @" 
return 1;
"
;
        var result = await _cSharpScriptHost.RunAsync(new CSharpScriptCompileContext(_logger, code));

        result.GetType().ShouldBe(typeof(int));
    }
}
