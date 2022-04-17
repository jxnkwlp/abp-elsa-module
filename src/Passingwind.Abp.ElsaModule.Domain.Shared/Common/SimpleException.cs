using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class SimpleException : Exception
    {
        private readonly SimpleExceptionData _data;

        public SimpleException(SimpleExceptionData data)
        {
            _data = data;
        }

        public SimpleException(Exception exception)
        {
            _data = new SimpleExceptionData(exception.GetType().FullName, exception.Message, exception.StackTrace, (Dictionary<string, object>)exception.Data);
        }

        public SimpleException(string message) : base(message)
        {
        }

        public SimpleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SimpleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message => _data.Message;
        public override string StackTrace => _data.StackTrace;
        public override IDictionary Data => _data.Data;
    }
}
