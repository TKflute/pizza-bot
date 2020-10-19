using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaBot.Models
{
    public class SideItem : OrderItem
    {
        public ItemType Type { get; set; }
        public enum ItemType
        {
            Breadsticks,
            Wings
        }
    }
}
