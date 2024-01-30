using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Passingwind.CSharpScriptEngine.References;

public class AssemblyReference : ScriptDirectiveReference
{
    public string Name { get; set; }

    public AssemblyReference(string name)
    {
        Name = name;
    }
}
