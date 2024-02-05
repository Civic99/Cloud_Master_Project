using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Category { get; set; }
        [DataMember]
        public long Price { get; set; } = 0;
        [DataMember]
        public int Quantity { get; set; } = 0;
    }
}
