using Spiritmonger.Cmon.Types;
using Spiritmonger.Core.Contracts.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spiritmonger.Api.Models.Requests
{
    public class MetaCardRequest
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public MetaType MetaType { get; set; }

        [Required]
        public int CardAdvantage { get; set; }

        [Required]
        public int CMC { get; set; }

        public string ManaCost { get; set; } = string.Empty;

        public string Mana { get; set; } = string.Empty;

        [Required]
        public bool IsInitial { get; set; }

        public MetaCardDTO GetDTO()
        {
            return new MetaCardDTO
            {
                Id = this.Id ?? Guid.Empty,
                Name = this.Name,
                MetaType = this.MetaType,
                CardAdvantage = this.CardAdvantage,
                CMC = this.CMC,
                ManaCost = this.ManaCost,
                Mana = this.Mana,
                IsInitial = this.IsInitial
            };
        }
    }
}
