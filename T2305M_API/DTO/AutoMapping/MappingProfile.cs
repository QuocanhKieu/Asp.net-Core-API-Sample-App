using AutoMapper;
using T2305M_API.DTO.Notification;
using T2305M_API.Entities.Notification;
using T2305M_API.Entities;
using T2305M_API.DTO.Event;

namespace T2305M_API.DTO.AutoMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateBasicNotificationDTO, UserNotification>();
            CreateMap<UserNotification, GetBasicNotificationDTO>();
            CreateMap<Image, GetBasicImageDTO>();
            CreateMap<Entities.Event, GetBasicEventDTO>();
            CreateMap<CreateEventDTO, Entities.Event>();
            CreateMap<OrderDTO, Order>();
            CreateMap<OrderDTO, Payment>();
            CreateMap<OrderDTO, EventTicket>();
            CreateMap<Entities.Event, GetDetailEventDTO>();
            CreateMap<UpdateEventDTO, Entities.Event>()
            // Ignore the UserId property during mapping
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        }
    }
}
