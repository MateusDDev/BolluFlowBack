namespace BolluFlow.Domain.Abstractions;

public sealed class DomainException(DomainError error) : Exception(error.Message)
{
    public DomainError Error { get; } = error;
}