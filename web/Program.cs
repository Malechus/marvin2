var builder = WebApplication.CreateBuilder(args);

IConfigurationBuilder configBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json");

string env = "Development";
if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production"){ env = "Production"; }

configBuilder.AddJsonFile($"appsettings.{env}.json");

IConfigurationRoot config = configBuilder.Build();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
