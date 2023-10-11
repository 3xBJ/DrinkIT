using DrinkIT.BaseClasses;
using DrinkIT.Ordering.DTO;

namespace DrinkIT.Ordering.Commands
{
    public class CreateOrder : Command<CommandResult>
    {
        public List<DrinkDTO> Drinks { get; set; } = new();
    }
}
