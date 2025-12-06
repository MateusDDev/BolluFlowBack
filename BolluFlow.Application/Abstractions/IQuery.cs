using MediatR;

namespace BolluFlow.Application.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>;