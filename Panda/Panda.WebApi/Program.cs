using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Panda.Repository;
using Panda.Repository.EntityFramework;
using Panda.Services;
using Panda.Services.Mappers;
using Panda.WebApi.Validators;

namespace Panda.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                options =>
                {
                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<PatientValidator>();
            builder.Services.AddScoped<NhsNumberValidator>();
            builder.Services.AddScoped<PatientMapper>();
            builder.Services.AddScoped<AppointmentMapper>();
            builder.Services.AddScoped<MissedAppointmentReportRequestValidator>();

            builder.Services.AddDbContext<PandaDbContext>(opt => opt.UseInMemoryDatabase("PandaDb"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}