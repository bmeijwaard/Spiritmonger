using MtgApiManager.Lib.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spiritmonger
{
    public class Program
    {
        private static CardService _cardService;
        static Program()
        {
            _cardService = new CardService();
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

                    var cards = await _cardService
                        .Where(q => q.Page, currentPage)
                        .Where(q => q.GameFormat, "Legacy")
                        .AllAsync();

                    if (!cards.IsSuccess)
                        throw cards.Exception;

                    totalPages = cards.PagingInfo.TotalPages;

                    cards.Value.Where(card => card.MultiverseId != null).ToList().ForEach(card =>
                    {
                        try
                        {
                            var c = new Card(card.Name, card.SetName, card.MultiverseId, card.ImageUrl);
                            Console.WriteLine($"Success: {card.Name}, {card.SetName}, {card.MultiverseId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"-- Fail: {card.Name}, {card.SetName}, {card.MultiverseId}, Error: {ex.Message}");
                        }
                    });

                    currentPage++;
                    Thread.Sleep(800);
                } while (totalPages > 0 && currentPage <= totalPages);

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

        [Key]
        public int MultiverseId { get; set; }
        public string Name { get; set; }
        public string Expansion { get; set; }
        public string ImageUrl { get; set; }
    }
}
