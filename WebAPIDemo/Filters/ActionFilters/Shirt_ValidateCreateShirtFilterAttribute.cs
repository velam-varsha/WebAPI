﻿using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIDemo.Models.Repositories;
using WebAPIDemo.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPIDemo.data;
using System.Drawing;
using System.Reflection;

namespace WebAPIDemo.Filters.ActionFilters
{
    /// <summary>
    /// Class Shirt_ValidateCreateShirtFilterAttribute
    /// </summary>
    public class Shirt_ValidateCreateShirtFilterAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDbContext db;

        public Shirt_ValidateCreateShirtFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var shirt = context.ActionArguments["shirt"] as Shirt;
            if (shirt == null)
            {
                context.ModelState.AddModelError("Shirt", "Shirt object is null.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
            }
            //else
            //{
            //    var existingShirt = ShirtRepository.GetShirtByProperties(shirt.Brand, shirt.Gender, shirt.Color, shirt.Size);
            //    if (existingShirt != null)
            //    {
            //        context.ModelState.AddModelError("Shirt", "Shirt already exist.");
            //        var problemDetails = new ValidationProblemDetails(context.ModelState)
            //        {
            //            Status = StatusCodes.Status400BadRequest
            //        };
            //        context.Result = new BadRequestObjectResult(problemDetails);

            //    }
            //}

            // this is for db implementation of endpoints using EFCore
            else
            {
                var existingShirt = db.Shirts.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(shirt.Brand) &&
                    !string.IsNullOrWhiteSpace(x.Brand) &&
                   // x.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&     cannont convert equals to sql query so we should use tolower or toupper and do the query
                    x.Brand.ToLower() == shirt.Brand.ToLower() &&
                    !string.IsNullOrWhiteSpace(shirt.Gender) &&
                    !string.IsNullOrWhiteSpace(x.Gender) &&
                    x.Gender.ToLower() == shirt.Gender.ToLower() &&
                    !string.IsNullOrWhiteSpace(shirt.Color) &&
                    !string.IsNullOrWhiteSpace(x.Color) &&
                    x.Color.ToLower() == shirt.Color.ToLower() &&
                    shirt.Size.HasValue &&
                    x.Size.HasValue &&
                    shirt.Size.Value == x.Size.Value);

                if (existingShirt != null)
                {
                    context.ModelState.AddModelError("Shirt", "Shirt already exist.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);

                }
            }

        }
    }
}
