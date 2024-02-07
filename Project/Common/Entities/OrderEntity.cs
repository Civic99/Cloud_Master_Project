using Azure;
using Azure.Data.Tables;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class OrderEntity : ITableEntity
    {
        DateTimeOffset? ITableEntity.Timestamp { get; set; }
        ETag ITableEntity.ETag { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Products { get; set; }
        public OrderType OrderType { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public long TotalPrice { get; set; } = 0;
    }
}
