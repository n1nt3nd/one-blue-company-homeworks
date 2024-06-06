using System.Net.Sockets;

namespace Core.Exceptions;

public class IncorrectDataException : ArgumentException
{
    public IncorrectDataException(string? exception) : base(exception)
    {
    }
    
    public IncorrectDataException(string? exception, string? paramName) : base(exception, paramName)
    {
    }
}