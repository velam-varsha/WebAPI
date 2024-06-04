using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using WebAPIDemo.Attributes;
using WebAPIDemo.data;
using WebAPIDemo.Filters;
using WebAPIDemo.Filters.ActionFilters;
using WebAPIDemo.Filters.AuthFilters;
using WebAPIDemo.Filters.ExceptionFilters;
using WebAPIDemo.Models;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Controllers
{
    [ApiVersion("1.0")]   // used for triggering different versions using http
    [ApiController]


    // the below line should not be commented and the [Route("api/v{v:apiVersion}/[controller]")] line should be commented when you want to execute on microsoft edge and do changes in the microsoft edge
    [Route("api/[controller]")]  // this line is commented when we want to trigger different versions using url, coz this line triggers using http
    //[Route("api/v{v:apiVersion}/[controller]")]  // this we use when we want to trigger different versions through url (we should comment the line wch triggeres different version through http). In {v:apiVersion}, apiVersion will be substituted by the value written in [ApiVersion("1.0")] 

    // another method is versioning with query string : in this method we should comment [Route("api/v{v:apiVersion}/[controller]")] line and uncomment [Route("api/[controller]")] line. And in postman url, we should use this url: https://localhost:7103/api/shirts?api-version=2.0 for version 2 and https://localhost:7103/api/shirts?api-version=1.0  for version 1.

    [JwtTokenAuthFilter]
    public class ShirtsController: ControllerBase
    {
        private readonly ApplicationDbContext db;

        public ShirtsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        //private List<Shirt> shirts = new List<Shirt>()
        //{
        //    new Shirt { ShirtId = 1, Brand = "My Brand", Color = "Blue", Gender = "Men", Price = 30, Size = 10},
        //    new Shirt { ShirtId = 2, Brand = "My Brand", Color = "Black", Gender = "Men", Price = 35, Size = 12},
        //    new Shirt { ShirtId = 3, Brand = "Your Brand", Color = "Pink", Gender = "Women", Price = 28, Size = 8},
        //    new Shirt { ShirtId = 4, Brand = "Your Brand", Color = "Yellow", Gender = "Women", Price = 30, Size = 9}
        //};



        //[HttpGet]
        //[Route("/shirts")]   // specifying the url for GetShirt action method

        //[HttpGet]
        //public string GetShirts()
        //{
        //    return "Reading all the shirts";
        //}
        //[HttpGet]
        //public IActionResult GetShirts()
        //{
        //    return Ok("Reading all the shirts");
        //}

        //[HttpGet]
        //public IActionResult GetShirts()
        //{
        //    return Ok(ShirtRepository.GetShirts());
        //}

        [HttpGet]
        [RequiredClaim("read", "true")]
        public IActionResult GetShirts()
        {
            return Ok(db.Shirts.ToList());
        }




        //[HttpGet]
        //[Route("/shirts/{id}")]   // specifying the url for GetShirtById action method

        //[HttpGet("{id}/{color}")]
        //public string GetShirtById(int id, string color)
        //{
        //    return $"Reading shirt: {id}, color: {color}";
        //}

        // data from route
        //[HttpGet("{id}/{color}")]
        //public string GetShirtById(int id, [FromRoute] string color)
        //{
        //    return $"Reading shirt: {id}, color: {color}";
        //}

        // data from query
        //[HttpGet("{id}")]
        //public string GetShirtById(int id, [FromQuery] string color)  // give correct url in postman
        //{
        //    return $"Reading shirt: {id}, color: {color}";
        //}

        //// data from the header
        //[HttpGet("{id}")]
        //public string GetShirtById(int id, [FromHeader(Name = "Color")] string color)
        //{
        //    return $"Reading shirt: {id}, color: {color}";
        //}

        //[HttpGet("{id}")]
        //public Shirt? GetShirt(int shirtId)
        //{
        //    return shirts.First(x => x.ShirtId == id);
        //}

        //[HttpGet("{id}")]
        //public IActionResult GetShirtById(int id)  // in an action method that returns multiple types, the return type cannt be one specific type, so we should mention IActionResult as the return type 
        //{
        //    if(id <= 0)
        //    {
        //        return BadRequest();
        //    }
        //    var shirt = shirts.First(x => x.ShirtId == id);
        //    if(shirt == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(shirt);
        //}

        // IActionResult is used when the action method returns different types of data

        //[HttpGet("{id}")]
        //public IActionResult GetShirtById(int id)  // in an action method that returns multiple types, the return type cannt be one specific type, so we should mention IActionResult as the return type 
        //{
        //    if (id <= 0)
        //    {
        //        return BadRequest();
        //    }
        //    var shirt = ShirtRepository.GetShirtById(id);
        //    if (shirt == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(shirt);
        //}

        [HttpGet("{id}")]
        //[Shirt_ValidateShirtIdFilter]
        // for db.. if we want to use a constructor dependency injection here, tou cannot use attribute this way [Shirt_ValidateShirtIdFilter], so we need to use TypeFilter
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("read", "true")]
        public IActionResult GetShirtById(int id)  // in an action method that returns multiple types, the return type cannt be one specific type, so we should mention IActionResult as the return type 
        {
            //return Ok(ShirtRepository.GetShirtById(id));
            return Ok(HttpContext.Items["shirt"]);
        }






        //[HttpPost]
        //[Route("/shirts")]   // specifying the url for CreateShirt action method

        //[HttpPost]
        //public string CreateShirt()
        //{
        //    return $"Creating a shirt";
        //}


        //// to use complex data 2 ways: FromBody and FromForm
        //[HttpPost]
        //public string CreateShirt([FromBody]Shirt shirt)
        //{
        //    return $"Creating a shirt";
        //}

        // to use complex data 2 ways: FromBody and FromForm
        //[HttpPost]
        //public string CreateShirt([FromForm] Shirt shirt)
        //{
        //    return $"Creating a shirt";
        //}
        [HttpPost]
        //[Shirt_ValidateCreateShirtFilter]
        [TypeFilter(typeof(Shirt_ValidateCreateShirtFilterAttribute))]
        [RequiredClaim("write", "true")]
        public IActionResult CreateShirt([FromBody] Shirt shirt)
        {
            //if(shirt == null)
            //{
            //    return BadRequest();
            //}
            //var existingShirt = ShirtRepository.GetShirtByProperties(shirt.Brand, shirt.Gender, shirt.Color, shirt.Size);
            //if(existingShirt != null)
            //{
            //    return BadRequest();
            //}

            //ShirtRepository.AddShirt(shirt);
            this.db.Shirts.Add(shirt);
            this.db.SaveChanges();
            return CreatedAtAction(nameof(GetShirtById),
                new { id = shirt.ShirtId },
                shirt);
        }




        //[HttpPut]
        //[Route("/shirts/{id}")]   // specifying the url for UpdateShirt action method

        //[HttpPut("{id}")]
        //public string UpdateShirt(int id)
        //{
        //    return $"Updating shirt: {id}";
        //}
        //[HttpPut("{id}")]
        //public IActionResult UpdateShirt(int id)
        //{
        //    return Ok("Updating shirt: {id}");
        //}
        [HttpPut("{id}")]
        //[Shirt_ValidateShirtIdFilter]
        // for db.. if we want to use a constructor dependency injection here, tou cannot use attribute this way [Shirt_ValidateShirtIdFilter], so we need to use TypeFilter
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [Shirt_ValidateUpdateShirtFilter]
        [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
        [RequiredClaim("write", "true")]
        public IActionResult UpdateShirt(int id, Shirt shirt)
        {
            //ShirtRepository.UpdateShirt(shirt);
            var shirtToUpdate = HttpContext.Items["shirt"] as Shirt; // to retreive shirt from database
            shirtToUpdate.Brand = shirt.Brand;
            shirtToUpdate.Price = shirt.Price;
            shirtToUpdate.Size = shirt.Size;
            shirtToUpdate.Gender = shirt.Gender;
            shirtToUpdate.Color = shirt.Color;
            
            db.SaveChanges(); // the database will be updated with changes
            return NoContent();
        }





        //[HttpDelete]
        //[Route("/shirts/{id}")]   // specifying the url for DeleteShirt action method

        //[HttpDelete("{id}")]
        //public string DeleteShirt(int id)
        //{
        //    return $"Deleting shirt: {id}";
        //}
        [HttpDelete("{id}")]
        //[Shirt_ValidateShirtIdFilter]
        // for db.. if we want to use a constructor dependency injection here, tou cannot use attribute this way [Shirt_ValidateShirtIdFilter], so we need to use TypeFilter
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("delete", "true")]
        public IActionResult DeleteShirt(int id)
        {
            //var shirt = ShirtRepository.GetShirtById(id);
            //ShirtRepository.DeleteShirt(id);
            //return Ok(shirt);

            var shirtToDelete = HttpContext.Items["shirt"] as Shirt;  // to retreive shirt from database
            db.Shirts.Remove(shirtToDelete);  // deletes the shirt
            db.SaveChanges();
            return Ok(shirtToDelete);
        }
    }
}
