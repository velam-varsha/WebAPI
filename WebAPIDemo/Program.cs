using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebAPIDemo.data;
using WebAPIDemo.Filters.OperationFilter;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShirtStoreManagement"));  // this is how we can get access to the contents to appsetting file 
});  // this is an extension method which helps us to dependency inject 

// Chat gpt link: https://chat.openai.com/share/4dd80210-f04e-4d8c-8f58-436a612ce4b3


// Add services to the container.
builder.Services.AddControllers();

// both the versions have the same url, so while running on postman, it will not know wch to run 1st. so here we will specify the order in wch the version will be run
builder.Services.AddApiVersioning(options =>
{
    // we cover how to trigger different versions through URL
    options.ReportApiVersions = true;  // lets enable reporting of the version
    options.AssumeDefaultVersionWhenUnspecified = true;  // run default version when version is not specified
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);  // here setting version 1 as default version and mentioning [ApiVersion ("1.0"] in the shirtcolntroller and authoritycontroller files of version 1
    //options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");  // HeaderApiVersionReader means that we are going to read the api version from the header. Here we covered how to specify the version through the HTTP header. this line is commented when we want to trigger different versions using url
});

//(SWAGGER) so now we need to dependency inject and configure our swagger middleware
// builder.Services.AddEndpointsApiExplorer();  // this will help us discover the endpoints  // this was used before adding version 2

builder.Services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");   // to understand this 'v'VVV read documentation about version format in aspnet-api-versioning git repo under wiki

// helps us to generate document and also the user interface
builder.Services.AddSwaggerGen( c =>            // here c represents configuration
{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Web API v1", Version = "version 1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "My Web API v2", Version = "version 2" });

    // we need to tell swagger that each endpoint requires a authorization token. its a requirement. the way to do that is:
    c.OperationFilter<AuthorizationHeaderOperationFilter>();
    // here we define the scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

}); 

var app = builder.Build();


// minimal apis ... url shud be provided while maping only
//// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//// Routing
//// "/shirts"
//app.MapGet("/shirts", () =>
//{
//    return "Reading all the shirts";
//});

//app.MapGet("/shirts/{id}", (int id) =>
//{
//    return $"Reading shirt with ID: {id}";
//});

//app.MapPost("/shirts", () =>
//{
//    return "Creating a shirt.";
//});

//app.MapPut("/shirts/{id}", (int id) =>
//{
//    return $"Updating shirt with ID: {id}";
//});

//app.MapDelete("/shirts/{id}", (int id) =>
//{
//    return $"Deleting shirt with ID: {id}";
//});


// Configure the HTTP request pipeline.
// Here we need to add the middleware in order to support the swagger documention. So we need to add it at the beginning of the middleware pipeline.
if (app.Environment.IsDevelopment())  // So we only need to add the swagger document support in the development environment to help developers. SO here we are actually using 2 middlewares.
{
    app.UseSwagger();  // which will help us to discover the API endpoints and also with documenting
    app.UseSwaggerUI(  // this provides the user interface
        options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "WebAPI v2");
        }); 
    // so now we need to dependency inject and configure our swagger middleware so mention above.
}

app.UseHttpsRedirection();
app.MapControllers(); // maps the http request to the controller, url is provided in the controller class

app.Run();

