using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Maper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DAL.Model.Video, BL.Model.BLVideo>()
             .ForMember(dest => dest.Image, option => option.MapFrom(src => src.Image))
             .ForMember(dest => dest.Genre, option => option.MapFrom(src => src.Genre))
             .ForMember(dest => dest.VideoTags, option => option.MapFrom(src => src.VideoTags));

            CreateMap<BL.Model.BLVideo, DAL.Model.Video>()
             .ForMember(dest => dest.Image, option => option.MapFrom(src => src.Image))
             .ForMember(dest => dest.Genre, option => option.MapFrom(src => src.Genre))
             .ForMember(dest => dest.VideoTags, option => option.MapFrom(src => src.VideoTags));

            CreateMap<DAL.Model.Tag, BL.Model.BLTag>();
            CreateMap<BL.Model.BLTag, DAL.Model.Tag>();

            CreateMap<DAL.Model.VideoTag, BL.Model.BLVideoTag>();
            CreateMap<BL.Model.BLVideoTag, DAL.Model.VideoTag>();

            CreateMap<DAL.Model.Country, BL.Model.BLCountry>();
            CreateMap<BL.Model.BLCountry, DAL.Model.Country>();

            CreateMap<DAL.Model.Genre, BL.Model.BLGenre>();
            CreateMap<BL.Model.BLGenre, DAL.Model.Genre>();

            CreateMap<DAL.Model.Image, BL.Model.BLImage>();
            CreateMap<BL.Model.BLImage, DAL.Model.Image>();

            CreateMap<DAL.Model.User, BL.Model.BLUser>();
            CreateMap<BL.Model.BLUser, DAL.Model.User>();

            CreateMap<DAL.Model.Notification, BL.Model.BLNotification>();
            CreateMap<BL.Model.BLNotification, DAL.Model.Notification>();

        }
    }
}
