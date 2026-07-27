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



            // Collection Eşlemeleri
            CreateMap<CollectionDto, Collection>();
            CreateMap<Collection, CollectionDto>();

            CreateMap<CreateCollectionDto, Collection>();
            CreateMap<Collection, CreateCollectionDto>();

            // SavedRequest Eşlemeleri
            CreateMap<SavedRequestDto, SavedRequest>();
            CreateMap<SavedRequest, SavedRequestDto>();

            CreateMap<CreateSavedRequestDto, SavedRequest>();
            CreateMap<SavedRequest, CreateSavedRequestDto>();

            //SavedHeader ve SavedParameter Eşlemeleri
            CreateMap<SavedRequestHeaderDto, SavedRequestHeader>();
            CreateMap<SavedRequestHeader, SavedRequestHeaderDto>();

            CreateMap<SavedRequestParameterDto, SavedRequestParameter>();
            CreateMap<SavedRequestParameter, SavedRequestParameterDto>();

            //auth eşleşmeleri
            CreateMap<AuthConfigDto, AuthConfig>();
            CreateMap<AuthConfig, AuthConfigDto>();

        }
    }
}