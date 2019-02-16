using System;
using System.Collections.Generic;

namespace Spiritmonger.Core.Contracts.DTO
{
    public class CardNameBaseDTO : IBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CardNameDTO : CardNameBaseDTO
    {
        public IList<CardBaseDTO> Cards { get; set; }
    }
}
