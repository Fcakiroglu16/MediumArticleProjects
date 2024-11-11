namespace PaymentMicroservice.API.Repositories;

public class Payment
{
    public long Id { get; set; }

    public Guid OrderId { get; set; }
    public PaymentStatus Status { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}