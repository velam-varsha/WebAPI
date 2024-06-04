using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using WebApp.Data;
using WebApp.Models;


// chat gpt link: https://chat.openai.com/share/4dd80210-f04e-4d8c-8f58-436a612ce4b3
namespace WebApp.Controllers
{
    public class ShirtsController : Controller
    {
        private readonly IWebApiExecuter webApiExecuter;

        public ShirtsController(IWebApiExecuter webApiExecuter)
        {
            this.webApiExecuter = webApiExecuter;
        }
        public async Task<IActionResult> Index()
        {
            return View(await webApiExecuter.InvokeGet<List<Shirt>>("shirts"));
            //try
            //{
            //    var shirts = await webApiExecuter.InvokeGet<List<Shirt>>("shirts");
            //    return View(shirts);
            //}
        }

        public IActionResult CreateShirt()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShirt(Shirt shirt)
        {
            //           if (ModelState.IsValid)
            //           {
            //               var response = await webApiExecuter.InvokePost("shirts", shirt);  // The first parameter "shirts" represents the relative URL or endpoint of the API where the request will be sent. In this case, it's likely the endpoint for creating a new shirt entity.
            //                                                                                 // The second parameter shirt is the data that will be sent with the POST request. It represents the shirt entity that the user wants to create.
            //               /*The await keyword is used to asynchronously wait for the completion of the InvokePost method.
            //By using await, the execution of the current method (CreateShirt(Shirt shirt)) is paused until the InvokePost method finishes executing and a response is received from the API.
            //                */
            //               if (response != null)
            //               {
            //                   return RedirectToAction(nameof(Index));
            //               }
            //           }

            if (ModelState.IsValid)
            {
                try
                {
                    var response = await webApiExecuter.InvokePost("shirts", shirt);
                    if(response != null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch(WebApiException ex)
                {
                    HandleWebApiException(ex);
                }
            }


            return View(shirt);
        }

        public async Task<IActionResult> UpdateShirt(int shirtId)
        {
            //var shirt = await webApiExecuter.InvokeGet<Shirt>($"shirts/{shirtId}");
            //if(shirt != null)
            //{
            //    return View(shirt);
            //}
            //return NotFound();
            try
            {
                var shirt = await webApiExecuter.InvokeGet<Shirt>($"shirts/{shirtId}");
                if (shirt != null)
                {
                    return View(shirt);
                }
            }
            catch(WebApiException ex)
            {
                HandleWebApiException(ex);
                return View();
            }
            return NotFound();
        }

        [HttpPost]  // in MVC there is no [HttpPut], its only [HttpPost] and [HttpGet]
        public async Task<IActionResult> UpdateShirt(Shirt shirt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await webApiExecuter.InvokePut($"shirts/{shirt.ShirtId}", shirt);
                    return RedirectToAction(nameof(Index));
                }
                catch(WebApiException ex)
                {
                    HandleWebApiException(ex);
                }
                
            }
            return View(shirt);
        }

        [HttpGet]  // in MVC there is no [HttpPut], its only [HttpPost] and [HttpGet]
        public async Task<IActionResult> DeleteShirt(int shirtId)
        {
            try
            {
                await webApiExecuter.InvokeDelete($"shirts/{shirtId}");
                return RedirectToAction(nameof(Index));
            }
            catch(WebApiException ex)
            {
                HandleWebApiException(ex);
                return View(nameof(Index), await webApiExecuter.InvokeGet<List<Shirt>>("shirts"));
            }
        }

        private void HandleWebApiException(WebApiException ex)
        {
            if (ex.ErrorResponse != null &&
                        ex.ErrorResponse.Errors != null &&
                        ex.ErrorResponse.Errors.Count > 0)
            {
                foreach (var error in ex.ErrorResponse.Errors)
                {
                    ModelState.AddModelError(error.Key, string.Join("; ", error.Value));
                }
            }
            else if(ex.ErrorResponse != null)
            {
                ModelState.AddModelError("Error", ex.ErrorResponse.Title);
            }
            else
            {
                ModelState.AddModelError("Error", ex.Message);
            }
        }
    }
}
