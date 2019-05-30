using Spiritmonger.Cmon.Types;
using System;

namespace Spiritmonger.Core.Contracts.DTO
{
    public class MetaCardDTO : IBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public MetaType MetaType { get; set; }
        public int CardAdvantage { get; set; }
        public int CMC { get; set; }
        public string ManaCost { get; set; }
        public string Mana { get; set; }
        public bool IsInitial { get; set; }
    }
}
