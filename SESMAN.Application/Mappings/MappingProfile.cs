using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Domain.Entities;

namespace SESMAN.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestHeaderDto, RequestHeader>();
 
            CreateMap<RequestHeader, RequestHeaderDto>();

            CreateMap<RequestParameterDto, RequestParameter>();

            CreateMap<RequestParameter, RequestParameterDto>();

            CreateMap<ResponseHeaderDto, ResponseHeader>();

            CreateMap<ResponseHeader, ResponseHeaderDto>();


            CreateMap<RequestLog, RequestLogDto>();

            CreateMap<CreateRequestLogDto, RequestLog>();
    


            CreateMap<UpdateRequestLogDto, RequestLog>()
                .ForMember(dest => dest.RequestHeaders, opt => opt.Ignore())
                .ForMember(dest => dest.RequestParameters, opt => opt.Ignore());

    
      
            CreateMap<ResponseLog, ResponseLogDto>();

            CreateMap<CreateResponseLogDto, ResponseLog>();


   
            CreateMap<UpdateResponseLogDto, ResponseLog>()
                .ForMember(dest => dest.ResponseHeaders, opt => opt.Ignore());
        }
    }
}