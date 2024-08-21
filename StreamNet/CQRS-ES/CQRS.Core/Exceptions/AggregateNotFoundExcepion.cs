namespace CQRS.Core.Exceptions;

public class AggregateNotFoundExcepion : Exception
{
    public AggregateNotFoundExcepion(string message) : base(message)
    {

    }
}
