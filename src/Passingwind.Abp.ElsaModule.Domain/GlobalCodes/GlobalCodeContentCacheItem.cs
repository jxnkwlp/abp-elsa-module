using System;
using Volo.Abp.Caching;

namespace Passingwind.Abp.ElsaModule.GlobalCodes;

[Serializable]
[CacheName("GlobalCodeContent")]
public class GlobalCodeContentCacheItem
{
    public GlobalCodeContentCacheItem(string name, GlobalCodeLanguage language, GlobalCodeType type, Guid contentId, int version, string content)
    {
        Name = name;
        Language = language;
        Type = type;
        ContentId = contentId;
        Version = version;
        Content = content;
    }

    public string Name { get; set; }
    public GlobalCodeLanguage Language { get; set; }
    public GlobalCodeType Type { get; set; }
    public Guid ContentId { get; set; }
    public int Version { get; set; } = 1;
    public string Content { get; set; }
}
