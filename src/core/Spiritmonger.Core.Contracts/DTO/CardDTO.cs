using System;

namespace Spiritmonger.Core.Contracts.DTO
{
    public class CardBaseDTO
    {
        public Guid Id { get; set; }
        public int MultiverseId { get; set; }
        public string Name { get; set; }
        public string Expansion { get; set; }
        public string ImageUrl { get; set; }

    }

    public class CardDTO : CardBaseDTO
    {
        public CardNameBaseDTO CardName { get; set; }
    }
}
