using Spiritmonger.Cmon.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spiritmonger.Domain.Entities
{
    [Table("MetaCards")]
    public class MetaCard : BaseEntity
    {
        /// <summary>
        /// The name of the card, unique in the database
        /// </summary>
        public string Name { get; set; } // Unique

        /// <summary>
        /// Metatype represents the 
        /// </summary>
        public MetaType MetaType { get; set; }

        /// <summary>
        /// Indication to wether this card adds or decreases the general CA
        /// </summary>
        public int CardAdvantage { get; set; }

        /// <summary>
        /// The converted manacost of the card, take into account X in the casting cost
        /// </summary>
        public int CMC { get; set; }

        /// <summary>
        /// The manacost of the card including colors ie 2GWW
        /// </summary>
        public string ManaCost { get; set; }

        /// <summary>
        /// The mana colors and ammounts this provides ie GGW
        /// </summary>
        public string Mana { get; set; }

        /// <summary>
        /// Indicates wether the mana requirements should be taken into account with only initial mana sources or also with virtual sources such as mana dorks
        /// </summary>
        public bool IsInitial { get; set; }

    }
}
