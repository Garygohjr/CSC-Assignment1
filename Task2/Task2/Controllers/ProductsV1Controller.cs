using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Task2.Models;

namespace Task2.Controllers
{
    public class ProductsV1Controller : ApiController
    {
        public class ProductsController : ApiController
        {

            static readonly IProductRepository repository = new ProductRepository();

            [HttpGet]
            [Route("api/v1/products")]
            public IEnumerable<Product> GetAllProductsFromRepository()
            {
                return repository.GetAll();

            }

            [HttpGet]
            [Route("api/v1/products/{id:int:min(2)}", Name = "getProductByIdv1")]

            public Product retrieveProductfromRepository(int id)
            {
                Product item = repository.Get(id);
                if (item == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return item;
            }

            [HttpGet]
            [Route("api/v1/products", Name = "getProductByCategoryv1")]
            //http://localhost:9000/api/v1/products?category=

            public IEnumerable<Product> GetProductsByCategory(string category)
            {
                return repository.GetAll().Where(
                    p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
            }

            [HttpPost]
            [Route("api/v1/products")]
            public HttpResponseMessage PostProduct(Product item)
            {
                if (ModelState.IsValid)
                {
                    item = repository.Add(item);
                    var response = Request.CreateResponse<Product>(HttpStatusCode.Created, item);

                    // Generate a link to the new product and set the Location header in the response.

                    string uri = Url.Link("getProductByIdv1", new { id = item.Id });
                    response.Headers.Location = new Uri(uri);
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }

            [HttpPut]
            [Route("api/v1/products/{id:int}")]
            public Product PutProduct(int id, Product product)
            {
                product.Id = id;
                if (!repository.Update(product))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return product;
            }

            [HttpDelete]
            [Route("api/v1/products/{id:int}")]
            public string DeleteProduct(int id)
            {
                if (repository.Remove(id) != true)
                {
                    return "Delete unsuccessful";
                }
                return "Delete successful";
            }
        }
    }
}
