using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MyCourse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // .ConfigureLogging(logging =>
                // {
                //     logging.ClearProviders(); // Rimuove i provider predefiniti se vuoi ricominciare da zero
                //     logging.AddConsole(); // Aggiunge il provider per la console
                // })
                .UseStartup<Startup>();
    }
}
