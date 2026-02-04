using AutoMapper;
using Api.Application.DTOs;
using Api.Domain.Entities;

namespace Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Slider, SliderDto>().ReverseMap();
            CreateMap<Size, SizeDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<GalleryFilter, GalleryFilterDto>().ReverseMap();
            CreateMap<Gallery, GalleryDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<News, NewsCreateDto>().ReverseMap();
            CreateMap<Shop, ShopDto>().ReverseMap();
            CreateMap<Menu, MenuDto>()
                .ForMember(dest => dest.Permission, opt => opt.Ignore())
                .ForMember(dest => dest.SubMenu, opt => opt.Ignore());
            CreateMap<ProductNutrition, NutritionDto>().ReverseMap();
            CreateMap<Enquiry, EnquiryResponseDto>().ReverseMap();
            CreateMap<Chemical, ChemicalDto>().ReverseMap();
            CreateMap<Grade, GradeDto>().ReverseMap();
            CreateMap<Colour, ColourDto>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();
            CreateMap<Gramage, GramageDto>().ReverseMap();
            CreateMap<Width, WidthDto>().ReverseMap();
            CreateMap<PVCproductList, PVCproductListDto>().ReverseMap();
            CreateMap<FGramage, FGramageDto>().ReverseMap();
            CreateMap<FproductList, FproductListDto>().ReverseMap();







        }
    }
}
