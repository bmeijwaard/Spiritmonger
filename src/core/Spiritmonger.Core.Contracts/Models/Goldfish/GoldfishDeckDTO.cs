using Spiritmonger.Cmon.Types;
using System.Collections.Generic;

namespace Spiritmonger.Core.Contracts.Models.Goldfish
{
    public class GoldfishDeckDTO
    {
        public GoldfishDeckDTO(string name, string url)
        {
            Url = url;
            Name = name;
            Cards = new List<GoldfishCardDTO>();
        }
        public string Url;
        public string Name;
        public IEnumerable<GoldfishCardDTO> Cards;
        public Format Format;
    }
}
