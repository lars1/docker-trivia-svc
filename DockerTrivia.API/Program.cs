
using DockerTrivia.API.Features;
using DockerTrivia.API.Features.Questions;
using DockerTrivia.API.Features.Questions.Remote;

namespace DockerTrivia.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<TriviaApiOptions>(
                builder.Configuration.GetSection(TriviaApiOptions.TriviaApi));

            builder.Services.AddHttpClient<ITriviaApiService, TriviaApiService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            // For simplicity, let's use swagger even in production
            app.UseSwagger();
            app.UseSwaggerUI();

            //app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
