using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class OrderItem 
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }

        public override string ToString() 
        {
            return "Item: " + Name;
        }

    }
}
