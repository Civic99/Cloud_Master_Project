using System.Runtime.Serialization;


namespace Common.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public List<Guid> Orders { get; set; }
    }
}
