using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeleteFromGridFSTest.Data
{
    public class Contact
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string FirstName { get; set; }

        public string LastName { get; set; } 

        public string PhoneNumber { get; set; }
    }
}
