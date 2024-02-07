using Common.DTO;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mappers
{
    public class UserWithOrdersMapper
    {
        public static UserWithOrdersDto ToDto(Order order)
        {
            var dto = new UserWithOrdersDto();

            return dto;
        }
    }
}
