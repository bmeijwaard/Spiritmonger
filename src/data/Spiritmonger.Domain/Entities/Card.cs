using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spiritmonger.Domain.Entities
{
    [Table("Cards")]
    public class Card : BaseEntity
    {
        public int MultiverseId { get; set; }
        public string Name { get; set; }
        public string Expansion { get; set; }
        public string ImageUrl { get; set; }

        public string CardType { get; set; }
        public string ManaCost { get; set; }
        public string Mana { get; set; }

        // TODO: this field is currently nullable for import purposes
        public Guid? CardNameId { get; set; }
        public CardName CardName { get; set; }

        public decimal? MKM_price { get; set; }
        public decimal? CKD_price { get; set; }
        public decimal? TIX_price { get; set; }


        [NotMapped]
        public int Relevance { get; set; }

    }
}
