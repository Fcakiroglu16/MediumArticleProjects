namespace Shared.Events;

public record PaymentCompletedEvent(Guid OrderId, long PaymentId);