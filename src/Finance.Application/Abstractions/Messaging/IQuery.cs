using Finance.Domain.Abstracts;
using MediatR;

namespace Finance.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
