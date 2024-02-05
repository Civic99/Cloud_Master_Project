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
        public IEnumerable<Guid> Products { get; set; } 
    }
}
