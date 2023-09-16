namespace Passingwind.Abp.ElsaModule;

public static class ElsaModuleDbProperties
{
    public static string DbTablePrefix { get; set; } = "Elsa";

    public static string DbSchema { get; set; }

    public const string ConnectionStringName = "Elsa";
}
