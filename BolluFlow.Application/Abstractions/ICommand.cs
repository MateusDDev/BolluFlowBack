using MediatR;

namespace BolluFlow.Application.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>;