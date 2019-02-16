using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spiritmonger.Domain.Entities
{
    [Table("CardNames")]
    public class CardName : BaseEntity
    {
        public string Name { get; set; }

        public IList<Card> Cards { get; set; }
    }
}
