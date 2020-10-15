using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class OrderItem<T> 
    {
        public T Type { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public class Pizza
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

        public enum SideItem
        {
            Breadsticks,
            Wings
        }

        public enum Drink
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
