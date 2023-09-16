using System;
using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp;

public class CSharpTypeDefinitions
{
    public Dictionary<string, Type> Properties { get; } = new Dictionary<string, Type>();
    public Dictionary<string, CSharpMethodDefinition> Methods { get; } = new Dictionary<string, CSharpMethodDefinition>();

    public CSharpTypeDefinitions AddProperty(string name, Type type)
    {
        Properties[name] = type;
        return this;
    }

    //public CSharpTypeDefinitions AddMethod(CSharpMethodDefinition definition)
    //{
    //    Methods[definition.Name] = definition;
    //    return this;
    //}

    //public CSharpTypeDefinitions AddMethod(string name, Type returnType, params CSharpParamDefinition[] @params)
    //{
    //    Methods[name] = new CSharpMethodDefinition(name, returnType, @params);
    //    return this;
    //}

    //public CSharpTypeDefinitions AddMethod(string name, params CSharpParamDefinition[] @params)
    //{
    //    Methods[name] = new CSharpMethodDefinition(name, @params);
    //    return this;
    //}

    public class CSharpParamDefinition
    {
        public CSharpParamDefinition(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public Type Type { get; }
    }

    public class CSharpMethodDefinition
    {
        public CSharpMethodDefinition(string name, Type returnType, params CSharpParamDefinition[] @params)
        {
            Name = name;
            ReturnType = returnType;
            Params = @params;
        }

        public CSharpMethodDefinition(string name, params CSharpParamDefinition[] @params)
        {
            Name = name;
            Params = @params;
        }

        public string Name { get; }
        public Type ReturnType { get; }
        public CSharpParamDefinition[] Params { get; }
    }
}
