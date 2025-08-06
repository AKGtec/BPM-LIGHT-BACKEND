namespace ProjectTemplate.Models.Exceptions;

public abstract class BadRequestException : Exception
{
    protected BadRequestException(string message) : base(message)
    {
    }
}

public sealed class EntityNotFoundException : BadRequestException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"The {entityName} with id: {id} doesn't exist in the database.")
    {
    }
}

public sealed class ValidationException : BadRequestException
{
    public ValidationException(string message) : base(message)
    {
    }
}
