// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CoreBot.Database;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class Program
    {
        public static Database database;

        public static void Main(string[] args)
        {
            database = new Database();
            database.addItem(new Book("Book1", "William Shakespeare", 200));
            database.addItem(new Book("Book2", "William Shakespeare", 250));
            database.addItem(new Book("Book3", "Penny Jordan", 300));
            database.addItem(new Book("Book4", "Rex Stout", 250));

            database.addItem(new Car("Ferrari", "Red", 405));
            database.addItem(new Car("Mercedes", "Black", 200));
            database.addItem(new Car("Toyota", "Grey", 180));
            database.addItem(new Car("Peugeot", "Yellow", 110));
            database.addItem(new Car("Citroen", "Black", 110));

            database.addItem(new Food("Steak", "main", 600));
            database.addItem(new Food("Gulash", "main", 450));
            database.addItem(new Food("Tiramisu", "dessert", 300));

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((logging) =>
                {
                    logging.AddDebug();
                    logging.AddConsole();
                })
                .UseStartup<Startup>();
    }
}
