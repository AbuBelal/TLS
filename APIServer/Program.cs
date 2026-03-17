
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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

//builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


//cors
AllowCors.AddCorsPolicy(builder);

//App Services
AddAppServices.AddAppServicesToContainer(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    //
    using var scope = app.Services.CreateScope();
    var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //dbcontext.Database.Migrate();

    var Roleamanager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if(!await Roleamanager.RoleExistsAsync(Roles.Admin))
    {
        await Roleamanager.CreateAsync(new IdentityRole(Roles.Admin));
    }
     if (!await Roleamanager.RoleExistsAsync(Roles.User))
    {
        await Roleamanager.CreateAsync(new IdentityRole(Roles.User));
    }
     if (!await Roleamanager.RoleExistsAsync(Roles.Viewer))
    {
        await Roleamanager.CreateAsync(new IdentityRole(Roles.Viewer));
    }

}
AllowCors.UseCorsPolicy(app);

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.Run();
