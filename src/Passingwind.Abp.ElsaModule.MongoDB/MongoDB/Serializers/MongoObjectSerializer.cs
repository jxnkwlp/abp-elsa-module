using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace Passingwind.Abp.ElsaModule.MongoDB.Serializers;

public class MongoObjectSerializer : SerializerBase<object>
{
    public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        switch (context.Reader.CurrentBsonType)
        {
            case BsonType.EndOfDocument:
                context.Reader.ReadEndDocument();
                break;
            case BsonType.Double:
                return BsonSerializer.LookupSerializer(typeof(double)).Deserialize(context, args);
            case BsonType.String:
                return BsonSerializer.LookupSerializer(typeof(string)).Deserialize(context, args);
            case BsonType.Document:
                {
                    BsonDocument document = BsonSerializer.LookupSerializer<BsonDocument>().Deserialize(context, args);
                    var json = document.ToJson();
                    return JObject.Parse(json);
                }
            case BsonType.Array:
                {
                    var bson = BsonSerializer.LookupSerializer<BsonArray>().Deserialize(context, args);
                    var json = bson.ToJson();
                    return JArray.Parse(json);
                }
            case BsonType.Binary:
                return BsonSerializer.LookupSerializer(typeof(BsonBinaryData)).Deserialize(context, args);
            case BsonType.Undefined:
                return BsonSerializer.LookupSerializer(typeof(BsonUndefined)).Deserialize(context, args);
            case BsonType.ObjectId:
                return BsonSerializer.LookupSerializer(typeof(BsonObjectId)).Deserialize(context, args);
            case BsonType.Boolean:
                return BsonSerializer.LookupSerializer(typeof(bool)).Deserialize(context, args);
            case BsonType.DateTime:
                return BsonSerializer.LookupSerializer(typeof(DateTime)).Deserialize(context, args);
            case BsonType.Null:
                context.Reader.ReadNull();
                break;
            case BsonType.RegularExpression:
                return BsonSerializer.LookupSerializer(typeof(BsonRegularExpression)).Deserialize(context, args);
            case BsonType.JavaScript:
                return BsonSerializer.LookupSerializer(typeof(BsonJavaScript)).Deserialize(context, args);
            case BsonType.Symbol:
                return BsonSerializer.LookupSerializer(typeof(BsonSymbol)).Deserialize(context, args);
            case BsonType.JavaScriptWithScope:
                return BsonSerializer.LookupSerializer(typeof(BsonJavaScriptWithScope)).Deserialize(context, args);
            case BsonType.Int32:
                return BsonSerializer.LookupSerializer(typeof(int)).Deserialize(context, args);
            case BsonType.Timestamp:
                return BsonSerializer.LookupSerializer(typeof(BsonTimestamp)).Deserialize(context, args);
            case BsonType.Int64:
                return BsonSerializer.LookupSerializer(typeof(long)).Deserialize(context, args);
            case BsonType.Decimal128:
                return BsonSerializer.LookupSerializer(typeof(BsonDecimal128)).Deserialize(context, args);
            case BsonType.MinKey:
                return BsonSerializer.LookupSerializer(typeof(BsonMinKey)).Deserialize(context, args);
            case BsonType.MaxKey:
                return BsonSerializer.LookupSerializer(typeof(BsonMaxKey)).Deserialize(context, args);
            default:
                break;
        }

        return null;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        if (value == null)
        {
            context.Writer.WriteNull();
            return;
        }

        Type valueType = value.GetType();

        IBsonSerializer valueSerializer = BsonSerializer.LookupSerializer(valueType);

        valueSerializer.Serialize(context, args, value);
    }
}
