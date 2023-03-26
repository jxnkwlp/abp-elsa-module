using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Passingwind.Abp.ElsaModule.Common;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers;

public class MongoSimpleExceptionModelSerializer : SerializerBase<SimpleExceptionModel>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, SimpleExceptionModel value)
    {
        var json = JsonConvert.SerializeObject(value, MongoJsonSerializerSettings.Settings);

        var serializer = BsonSerializer.LookupSerializer<BsonDocument>();

        serializer.Serialize(context, args, BsonDocument.Parse(json));
    }

    public override SimpleExceptionModel Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var serializer = BsonSerializer.LookupSerializer<BsonDocument>();

        var bson = serializer.Deserialize(context, args);

        return JsonConvert.DeserializeObject<SimpleExceptionModel>(bson.ToJson(), MongoJsonSerializerSettings.Settings);
    }
}
