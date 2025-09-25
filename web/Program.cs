using data.Services;

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
builder.Services.AddSingleton<IPiService, PiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
