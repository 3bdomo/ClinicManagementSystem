using AutoMapper;
using BLL.DTOs.User;
using Web.ViewModel;

namespace Web.Mapping;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        // UserDto  →  UserRowViewModel  (used in Index)
        CreateMap<UserDto, UserRowViewModel>();

        // CreateUserViewModel  →  CreateUserDto  (used in POST Create)
        CreateMap<CreateUserViewModel, CreateUserDto>();

        // UserDto  →  EditUserViewModel  (used in GET Edit)
        CreateMap<UserDto, EditUserViewModel>();

        // EditUserViewModel  →  UpdateUserDto  (used in POST Edit)
        CreateMap<EditUserViewModel, UpdateUserDto>();
    }
}
