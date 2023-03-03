using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers;

public class MongoListSerializer<T> : SerializerBase<List<T>>
{
    private static readonly IBsonSerializer<T> _itemSerializer = BsonSerializer.LookupSerializer<T>();

    public override List<T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var result = new List<T>();

        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return result;
        }

        context.Reader.ReadStartArray();

        while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            result.Add(_itemSerializer.Deserialize(context));
        }

        context.Reader.ReadEndArray();

        return result;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, List<T> value)
    {
        context.Writer.WriteStartArray();

        if (value == null)
            context.Writer.WriteEndArray();
        else
        {
            foreach (var item in value)
            {
                _itemSerializer.Serialize(context, item);
            }

            context.Writer.WriteEndArray();
        }
    }
}
