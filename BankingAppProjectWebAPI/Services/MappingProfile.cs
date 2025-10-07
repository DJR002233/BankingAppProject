using AutoMapper;
using BankingAppProjectWebAPI.Models;
using BankingAppProjectWebAPI.Requests;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateAccountRequest, AccountInformation>();
    }
}
