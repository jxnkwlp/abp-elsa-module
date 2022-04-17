using Volo.Abp.Reflection;

namespace Passingwind.Abp.ElsaModule.Permissions;

public class ElsaModulePermissions
{
    public const string GroupName = "ElsaModule";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(ElsaModulePermissions));
    }
}
