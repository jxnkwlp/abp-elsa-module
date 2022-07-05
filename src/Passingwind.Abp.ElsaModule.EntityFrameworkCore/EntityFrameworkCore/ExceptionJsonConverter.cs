//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace Abp.ElsaManagement.Internal
//{
//    public class ExceptionJsonConverter : JsonConverter<Exception>
//    {
//        public override Exception ReadJson(JsonReader reader, Type objectType, Exception existingValue, bool hasExistingValue, JsonSerializer serializer)
//        {
//            var jobject = JObject.Load(reader);

//            return new JsonRestoredException(jobject.GetValue("Message").Value<string>());
//        }

//        public override void WriteJson(JsonWriter writer, Exception value, JsonSerializer serializer)
//        {
//            var dict = new Dictionary<string, object>();

//            var type = value.GetType();
//            dict["ClassName"] = type.FullName;
//            dict["Message"] = value.Message;
//            dict["Data"] = value.Data;
//            dict["InnerException"] = value.InnerException;
//            dict["HResult"] = value.HResult;
//            dict["Source"] = value.Source;

//            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
//            {
//                if (!dict.ContainsKey(p.Name))
//                    dict[p.Name] = p.GetValue(value);
//            }

//            writer.WriteValue(JsonConvert.SerializeObject(dict));
//        }
//    }

//    public class JsonRestoredException : Exception
//    {
//        public JsonRestoredException() : base()
//        {
//        }

//        public JsonRestoredException(string message) : base(message)
//        {
//        }

//        public JsonRestoredException(string message, Exception innerException) : base(message, innerException)
//        {
//        }
//    }
//}