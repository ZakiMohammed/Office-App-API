using OfficeApp.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Bson;

namespace OfficeApp.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly CustomerRepository _repo;

        CustomerController()
        {
            _repo = new CustomerRepository();
        }

        public IHttpActionResult Get()
        {
            return Ok(_repo.GetAll());
        }

        // GET api/customer/5
        public IHttpActionResult Get(string id)
        {
            return Ok(_repo.GetById(id));
        }

        // POST api/values
        public IHttpActionResult Post(Customer customer)
        {
            _repo.Add(customer);

            return Created(
                location: Request.RequestUri.AbsoluteUri + "/" + customer.ID, 
                content: customer
            );
        }

        // PUT api/values/5
        public IHttpActionResult Put(string id, Customer customer)
        {
            var oldCustomer = _repo.GetById(id);
            if (oldCustomer == null)
            {
                return NotFound();
            }

            customer.ID = oldCustomer.ID;
            _repo.Update(customer);

            return Ok();
        }

        // DELETE api/values/5
        public IHttpActionResult Delete(string id)
        {
            var customer = _repo.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            _repo.Delete(customer);
            return Ok();
        }
    }
}
