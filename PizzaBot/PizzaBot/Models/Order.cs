using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class Order
    {
        public List<OrderItem<Object>> OrderItems { get; set; }  // TODO: not sure what to put for T
        public Customer Customer { get; set; }
    }
}
