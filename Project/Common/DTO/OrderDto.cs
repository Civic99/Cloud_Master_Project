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
        public IEnumerable<Guid> Products { get; set; }
    }
}
