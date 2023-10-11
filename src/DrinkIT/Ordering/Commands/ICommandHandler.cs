using DrinkIT.BaseClasses;
using MediatR;

namespace DrinkIT.Ordering.Commands
{
    public interface ICommandHandler<CommandType> : IRequestHandler<CommandType, CommandResult> where CommandType : Command<CommandResult>
    {
    }
}
