using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Spiritmonger.Cmon.Clients;
using Spiritmonger.Cmon.Types;
using Spiritmonger.Core.Contracts.Models.Goldfish;

namespace Spiritmonger.Core.Scrapers
{
    public static class GoldfishScraper
    {
        public static async Task<IEnumerable<GoldfishDeckDTO>> GetDecksAsync(Format format)
        {
            var decks = new ConcurrentBag<GoldfishDeckDTO>();
            var html = await new HtmlClient($"https://www.mtggoldfish.com/metagame/{format.ToString()}/full").MakeRequestAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode
                .SelectNodes("//div[contains(@class,'archetype-tile-description-wrapper')]/div")
                .Distinct()
                .ToList();

            foreach (var node in nodes)
            {
                var anchor = node.SelectNodes(".//a")?.FirstOrDefault();
                var href = anchor?.Attributes["href"]?.Value;
                var name = WebUtility.HtmlDecode(anchor?.InnerHtml);
                decks.Add(new GoldfishDeckDTO(name, $"https://www.mtggoldfish.com{href}"));
            };

            return decks;
        }

        public static async Task<IEnumerable<GoldfishCardDTO>> GetCardsAsync(string deckUrl)
        {
            var cards = new ConcurrentBag<GoldfishCardDTO>();
            var html = await new HtmlClient(deckUrl).MakeRequestAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var views = doc.DocumentNode.SelectNodes("//table[contains(@class, 'deck-view-deck-table')]");

            var paperRows = views
                .Skip(2).FirstOrDefault()
                .SelectNodes("tr");

            var onlineRows = views
                .Skip(0).FirstOrDefault()
                .SelectNodes("tr");

            var cardType = CardType.Creatures;
            foreach (var row in paperRows) // Card kingdom
            {
                var cells = row.SelectNodes("td");

                if (cells.Count == 1)
                {
                    foreach (CardType type in (CardType[])Enum.GetValues(typeof(CardType)))
                    {
                        if (cells.FirstOrDefault()?.InnerHtml?.Contains(type.ToString()) ?? false)
                        {
                            cardType = type;
                            break;
                        }
                    }
                }

                if (cells.Count != 4) continue;
                var card = new GoldfishCardDTO
                {
                    CardType = cardType
                };

                foreach (var cell in cells)
                {
                    try
                    {
                        if (cell.HasClass("deck-col-qty") && !string.IsNullOrWhiteSpace(cell.InnerHtml))
                            card.Copies = int.Parse(cell.InnerHtml);

                        if (cell.HasClass("deck-col-price") && !string.IsNullOrWhiteSpace(cell.InnerHtml))
                            card.CKD_Price = Convert.ToDouble(decimal.Parse(WebUtility.HtmlDecode(cell.InnerHtml).Replace(",", "")) / card.Copies) / 100;

                        if (cell.HasClass("deck-col-card") && !string.IsNullOrWhiteSpace(cell.InnerHtml))
                        {
                            var anchor = cell.SelectNodes("a")?.FirstOrDefault();
                            card.Name = WebUtility.HtmlDecode(anchor.InnerHtml);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{cell.InnerHtml}, {e.Message}");
                        Debug.WriteLine($"{cell.InnerHtml}, {e.Message}");
                    }
                }

                cards.Add(card);
            }

            foreach (var row in onlineRows) //TIX
            {
                var cells = row.SelectNodes("td");
                if (cells.Count != 4) continue;

                string name = string.Empty;
                GoldfishCardDTO card = default;

                foreach (var cell in cells)
                {
                    if (cell.HasClass("deck-col-price") && !string.IsNullOrWhiteSpace(cell.InnerHtml))
                        cards.FirstOrDefault(c => c.Name == name).TIX_Price = Convert.ToDouble(decimal.Parse(WebUtility.HtmlDecode(cell.InnerHtml).Replace(",", "")) / card.Copies) / 100;

                    if (cell.HasClass("deck-col-card") && !string.IsNullOrWhiteSpace(cell.InnerHtml))
                    {
                        var anchor = cell.SelectNodes("a")?.FirstOrDefault();
                        name = WebUtility.HtmlDecode(anchor.InnerHtml);
                        card = cards.FirstOrDefault(c => c.Name == name);
                        if (card == null) break;
                    }
                }
            }

            return cards;
        }
    }
}
