using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class Pizza : OrderItem
    {
        public List<Topping> MyProperty { get; set; }
        public enum Size
        {
            Small,
            Medium,
            Large
        }

        public enum Crust
        {
            Thin,
            HandTossed,
            DeepDish
        }

        public enum Topping
        {
            Sausage,
            Pepperoni,
            Mushroom,
            Onion,
            GreenPepper,
            ExtraCheese
        }
    }
}
