using System.Linq;
using AhaTech.Cqs.AspnetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AhaTech.Cqs.ServerTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCqs(typeof(Program).Assembly);
            builder.Services.AddSwaggerGen(c => c.ResolveConflictingActions(descs => descs.First()));
            var app = builder.Build();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapCqs();

            app.Run();
        }
    }
}