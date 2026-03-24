
using APIServerLib.Data;
using APIServerLib.ProgramSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using System.Text.Json.Serialization;
using SharedLib.Fixed;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // This ignores properties with null values across the whole app
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

    // This ignores circular references (common in complex models)
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}); ;
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(conn ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

//builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


//cors
AllowCors.AddCorsPolicy(builder);
//string[] WebAppUrl = builder.Configuration.GetSection("CorsOrigins").Get<string[]>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowBlazorWasm",
//        policy => policy.WithOrigins(WebAppUrl!) // تأكد من مطابقة بورت الـ Client
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            .AllowCredentials());
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://man.runasp.net") // الرابط الخاص بك
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // إذا كنت تستخدم Authentication/Cookies
    });
});


//App Services
AddAppServices.AddAppServicesToContainer(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.MapOpenApi();
    app.MapScalarApiReference();
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    //app.MapScalarApiReference();
    //
    //using var scope = app.Services.CreateScope();
    //var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    ////dbcontext.Database.Migrate();

    //var Roleamanager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    //if(!await Roleamanager.RoleExistsAsync(Roles.Admin))
    //{
    //    await Roleamanager.CreateAsync(new IdentityRole(Roles.Admin));
    //}
    // if (!await Roleamanager.RoleExistsAsync(Roles.User))
    //{
    //    await Roleamanager.CreateAsync(new IdentityRole(Roles.User));
    //}
    // if (!await Roleamanager.RoleExistsAsync(Roles.Viewer))
    //{
    //    await Roleamanager.CreateAsync(new IdentityRole(Roles.Viewer));
    //}

}
AllowCors.UseCorsPolicy(app);
//app.UseCors("AllowBlazorWasm"); // أولاً السماح بالاتصال
app.UseHttpsRedirection();

app.MapIdentityApi<ApplicationUser>();
//app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
