using AutoMapper;
using Spiritmonger.Mapping.Profiles;

namespace Spiritmonger.Api.Config
{
    public class AutoMapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CoreProfile>();
            });
        }
    }
}
