using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIDemo.data;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Filters.ActionFilters
{
    public class Shirt_ValidateShirtIdFilterAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDbContext db;

        public Shirt_ValidateShirtIdFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var shirtId = context.ActionArguments["id"] as int?;
            if (shirtId.HasValue)
            {
                if (shirtId.Value <= 0)
                {
                    context.ModelState.AddModelError("ShirtId", "ShirtId is invalid.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }
                //else if (!ShirtRepository.ShirtExists(shirtId.Value))
                //{
                //    context.ModelState.AddModelError("ShirtId", "ShirtId doesn't exist.");
                //    var problemDetails = new ValidationProblemDetails(context.ModelState)
                //    {
                //        Status = StatusCodes.Status404NotFound
                //    };
                //    context.Result = new NotFoundObjectResult(problemDetails);
                //}


                // this is for implementing end points using EFCore 
                else
                {
                    var shirt = db.Shirts.Find(shirtId.Value);
                    if(shirt == null) {
                        context.ModelState.AddModelError("ShirtId", "ShirtId doesn't exist.");
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = StatusCodes.Status404NotFound
                        };
                        context.Result = new NotFoundObjectResult(problemDetails);
                    }
                    else
                    {
                        context.HttpContext.Items["shirt"] = shirt;
                    }
                }
            }
        }
    }
}
