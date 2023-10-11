using DrinkIT.Ordering.Commands;
using MediatR;

namespace DrinkIT.BaseClasses
{
    public class Command<ResultType> : IRequest<ResultType>
    {
        public string Id { get; } = Guid.NewGuid().ToString();
    }
}
