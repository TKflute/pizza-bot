using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class Drink : OrderItem
    {
        public SodaType Type { get; set; }
        public enum SodaType
        {
            Pepsi,
            DietPepsi,
            Coke,
            DietCoke,
            RootBeer,
            Sprite
        }
    }
}
