using System;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers;

public class MongoJsonElementSerializer : SerializerBase<JsonElement>
{
    public override JsonElement Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        throw new NotSupportedException();
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonElement value)
    {
        var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

        var document = BsonDocument.Parse(value.GetRawText());

        serializer.Serialize(context, document);
    }
}
