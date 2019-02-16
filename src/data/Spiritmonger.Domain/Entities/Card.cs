using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spiritmonger.Domain.Entities
{
    [Table("Cards")]
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int MultiverseId { get; set; }
        public string Name { get; set; }
        public string Expansion { get; set; }
        public string ImageUrl { get; set; }

        // TODO: this field is currently nullable for import purposes
        public Guid? CardNameId { get; set; }
        public CardName CardName { get; set; }
    }
}
