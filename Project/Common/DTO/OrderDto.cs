using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class OrderDto
    {
        public Guid UserId { get; set; }
        public Dictionary<Guid, int> Products { get; set; }
        public OrderType OrderType { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public long TotalPrice { get; set; } = 0;
    }
}
