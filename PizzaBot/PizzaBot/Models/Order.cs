using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class Order
    {
        public OrderType Type { get; set; }
        public List<OrderItem> OrderItems { get; set; }  // TODO: not sure what to put for T
        public Customer Customer { get; set; }
        public enum OrderType
        {
            Pickup,
            Delivery
        }

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }
    }
}
