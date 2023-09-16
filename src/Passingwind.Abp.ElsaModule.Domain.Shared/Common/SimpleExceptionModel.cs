using System;
using System.Collections;
using Elsa.Models;

namespace Passingwind.Abp.ElsaModule.Common;

public class SimpleExceptionModel
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
}
