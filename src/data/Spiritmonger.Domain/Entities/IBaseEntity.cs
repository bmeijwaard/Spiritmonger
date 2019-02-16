using System;

namespace Spiritmonger.Domain.Entities
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }
    }
}
