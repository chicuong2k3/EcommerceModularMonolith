namespace Ordering.Domain.CustomerAggregate;

public class Customer : AggregateRoot
{
    public Email Email { get; set; }

    public Customer(Email email)
    {
        Id = Guid.NewGuid();
    }
}
