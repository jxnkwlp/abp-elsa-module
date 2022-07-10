using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers
{
    public class MongoDictionarySerializer<TDictionary> : SerializerBase<TDictionary>
    {
        public override TDictionary Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return default;
            }

            var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

            var document = serializer.Deserialize(context, args);

            if (document == null)
                return Activator.CreateInstance<TDictionary>();

            var bsonDocument = document.ToBsonDocument();

            var result = bsonDocument.ToJson();

            return JsonConvert.DeserializeObject<TDictionary>(result);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TDictionary value)
        {
            if (value == null)
                context.Writer.WriteNull();
            else
            {
                var jsonDocument = JsonConvert.SerializeObject(value);

                var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(jsonDocument);

                var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));

                serializer.Serialize(context, bsonDocument.AsBsonValue);
            }
        }
    }
}