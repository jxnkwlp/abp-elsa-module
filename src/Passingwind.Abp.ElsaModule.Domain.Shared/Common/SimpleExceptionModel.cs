using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Elsa.Models;

namespace Passingwind.Abp.ElsaModule.Common;

public class SimpleExceptionModel : IEquatable<SimpleExceptionModel>
{
    public SimpleExceptionModel(Type type, string message, string stackTrace, IDictionary data, SimpleExceptionModel innerException = null)
    {
        Type = type;
        Message = message;
        StackTrace = stackTrace;
        Data = data;
        InnerException = innerException;
    }

    public Type Type { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public IDictionary Data { get; set; }
    public SimpleExceptionModel InnerException { get; set; }

    public static SimpleExceptionModel FromException(SimpleException ex)
    {
        if (ex == null)
        {
            return null;
        }

        Type type = ex!.GetType();
        var ex2 = new SimpleExceptionModel(type, ex!.Message, ex!.StackTrace, ex!.Data);
        if (ex!.InnerException != null)
        {
            ex2.InnerException = FromException(ex!.InnerException);
        }

        return ex2;
    }

    public SimpleException ToException()
    {
        return new SimpleException(Type, Message, StackTrace, Data, InnerException?.ToException());
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as SimpleExceptionModel);
    }

    public bool Equals(SimpleExceptionModel other)
    {
        return other is not null &&
               EqualityComparer<Type>.Default.Equals(Type, other.Type) &&
               Message == other.Message &&
               StackTrace == other.StackTrace &&
               JsonSerializer.Serialize(Data) == JsonSerializer.Serialize(other.Data);
    }

    public override int GetHashCode()
    {
        int hashCode = -291789878;
        hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(Type);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StackTrace);
        hashCode = hashCode * -1521134295 + EqualityComparer<IDictionary>.Default.GetHashCode(Data);
        return hashCode;
    }

    public static bool operator ==(SimpleExceptionModel left, SimpleExceptionModel right)
    {
        return EqualityComparer<SimpleExceptionModel>.Default.Equals(left, right);
    }

    public static bool operator !=(SimpleExceptionModel left, SimpleExceptionModel right)
    {
        return !(left == right);
    }
}
