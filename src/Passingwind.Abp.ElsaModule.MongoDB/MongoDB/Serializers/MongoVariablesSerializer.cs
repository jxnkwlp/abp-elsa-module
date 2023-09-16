using System.Collections.Generic;
using Elsa.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers;

public class MongoVariablesSerializer : SerializerBase<Variables>
{
    public override Variables Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return new Variables();
        }

        var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

        var document = serializer.Deserialize(context, args);

        var json = document.ToJson();

        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, MongoJsonSerializerSettings.Settings);
        return new Variables(data);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Variables value)
    {
        if (value == null || value.Data == null)
        {
            context.Writer.WriteNull();
        }
        else
        {
            var json = JsonConvert.SerializeObject(value.Data, MongoJsonSerializerSettings.Settings);

            var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

            serializer.Serialize(context, BsonDocument.Parse(json));
        }
    }
}
