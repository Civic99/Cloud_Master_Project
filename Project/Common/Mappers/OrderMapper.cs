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
        public static OrderDto ToDto(Order order)
        {
            var dto = new OrderDto();
            dto.Products = order.Products;
            dto.UserId = order.UserId;
            dto.OrderType = order.OrderType;
            dto.OrderStatus= order.OrderStatus;
            order.TotalPrice = order.TotalPrice;

            return dto;
        }

        public static Order FromDto(OrderDto dto)
        {
            var order = new Order();
            order.Id = Guid.NewGuid();
            order.Products = dto.Products;
            order.UserId = dto.UserId;
            order.OrderType = dto.OrderType;
            order.OrderStatus = dto.OrderStatus;
            order.TotalPrice = dto.TotalPrice;

            return order;
        }
    }
}
