using AutoMapper;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Domain.Entities;

namespace Spiritmonger.Mapping.Profiles
{
    public class CoreProfile : Profile
    {
        public CoreProfile()
        {
            CreateMap<Card, CardDTO>();
            CreateMap<Card, CardBaseDTO>();
            CreateMap<CardDTO, Card>();
            CreateMap<CardBaseDTO, Card>();

            CreateMap<CardName, CardNameDTO>();
            CreateMap<CardName, CardNameBaseDTO>();
            CreateMap<CardNameDTO, CardName>();
            CreateMap<CardNameBaseDTO, CardName>();
        }
    }
}
