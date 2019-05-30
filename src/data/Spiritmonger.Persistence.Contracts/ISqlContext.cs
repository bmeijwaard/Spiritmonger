using Microsoft.EntityFrameworkCore;
using Spiritmonger.Domain.Entities;

namespace Spiritmonger.Persistence.Contracts
{
    public interface ISqlContext : IBaseContext
    {
        DbSet<Card> Cards { get; set; }
        DbSet<MetaCard> MetaCards { get; set; }
        DbSet<CardName> CardNames { get; set; }
    }
}