using System.Runtime.Serialization;

namespace TestApp;

[Serializable]
public class InvalidInputException : Exception
{
    public InvalidInputException() { }
    public InvalidInputException(string message) : base(message) { }
    public InvalidInputException(string message, Exception inner) : base(message, inner) { }

    protected InvalidInputException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
