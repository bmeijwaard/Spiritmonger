using System;

namespace Spiritmonger.Core.Contracts.DTO
{
    public class CardBaseDTO : IBaseDto
    {
        public Guid Id { get; set; }
        public int MultiverseId { get; set; }
        public string Name { get; set; }
        public string Expansion { get; set; }
        public string ImageUrl { get; set; }

        public string CardType { get; set; }
        public string ManaCost { get; set; }
        public string Mana { get; set; }


        public decimal? MKM_price { get; set; }
        public decimal? CKD_price { get; set; }
        public decimal? TIX_price { get; set; }
    }

    public class CardDTO : CardBaseDTO
    {
        public int Relevance { get; set; }
        public CardNameBaseDTO CardName { get; set; }
    }
}
