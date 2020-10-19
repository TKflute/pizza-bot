using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class Pizza : OrderItem
    {
        public List<Topping> Toppings { get; set; }
        public PizzaSize Size { get; set; }
        public PizzaCrust Crust { get; set; }

        public enum PizzaSize
        {
            Small,
            Medium,
            Large
        }

        public enum PizzaCrust
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

        public Pizza()
        {
            Toppings = new List<Topping>();
        }
    }
}
