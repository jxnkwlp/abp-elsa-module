using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.Common
{
    public class SimpleExceptionData
    {
        public SimpleExceptionData(string type, string message, string stackTrace, Dictionary<string, object> data)
        {
            Type = type;
            Message = message;
            StackTrace = stackTrace;
            Data = data;
        }

        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
