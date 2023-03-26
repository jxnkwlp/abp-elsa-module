using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers;

public class MongoJArraySerializer : SerializerBase<JArray>
{
    public override JArray Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        throw new NotSupportedException();
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JArray value)
    {
        var serializer = BsonSerializer.LookupSerializer(typeof(BsonArray));

        var json = value.ToString(Newtonsoft.Json.Formatting.None);

        var bson = BsonSerializer.Deserialize<BsonArray>(json);

        serializer.Serialize(context, bson);
    }
}
