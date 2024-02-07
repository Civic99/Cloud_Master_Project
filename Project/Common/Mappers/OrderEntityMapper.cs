using Common.Entities;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mappers
{
    public static class OrderEntityMapper
    {
        public static OrderEntity ToEntity(Order order)
        {
            var entity = new OrderEntity();
            entity.PartitionKey = "Orders";
            entity.RowKey = order.Id.ToString();
            entity.OrderStatus = order.OrderStatus;
            entity.TotalPrice = order.TotalPrice;

            entity.OrderType = order.OrderType;
            entity.Id = order.Id;
            entity.UserId = order.UserId;
            if(order.Products != null)
            {
                entity.Products = string.Join(",", order.Products.Select(kv => $"{kv.Key}:{kv.Value}"));
            }
            

            return entity;
        }

        public static Order FromEntity(OrderEntity entity)
        {
            var order = new Order();
            order.OrderStatus = entity.OrderStatus;
            order.TotalPrice = entity.TotalPrice;
            order.OrderType = entity.OrderType;
            order.Id = entity.Id;
            order.UserId = entity.UserId;
            if(entity.Products != null)
            {
                order.Products = entity.Products
                                    .Split(',')
                                    .Select(pair =>
                                    {
                                        var parts = pair.Split(':');
                                        return new KeyValuePair<Guid, int>(Guid.Parse(parts[0]), int.Parse(parts[1]));
                                    })
                                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            
            return order;
        }
    }
}
