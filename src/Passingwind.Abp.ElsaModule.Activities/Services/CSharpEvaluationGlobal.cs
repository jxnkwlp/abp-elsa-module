using System.Collections.Generic;
using System.Dynamic;

namespace Passingwind.Abp.ElsaModule.Services
{
    public class CSharpEvaluationGlobal
    {
        public dynamic Context { get; }

        public CSharpEvaluationGlobal()
        {
            Context = new CSharpEvaluationContextContainer();
        }
    }

    public class CSharpEvaluationContextContainer : DynamicObject
    {
        private static readonly Dictionary<string, object> _member = new();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _member[binder.Name.ToLower()] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _member.TryGetValue(binder.Name.ToLower(), out result);
        }
    }
}
