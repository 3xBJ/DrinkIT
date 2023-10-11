using DrinkIT.Products.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrinkIT.Products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrinksController : ControllerBase
    {
        private static readonly IReadOnlyList<Drink> Drinks = new List<Drink>(4)
        {
            new Drink(1, "Italian Coffee", 1.1M),
            new Drink(2, "American Coffee", 2.2M),
            new Drink(3, "Tea", 1.4M),
            new Drink(4, "Chocolate", 3M)
        };

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllDrinks()
        {
            return Ok(Drinks);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id)
        {
            Drink? selectedDrink = Drinks.FirstOrDefault(drink => drink.Id == id);

            return (selectedDrink is null) switch
            {
                true => NotFound(id),
                false => Ok(selectedDrink)
            };            
        }
    }
}
