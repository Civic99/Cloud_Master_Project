using Common.DTO;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mappers
{
    public static class OrderMapper
    {
        public static OrderDto ToDto(Order user)
        {
            var dto = new OrderDto();
            dto.Products = user.Products;
            dto.UserId = user.UserId;

            return dto;
        }

        public static Order FromDto(OrderDto dto)
        {
            var order = new Order();
            order.Id = Guid.NewGuid();
            order.Products = dto.Products;
            order.UserId = dto.UserId;

            return order;
        }
    }
}
