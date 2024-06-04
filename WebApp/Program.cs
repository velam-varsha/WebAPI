using WebApp.Data;


// chat gpt link: https://chat.openai.com/share/4dd80210-f04e-4d8c-8f58-436a612ce4b3

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("ShirtsApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7127/api/");  // this Uri will be the base address of our webapi
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});  // this is an extension method which helps us to dependency inject the http client factory into our service collection

builder.Services.AddHttpClient("AuthorityApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7127/");  // this Uri will be the base address of our webapi
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(5);
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();


builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IWebApiExecuter, WebApiExecuter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();  // its a middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
