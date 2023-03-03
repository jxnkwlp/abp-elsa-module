using Newtonsoft.Json;

namespace Passingwind.Abp.ElsaModule.Workflow;

[JsonObject(ItemTypeNameHandling = TypeNameHandling.All)]
public class RuntimeSelectListContextDto
{
    public string ProviderTypeName { get; set; } = default!;

    public object Context { get; set; }
}
