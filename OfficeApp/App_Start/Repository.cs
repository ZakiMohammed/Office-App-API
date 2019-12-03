using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Configuration;
using MongoDB.Bson;

namespace OfficeApp.App_Start
{
    public class CustomerRepository
    {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<Customer> _collection;

        public CustomerRepository()
        {
            _client = new MongoClient(Constants.MONGO_CONNECTION_STRING);
            _database = _client.GetDatabase(Constants.MONGO_DATABASE);
            _collection = _database.GetCollection<Customer>(nameof(Customer));
        }

        public Customer GetById(string id)
        {
            var customer = _collection.Find(i => i.ID == ObjectId.Parse(id));
            return customer.FirstOrDefault();
        }

        public IEnumerable<Customer> GetAll()
        {
            var list = _collection.Find(i => true).ToList();
            return list;
        }

        public void Add(Customer Customer)
        {
            _collection.InsertOne(Customer);
        }

        public void Update(Customer Customer)
        {
            var filter = Builders<Customer>.Filter.Eq("_id", Customer.ID);
            var update = Builders<Customer>.Update
                .Set("firstName", Customer.FirstName)
                .Set("lastName", Customer.LastName)
                .Set("gender", Customer.Gender)
                .Set("dob", Customer.DOB)
                .Set("email", Customer.Email)
                .Set("mobile", Customer.Mobile);

            _collection.UpdateOne(filter, update);
        }

        public void Delete(Customer Customer)
        {
            var filter = Builders<Customer>.Filter.Eq("_id", Customer.ID);

            _collection.DeleteOne(filter);
        }
    }
}