using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OfficeApp.App_Start
{
    public class Customer
    {
        [BsonId]
        [BsonElement("_id")]
        public BsonObjectId ID { get; set; }
        [BsonElement("firstName")]
        public string FirstName { get; set; }
        [BsonElement("lastName")]
        public string LastName { get; set; }
        [BsonElement("gender")]
        public string Gender { get; set; }
        [BsonElement("dob")]
        public DateTime? DOB { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("mobile")]
        public string Mobile { get; set; }
    }
}