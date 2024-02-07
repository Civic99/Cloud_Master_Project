using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class UserWithOrdersDto
    {
        public Guid UserId { get; set; }
        
        public List<OrderDto?> Orders { get; set; }
    }
}
