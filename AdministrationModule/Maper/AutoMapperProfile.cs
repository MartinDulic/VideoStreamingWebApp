using AdministrationModule.Models;
using AutoMapper;

namespace AdministrationModule.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BL.Model.BLVideo, VMVideo>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre))
            .ForMember(dest => dest.VideoTags, opt => opt.MapFrom(src => src.VideoTags))
            .ForMember(dest => dest.NewTags, opt => opt.Ignore());

            CreateMap<VMVideo, BL.Model.BLVideo>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre))
            .ForMember(dest => dest.VideoTags, opt => opt.MapFrom(src => src.VideoTags));

            CreateMap<BL.Model.BLTag, VMTag>();
            CreateMap<VMTag, BL.Model.BLTag>();

            CreateMap<BL.Model.BLVideoTag, VMVideoTag>();
            CreateMap<VMVideoTag, BL.Model.BLVideoTag>();

            CreateMap<BL.Model.BLCountry, VMCountry>();
            CreateMap<VMCountry, BL.Model.BLCountry>();

            CreateMap<BL.Model.BLGenre, VMGenre>();
            CreateMap<VMGenre, BL.Model.BLGenre>();

            CreateMap<BL.Model.BLImage, VMImage>();
            CreateMap<VMImage, BL.Model.BLImage>();

            CreateMap<BL.Model.BLUser, VMUser>()
            .ForMember(dest => dest.CountryOfResidenceId, opt => opt.MapFrom(src => src.CountryOfResidence.Id))
            .ForMember(dest => dest.CountryOfResidence, opt => opt.MapFrom(src => src.CountryOfResidence)); 
            CreateMap<VMUser, BL.Model.BLUser>()
            .ForMember(dest => dest.CountryOfResidenceId, opt => opt.MapFrom(src => src.CountryOfResidence.Id))
            .ForMember(dest => dest.CountryOfResidence, opt => opt.MapFrom(src => src.CountryOfResidence)); 
        }
    }
}
