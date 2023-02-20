using MediatR;
using Passingwind.Abp.ElsaModule.CSharp;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.Messages;

public class CSharpScriptConfigureNotification : INotification
{
    public CSharpScriptConfigureNotification(string programText)
    {
        ProgramText = programText;
        Reference = new CSharpScriptReference();
    }

    public string ProgramText { get; }

    public CSharpScriptReference Reference { get; set; }
}
