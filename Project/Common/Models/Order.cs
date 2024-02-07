using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Dictionary<Guid, int> Products { get; set; }

        [DataMember]
        public OrderType OrderType { get; set; }

        [DataMember]
        public OrderStatus OrderStatus { get; set; }

        [DataMember]
        public long TotalPrice { get; set; } = 0;
    }
}
