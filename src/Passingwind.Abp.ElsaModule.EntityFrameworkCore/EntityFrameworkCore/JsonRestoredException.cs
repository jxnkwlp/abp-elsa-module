//using System;

//namespace Abp.ElsaManagement.Internal
//{
//    internal class JsonRestoreException : Exception
//    {
//        private readonly string _stackTrace;

//        public JsonRestoreException(string message, string stackTrace) : base(message)
//        {
//            _stackTrace = stackTrace;
//        }

//        public JsonRestoreException() : base()
//        {
//        }

//        public JsonRestoreException(string message) : base(message)
//        {
//        }

//        public JsonRestoreException(string message, Exception innerException) : base(message, innerException)
//        {
//        }

//        public override string StackTrace => _stackTrace;
//    }
//}