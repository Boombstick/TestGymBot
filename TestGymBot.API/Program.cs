
using Microsoft.EntityFrameworkCore;
using TestGymBot.Application.Services;
using TestGymBot.DataAccess;
using TestGymBot.DataAccess.Repositories;
using TestGymBot.Domain.Abstractions.Repositories;
using TestGymBot.Domain.Abstractions.Services;

namespace TestGymBot.API
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
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<TgBotDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(TgBotDbContext)));
            });

            builder.Services.AddScoped<IPersonPropsRepository, PersonPropsRepository>();

            builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
            builder.Services.AddScoped<IPersonsService, PersonsService>();

            builder.Services.AddScoped<IRecordsRepository, RecordsRepository>();
            builder.Services.AddScoped<IRecordsService, RecordsService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
