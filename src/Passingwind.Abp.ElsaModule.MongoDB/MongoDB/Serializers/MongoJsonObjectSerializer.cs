using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers;

public class MongoJsonObjectSerializer<TData> : SerializerBase<TData>
{
    public override TData Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return default;
        }

        var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

        var document = serializer.Deserialize(context, args);

        var json = document.ToJson();

        return JsonConvert.DeserializeObject<TData>(json, MongoJsonSerializerSettings.Settings);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TData value)
    {
        if (value == null)
            context.Writer.WriteNull();
        else
        {
            var jsonDocument = JsonConvert.SerializeObject(value, MongoJsonSerializerSettings.Settings);

            var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(jsonDocument);

            var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

            serializer.Serialize(context, bsonDocument.AsBsonValue);
        }
    }
}