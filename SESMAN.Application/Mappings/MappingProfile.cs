using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Domain.Entities;

namespace SESMAN.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestHeaderDto, RequestHeader>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<RequestHeader, RequestHeaderDto>();

            CreateMap<RequestParameterDto, RequestParameter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<RequestParameter, RequestParameterDto>();

            CreateMap<ResponseHeaderDto, ResponseHeader>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ResponseHeader, ResponseHeaderDto>();


            CreateMap<RequestLog, RequestLogDto>();

            CreateMap<CreateLogDto, RequestLog>();
    


            CreateMap<UpdateLogDto, RequestLog>()
                .ForMember(dest => dest.RequestHeaders, opt => opt.Ignore())
                .ForMember(dest => dest.RequestParameters, opt => opt.Ignore());

    
      
            CreateMap<ResponseLog, ResponseLogDto>();

            CreateMap<CreateResponseLogDto, ResponseLog>();


   
            CreateMap<UpdateResponseLogDto, ResponseLog>()
                .ForMember(dest => dest.ResponseHeaders, opt => opt.Ignore());
        }
    }
}