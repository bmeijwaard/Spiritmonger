using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtgApiManager.Lib.Service;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Services;
using Spiritmonger.Mapping.Modules;
using Spiritmonger.Mapping.Profiles;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spiritmonger
{
    public class Program
    {
        private static CardService _cardApi;
        private static ICardService _cardService;
        private static ICardNameService _cardNameService;

        static Program()
        {
            _cardApi = new CardService();


            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            var config = builder.Build();
            IServiceCollection services = new ServiceCollection();
            services = CoreConfiguration.Load(services, config);
            services = CoreModule.Load(services, config);
            var serviceProvider = services.BuildServiceProvider();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CoreProfile>();
            });

            _cardService = serviceProvider.GetService<ICardService>();
            _cardNameService = serviceProvider.GetService<ICardNameService>();
        }

        public static async Task Main()
        {
            try
            {
                var currentPage = 1;
                var totalPages = 0;

                do
                {
                    Console.WriteLine($"Starting page {currentPage} / {totalPages}");

                    var cards = await _cardApi
                        .Where(q => q.Page, currentPage)
                        .Where(q => q.GameFormat, "Legacy")
                        .AllAsync().ConfigureAwait(false);

                    if (!cards.IsSuccess)
                        throw cards.Exception;

                    totalPages = cards.PagingInfo.TotalPages;

                    cards.Value.Where(card => card.MultiverseId != null).ToList().ForEach(async card =>
                    {
                        try
                        {
                            var result = await _cardService.CreateAsync(new CardDTO
                            {
                                Name = card.Name,
                                Expansion = card.SetName,
                                MultiverseId = card.MultiverseId != null ? (int)card.MultiverseId : 0,
                                ImageUrl = card.ImageUrl.ToString()
                            }).ConfigureAwait(false);

                            if (!result.Succeeded)
                                throw new InvalidOperationException(result.Error);

                            Console.WriteLine($"Success: {card.Name}, {card.SetName}, {card.MultiverseId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"-- Fail: {card.Name}, {card.SetName}, {card.MultiverseId}, Error: {ex.Message}");
                        }
                    });

                    currentPage++;
                    Thread.Sleep(800);
                } while (totalPages > 0 && currentPage <= (totalPages + 1));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Done! Press 'any' key to exit.");
                Console.ReadKey(true);
            }
        }
    }

    public class Card
    {
        public Card()
        {
        }

        public Card(string name, string expansion, int? multiverseId, Uri imageUrl) : this()
        {
            Name = name;
            Expansion = expansion;
            MultiverseId = multiverseId != null ? (int)multiverseId : 0;
            ImageUrl = imageUrl.ToString();
        }

        public int MultiverseId { get; set; }
        public string Name { get; set; }
        public string Expansion { get; set; }
        public string ImageUrl { get; set; }
    }
}
