using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }

        public Customer(string name, string phone, Address newAddress)
        {
            Name = name;
            Phone = phone;
            Address = newAddress;
        }
        // public List<Order> PastOrders { get; set; }

    }
}
