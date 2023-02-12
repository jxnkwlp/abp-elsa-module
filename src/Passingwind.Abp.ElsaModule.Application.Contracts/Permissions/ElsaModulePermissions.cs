﻿using Volo.Abp.Reflection;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class ElsaModulePermissions
{
    public const string GroupName = "ElsaModule";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(ElsaModulePermissions));
    }

    public static class Instances
    {
        public const string Default = GroupName + ".Instances";
        public const string Action = Default + ".Action";
        public const string Delete = Default + ".Delete";
        public const string Data = Default + ".Data";
        public const string Statistic = Default + ".Statistics";
    }

    public static class Definitions
    {
        public const string Default = GroupName + ".Definitions";
        public const string Publish = Default + ".Publish";
        public const string Delete = Default + ".Delete";
        public const string Dispatch = Default + ".Dispatch";
        public const string Export = Default + ".Export";
        public const string Import = Default + ".Import";
    }

    public static class GlobalVariables
    {
        public const string Default = GroupName + ".GlobalVariables";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
}
