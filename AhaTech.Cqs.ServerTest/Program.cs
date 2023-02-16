using AhaTech.Cqs.AspnetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace AhaTech.Cqs.ServerTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCqs(typeof(Program).Assembly);
            
            var app = builder.Build();

            app.UseRouting();
            
            app.MapCqs();

            app.Run();
        }
    }
}