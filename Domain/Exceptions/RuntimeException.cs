namespace Domain.Exceptions;

public sealed class RuntimeException : Exception
{
    public RuntimeException()
    {
    }

    public RuntimeException(string? message): base(message)
    {
    }
}