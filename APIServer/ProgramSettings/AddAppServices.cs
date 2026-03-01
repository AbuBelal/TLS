using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using SharedLib.Entities;
using SharedLib.Repositories;


namespace APIServerLib.ProgramSettings
{
    public static class AddAppServices
    {
        public static WebApplicationBuilder AddAppServicesToContainer(this WebApplicationBuilder builder)
        {
            // إضافة خدمات أخرى هنا إذا لزم الأمر
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<ICenterRepository, CenterRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IEmpCenterRepository, EmpCenterRepository>();
            builder.Services.AddScoped<IStdCenterRepository, StdCenterRepository>();
            builder.Services.AddScoped<ILookupValueRepository, LookupValueRepository>();

            return builder;
        }
    }
}
