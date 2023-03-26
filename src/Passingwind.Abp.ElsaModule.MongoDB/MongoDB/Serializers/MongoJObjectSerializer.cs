using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers;

public class MongoJObjectSerializer : SerializerBase<JObject>
{
    public override JObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return default;
        }

        var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

        var document = serializer.Deserialize(context, args);

        return JsonConvert.DeserializeObject<JObject>(document.ToJson(), MongoJsonSerializerSettings.Settings);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
    {
        var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

        var json = JsonConvert.SerializeObject(value, MongoJsonSerializerSettings.Settings);

        var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(json);

        serializer.Serialize(context, bsonDocument);
    }
}
