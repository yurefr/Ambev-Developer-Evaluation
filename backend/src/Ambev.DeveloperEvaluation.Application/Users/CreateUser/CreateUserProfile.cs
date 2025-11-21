using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser;

public class CreateUserProfile : Profile
{
    public CreateUserProfile()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());

        CreateMap<User, CreateUserResult>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));
    }
}