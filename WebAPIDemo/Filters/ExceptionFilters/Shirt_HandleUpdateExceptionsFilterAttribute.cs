using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIDemo.data;
using WebAPIDemo.Models;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Filters.ExceptionFilters
{
    public class Shirt_HandleUpdateExceptionsFilterAttribute: ExceptionFilterAttribute
    {
        private readonly ApplicationDbContext db;

        public Shirt_HandleUpdateExceptionsFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;
        }
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            var strShirtId = context.RouteData.Values["id"] as string;
            if( int.TryParse(strShirtId, out int shirtId))
            {
                //if (!ShirtRepository.ShirtExists(shirtId))
                //{
                //    context.ModelState.AddModelError("ShirtId", "Shirt doesn't exist anymore.");
                //    var problemDetails = new ValidationProblemDetails(context.ModelState)
                //    {
                //        Status = StatusCodes.Status404NotFound
                //    };
                //    context.Result = new NotFoundObjectResult(problemDetails);
                //}
                if (db.Shirts.FirstOrDefault(x => x.ShirtId == shirtId) == null)
                {
                    context.ModelState.AddModelError("ShirtId", "Shirt doesn't exist anymore.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status404NotFound
                    };
                    context.Result = new NotFoundObjectResult(problemDetails);
                }

            }
        }
    }
}
