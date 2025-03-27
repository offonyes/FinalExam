using AutoMapper;
using FinalExam.Models.DTO_s.Booking;
using FinalExam.Models.DTO_s.City;
using FinalExam.Models.DTO_s.Hotel;
using FinalExam.Models.DTO_s.Role;
using FinalExam.Models.DTO_s.Room;
using FinalExam.Models.DTO_s.RoomPhoto;
using FinalExam.Models.DTO_s.RoomType;
using FinalExam.Models.DTO_s.User;
using FinalExam.Models.Entities;

namespace FinalExam
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<City, CityDTO>().ReverseMap();
            CreateMap<City, CityCreateDTO>().ReverseMap();

            CreateMap<RoomType, RoomTypeDTO>().ReverseMap();
            CreateMap<RoomType, RoomTypeCreateDTO>().ReverseMap();

            CreateMap<Hotel, HotelCreateDTO>().ReverseMap();
            CreateMap<Hotel, HotelUpdateDTO>().ReverseMap();
            CreateMap<Hotel, HotelListDTO>().ReverseMap();
            CreateMap<Hotel, HotelDetailDTO>().ReverseMap();

            CreateMap<Role, RoleCreateDTO>().ReverseMap();
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<Role, RoleUpdateDTO>().ReverseMap();

            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<Room, RoomCreateDTO>().ReverseMap();
            CreateMap<Room, RoomUpdateDTO>().ReverseMap();

            CreateMap<RoomPhoto, RoomPhotoDTO>().ReverseMap();
            CreateMap<RoomPhoto, RoomPhotoCreateDTO>().ReverseMap();

            CreateMap<Booking, BookingDTO>().ReverseMap();
            CreateMap<Booking, BookingCreateDTO>().ReverseMap();
            CreateMap<Booking, BookingUpdateDTO>().ReverseMap();
            CreateMap<Booking, BookingUpdateStatusDTO>().ReverseMap();                  

            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
