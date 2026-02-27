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
            CreateMap<FGramage, FGramageDto>().ReverseMap();

            CreateMap<PVCproductList, PVCproductListDto>()
             .ForMember(dest => dest.GramageMasterName,
                 opt => opt.MapFrom(src => src.Gramage != null ? src.Gramage.GRM : null))
             .ForMember(dest => dest.WidthMasterName,
                 opt => opt.MapFrom(src => src.Width != null ? src.Width.GRM : null))
             .ForMember(dest => dest.ColourMasterName,
                 opt => opt.MapFrom(src => src.Colour != null ? src.Colour.Name : null));

            CreateMap<PVCproductListDto, PVCproductList>()
                .ForMember(dest => dest.Gramage, opt => opt.Ignore())
                .ForMember(dest => dest.Width, opt => opt.Ignore())
                .ForMember(dest => dest.Colour, opt => opt.Ignore());


            CreateMap<FproductList, FproductListDto>()
                .ForMember(dest => dest.FGramageMasterName, opt => opt.MapFrom(src => src.FGramage != null ? src.FGramage.GRM : null))
                .ForMember(dest => dest.ColourMasterName, opt => opt.MapFrom(src => src.Colour != null ? src.Colour.Name : null));

            CreateMap<FproductList, FproductListDto>().ReverseMap();
            CreateMap<Quality, QualityDto>()
     .ForMember(dest => dest.GSMMasterName,
         opt => opt.MapFrom(src => src.GSM != null ? src.GSM.Name : string.Empty))
     .ForMember(dest => dest.ColourMasterName,
         opt => opt.MapFrom(src => src.Colour != null ? src.Colour.Name : string.Empty))
     .ReverseMap()
     .ForMember(dest => dest.GSM, opt => opt.Ignore())
     .ForMember(dest => dest.Colour, opt => opt.Ignore());

            CreateMap<GSM, GSMDto>().ReverseMap();
            CreateMap<FinalProduct, FinalProductDto>().ReverseMap();
            CreateMap<FormulaMaster, FormulaMasterDto>().ReverseMap();
            // Mapping from Entity → DTO (for reading, include Id)
            CreateMap<FormulaChemicalTransaction, FormulaChemicalTransactionDto>();

            // Mapping from DTO → Entity (for creating/updating)
            CreateMap<FormulaChemicalTransactionDto, FormulaChemicalTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())          // Ignore identity column
                .ForMember(dest => dest.FormulaMaster, opt => opt.Ignore()) // Ignore navigation property
                .ForMember(dest => dest.Chemical, opt => opt.Ignore());    // Ignore navigation property




            CreateMap<ChemicalInward, ChemicalInwardDto>()
     .ForMember(dest => dest.ChemicalMasterName,
         opt => opt.MapFrom(src => src.Chemical != null ? src.Chemical.Name : null))
     .ForMember(dest => dest.SupplierMasterName,
         opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
     .ReverseMap()
     .ForMember(dest => dest.Chemical, opt => opt.Ignore())
     .ForMember(dest => dest.Supplier, opt => opt.Ignore());
            ;

            CreateMap<PVCInward, PVCInwardDto>()
            .ForMember(dest => dest.SupplierMasterName,
                opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
            .ForMember(dest => dest.PVCMasterName,
                opt => opt.MapFrom(src => src.PVC != null ? src.PVC.Name : null))
               .ReverseMap()
               .ForMember(dest => dest.PVC, opt => opt.Ignore())
               .ForMember(dest => dest.Supplier, opt => opt.Ignore())
               .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<FabricInward, FabricInwardDto>()
                .ForMember(dest => dest.FabricMasterName,
                    opt => opt.MapFrom(src => src.Fabric != null ? src.Fabric.Name : string.Empty))
                .ForMember(dest => dest.SupplierMasterName,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
                .ReverseMap()
                .ForMember(dest => dest.Fabric, opt => opt.Ignore())
                .ForMember(dest => dest.Supplier, opt => opt.Ignore());

        }
    }
}
