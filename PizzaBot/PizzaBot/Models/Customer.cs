using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }

        // public List<Order> PastOrders { get; set; }

    }
}
