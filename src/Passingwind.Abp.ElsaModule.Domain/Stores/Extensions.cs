using System;
using System.Linq.Expressions;
using Elsa.Models;
using LinqKit;
using Passingwind.Abp.ElsaModule.Common;

namespace Passingwind.Abp.ElsaModule.Stores
{
    public static class Extensions
    {
        public static Guid? ToGuid(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            else
                return Guid.Parse(value);
        }

        public static Expression<Func<WorkflowDefinitionVersion, bool>> WithVersion(this Expression<Func<WorkflowDefinitionVersion, bool>> expression, VersionOptions? versionOptions)
        {
            var option = versionOptions ?? VersionOptions.Latest;

            if (option.IsDraft)
                return expression.And(x => !x.IsPublished);
            else if (option.IsLatest)
                return expression.And(x => x.IsLatest);
            else if (option.IsPublished)
                return expression.And(x => x.IsPublished);
            else if (option.IsLatestOrPublished)
                return expression.And(x => x.IsPublished || x.IsLatest);
            else if (option.Version > 0)
                return expression.And(x => x.Version == option.Version);
            else // AllVersions
                return expression;
        }
    }
}
