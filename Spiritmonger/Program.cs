using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtgApiManager.Lib.Service;
using Newtonsoft.Json;
using Spiritmonger.Cmon.Types;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Models.Goldfish;
using Spiritmonger.Core.Contracts.Services;
using Spiritmonger.Core.Scrapers;
using Spiritmonger.Mapping.Modules;
using Spiritmonger.Mapping.Profiles;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Spiritmonger
{
    public class UserSecrets
    {

    }

    public static class Program
    {
        private static CardService _cardApi;
        private static ICardService _cardService;
        private static ICardNameService _cardNameService;
        private static int PAGE = 1;
        private static int COUNTER = 0;
        private static string PAGE_URL = "https://api.scryfall.com/cards?page=";

        static Program()
        {
            _cardApi = new CardService();


            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<UserSecrets>()
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
                await RotateScryfall(); // done
                // await ReplaceImagesAsync();

                //await ScrapeGoldfishAsync();
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

        private static async Task ScrapeGoldfishAsync()
        {
            ConcurrentBag<GoldfishDeckDTO> decks = new ConcurrentBag<GoldfishDeckDTO>();
            // foreach(Format format in IterateEnumOfType<Format>())
            await IterateEnumOfType<Format>().ParallelForEachAsync(async format =>
            {
                IEnumerable<GoldfishDeckDTO> formatDecks = await GoldfishScraper.GetDecksAsync(format);
                foreach (GoldfishDeckDTO deck in formatDecks)
                {
                    IEnumerable<GoldfishCardDTO> cards = await GoldfishScraper.GetCardsAsync(deck.Url);
                    deck.Cards = cards;
                    Console.WriteLine($"Cards: {cards.Select(c => c.Copies).Sum()}, {deck.Name}");
                }
                decks.AddRange(formatDecks);
            });

            ConcurrentBag<CardNameDTO> cardNames = new ConcurrentBag<CardNameDTO>();
            var names = decks.SelectMany(d => d.Cards.Select(c => c.Name)).Distinct();
            await names.ParallelForEachAsync(async name =>
            {
                cardNames.Add(new CardNameDTO()
                {
                    Id = Guid.NewGuid(),
                    Name = name
                });
            });

            await _cardNameService.BulkUpdateOrInsertAsync(cardNames);
        }

        private static async Task ReplaceImagesAsync()
        {
            var images = new ConcurrentBag<Card>();
            await (await _cardNameService.ReadAsync())?.Data.Select(d => d.Name).Distinct().ParallelForEachAsync(async name =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var arr = name.Split("|");
                    images.Add(
                        new Card
                        {
                            Name = arr[1],
                            MultiverseId = int.Parse(arr[0])
                        });
                }
            });

            Console.WriteLine($"images count: {images.Count}");
            var cards = new ConcurrentBag<CardDTO>((await _cardService.ReadAsync())?.Data);

            await cards.ParallelForEachAsync(async card =>
            {
                var image = images.FirstOrDefault(i => i.MultiverseId == card.MultiverseId);
                if (image != null)
                {
                    card.ImageUrl = image.Name;
                }
                Interlocked.Increment(ref COUNTER);
                Console.WriteLine($"Cards done: {COUNTER}");
            });

            Console.WriteLine($"Bulk updating {cards.Count}");
            await _cardService.BulkUpdateOrInsertAsync(cards);
        }

        public static async Task RotateScryfall()
        {
            await Enumerable.Range(PAGE, 2000).ParallelForEachAsync(async index =>
            {
                var references = new ConcurrentBag<CardDTO>();

                var response = await ReadFromClientAsync<ScryfallObject>(PAGE_URL + index);

                Interlocked.Increment(ref PAGE);
                foreach (var card in response.Data)
                {
                    if (card.Multiverse_ids?.Length > 0)
                    {
                        foreach (var id in card.Multiverse_ids)
                        {
                            var name = ProcessCardAsync(card, id);
                            if (name == null) continue;
                            references.Add(name);
                            Interlocked.Increment(ref COUNTER);
                            Console.WriteLine($"Cards done: {COUNTER}, page: {PAGE}");
                        }
                    }
                }
                Thread.Sleep(50);
                await _cardService.BulkUpdateOrInsertAsync(references);
            }, 4);
        }

        private static CardNameDTO ProcessCardNameAsync(Data card, int id)
        {
            if (card?.Image_uris == null) return null;

            var url = card.Image_uris.Png ?? card.Image_uris.Normal ?? string.Empty;
            var endNumber = url.IndexOf("?") == -1 ? url.Length : url.IndexOf("?");
            return new CardNameDTO
            {
                Id = Guid.NewGuid(),
                Name = $"{id}|{url.Substring(0, endNumber)}"
            };
        }

        private static CardDTO ProcessCardAsync(Data card, int id)
        {
            if (card == null)
                return null;

            var url = card.Image_uris == null ? "" : card.Image_uris.Png ?? card.Image_uris.Normal ?? "";
            var endNumber = url.IndexOf("?") == -1 ? url.Length : url.IndexOf("?");
            return new CardDTO
            {
                Id = Guid.NewGuid(),
                MultiverseId = id,
                ImageUrl = url.Substring(0, endNumber),
                Expansion = card.Set_name,
                Name = card.Name,
                CKD_price = string.IsNullOrWhiteSpace(card.Prices.Usd) ? null : (decimal?)decimal.Parse(card.Prices.Usd),
                MKM_price = string.IsNullOrWhiteSpace(card.Prices.Eur) ? null : (decimal?)decimal.Parse(card.Prices.Eur),
                TIX_price = string.IsNullOrWhiteSpace(card.Prices.Tix) ? null : (decimal?)decimal.Parse(card.Prices.Tix)
            };
        }

        private static async Task<T> ReadFromClientAsync<T>(string url)
        {
            HttpClient client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Get, url);
            using (var response = await client.SendAsync(message))
            {
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public static async Task RotateCardsAPI()
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
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"-- Request failed: {card.Name}, {card.SetName}, {card.MultiverseId}, Error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"-- Unexpected error: {card.Name}, {card.SetName}, {card.MultiverseId}, Error: {ex.Message}");
                    }
                });

                currentPage++;
                Thread.Sleep(800);
            } while (totalPages > 0 && currentPage <= (totalPages + 1));
        }

        private static void AddRange<T>(this ConcurrentBag<T> input, IEnumerable<T> rangeToAdd)
        {
            foreach (T item in rangeToAdd)
                input.Add(item);
        }

        private static T[] IterateEnumOfType<T>()
            where T : Enum
            => (T[])Enum.GetValues(typeof(T));

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
