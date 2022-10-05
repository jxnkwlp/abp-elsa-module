//using System;
//using Elsa.Design;
//using Elsa.Serialization.Converters;
//using Newtonsoft.Json;

//namespace Passingwind.Abp.ElsaModule
//{
//    public class ElsaDesignerRuntimeSelectListSettings
//    {
//        [JsonConverter(typeof(FullTypeJsonConverter))]
//        public Type RuntimeSelectListProviderType { get; }

//        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
//        public object Context { get; }

//        public Uri RemoteUri { get; set; }
//    }
//}
