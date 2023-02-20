using System.Collections.Generic;
using System.Dynamic;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class CSharpScriptEvaluationGlobal
{
    public dynamic Context { get; }

    public CSharpScriptEvaluationGlobal()
    {
        Context = new ContextContainer();
    }

    public void AddMember(string name, object value) => Context.AddMember(name, value);
    public void AddVariable(string name, object value) => Context.AddMember(name, value);

    public class ContextContainer : DynamicObject
    {
        private static readonly Dictionary<string, object> _member = new();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _member[binder.Name.ToLowerInvariant()] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _member.TryGetValue(binder.Name.ToLower(), out result);
        }

        public void AddMember(string name, object value)
        {
            _member[name.ToLowerInvariant()] = value;
        }
    }
}
