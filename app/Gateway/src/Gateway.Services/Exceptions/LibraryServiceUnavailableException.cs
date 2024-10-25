using System.Runtime.Serialization;

namespace Gateway.Services.Exceptions
{
    [Serializable]
    public class LibraryServiceUnavailableException : Exception
    {
        public LibraryServiceUnavailableException() { }
        public LibraryServiceUnavailableException(string message) : base(message) { }
        public LibraryServiceUnavailableException(string message, Exception ex) : base(message, ex) { }
        protected LibraryServiceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
