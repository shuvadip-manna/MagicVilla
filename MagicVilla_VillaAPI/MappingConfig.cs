using AutoMapper;
using MagicVilla_VillaAPI.Model;
using MagicVilla_VillaAPI.Model.DTO;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDTO>().ReverseMap();
            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
        }
    }
}
