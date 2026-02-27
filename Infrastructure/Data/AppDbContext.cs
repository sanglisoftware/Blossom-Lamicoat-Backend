using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<GalleryFilter> GalleryFilters { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<ProductNutrition> ProductNutritions { get; set; }
        public DbSet<Shop> Shop { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Enquiry> Enquiry { get; set; }
        public DbSet<Chemical> Chemical { get; set; }
        public DbSet<Grade> Grade { get; set; }
        public DbSet<Colour> Colour { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Gramage> Gramage { get; set; }
        public DbSet<Width> Width { get; set; }
        public DbSet<PVCproductList> PVCproductList { get; set; }
        public DbSet<FGramage> FGramage { get; set; }
        public DbSet<FproductList> FproductList { get; set; }
        public DbSet<Quality> Quality { get; set; }
        public DbSet<GSM> GSM { get; set; }
        public DbSet<FinalProduct> FinalProduct { get; set; }
        public DbSet<FormulaMaster> FormulaMaster { get; set; }
        public DbSet<FormulaChemicalTransaction> FormulaChemicalTransaction { get; set; }

        public DbSet<ChemicalInward> ChemicalInward { get; set; }
        public DbSet<PVCInward> PVCInward { get; set; }
        public DbSet<FabricInward> FabricInward { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------------- NEWS ----------------
            modelBuilder.Entity<News>(entity =>
            {
                entity.ToTable("m_news");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.Href).HasColumnName("href");
                entity.Property(e => e.Img).HasColumnName("img");
                entity.Property(e => e.Card).HasColumnName("card");
                entity.Property(e => e.Card2).HasColumnName("card2");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Para).HasColumnName("para");
                entity.Property(e => e.Views).HasColumnName("views");
                entity.Property(e => e.Comment).HasColumnName("comment");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.Video).HasColumnName("video");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            // ---------------- SHOP ----------------
            modelBuilder.Entity<Shop>(entity =>
            {
                entity.ToTable("m_shop");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Href).HasColumnName("href");
                entity.Property(e => e.Mobile).HasColumnName("mobile");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Latitude).HasColumnName("latitude");
                entity.Property(e => e.Longitude).HasColumnName("longitude");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            // ---------------- PRODUCT NUTRITION ----------------
            modelBuilder.Entity<ProductNutrition>(entity =>
            {
                entity.ToTable("product_nutrition_transaction");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.NutritionalValue).HasColumnName("nutritional_value");
                entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            });

            // ---------------- CATEGORY ----------------
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("m_category");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.ImagePath).HasColumnName("image_path");
                entity.Property(e => e.SequenceNo).HasColumnName("sequence_no");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            // ---------------- GALLERY ----------------
            modelBuilder.Entity<Gallery>(entity =>
            {
                entity.ToTable("m_gallery");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.FilterId).HasColumnName("filter_id");
                entity.Property(e => e.ImagePath).HasColumnName("image_path");
                entity.Property(e => e.SequenceNo).HasColumnName("sequence_no");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity
                    .HasOne(e => e.GalleryFilter)
                    .WithMany()
                    .HasForeignKey(e => e.FilterId)
                    .HasConstraintName("FK_Gallery_Filter");
            });

            // ---------------- SLIDER ----------------
            modelBuilder.Entity<Slider>(entity =>
            {
                entity.ToTable("m_slider");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.ImagePath).HasColumnName("image_path");
                entity.Property(e => e.SequenceNo).HasColumnName("sequence_no");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            // ---------------- USER ----------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("m_login");
                entity.HasKey(e => e.Username);
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.Role).HasColumnName("role");
                entity.Property(e => e.LoginStatus).HasColumnName("login_status");
                entity.Property(e => e.LastLoginDate).HasColumnName("last_login_date");
            });

            // ---------------- SIZE ----------------
            modelBuilder.Entity<Size>(entity =>
            {
                entity.ToTable("m_size");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.SizeValue).HasColumnName("size");
            });

            // ---------------- ROLE ----------------
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("m_role");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.RoleValue).HasColumnName("name");
            });

            // ---------------- GALLERY FILTER ----------------
            modelBuilder.Entity<GalleryFilter>(entity =>
            {
                entity.ToTable("m_gallery_filter");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.FilterValue).HasColumnName("name");
            });

            // ---------------- PRODUCT ----------------
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("m_product");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.SizeId).HasColumnName("size_id");
                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10,2)");
                entity.Property(e => e.Gst).HasColumnName("gst").HasColumnType("decimal(10,2)");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne<Category>().WithMany().HasForeignKey(e => e.CategoryId);

                entity.HasOne<Size>().WithMany().HasForeignKey(e => e.SizeId);
            });

            // ---------------- EMPLOYEE ----------------
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("m_employee");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity
                    .Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.MiddleName).HasColumnName("middle_name").HasMaxLength(50);
                entity
                    .Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(50)
                    .IsRequired();
                entity
                    .Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(15)
                    .IsRequired();
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity
                    .Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.ActiveStatus).HasColumnName("active_status");
                entity.Property(e => e.Type).HasColumnName("type");
            });

            // ---------------- MENU ----------------
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("m_menu");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.ParentId).HasColumnName("parent_id");
                entity.Property(e => e.Icon).HasColumnName("icon").HasMaxLength(50);
                entity.Property(e => e.Pathname).HasColumnName("path_name").HasMaxLength(255);
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255);
                entity.Property(e => e.Sequence).HasColumnName("sequence").HasDefaultValue(0);
            });

            // ---------------- ROLE MENU PERMISSION ----------------
            modelBuilder.Entity<RoleMenuPermission>(entity =>
            {
                entity.ToTable("role_menu_permission_transaction");
                entity.HasKey(e => new { e.RoleId, e.MenuId });
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.MenuId).HasColumnName("menu_id");
                entity
                    .Property(e => e.CreatePermission)
                    .HasColumnName("create_permission")
                    .HasDefaultValue(false);
                entity
                    .Property(e => e.UpdatePermission)
                    .HasColumnName("update_permission")
                    .HasDefaultValue(false);
                entity
                    .Property(e => e.DeletePermission)
                    .HasColumnName("delete_permission")
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<Enquiry>(entity =>
            {
                entity.ToTable("m_enquiry");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50).IsUnicode(true);

                entity
                    .Property(e => e.MobileNumber)
                    .HasColumnName("MobileNumber")
                    .HasMaxLength(13)
                    .IsUnicode(false);

                entity
                    .Property(e => e.Product)
                    .HasColumnName("Product")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity
                    .Property(e => e.PrimaryDiscussion)
                    .HasColumnName("PrimaryDiscussion")
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.status).HasColumnName("status");

                entity.Property(e => e.FollowupDate).HasColumnName("FollowupDate");

                entity
                    .Property(e => e.FeedBack)
                    .HasColumnName("FeedBack")
                    .HasMaxLength(50)
                    .IsUnicode(true);

                // This column exists in DB but is ignored in Entity
                entity.Ignore(e => e.EnquiryTakenBy);
            });

            modelBuilder.Entity<Chemical>(entity =>
            {
                entity.ToTable("m_chemical");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(100)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.ToTable("m_grade");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<Colour>(entity =>
            {
                entity.ToTable("m_colour");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("m_customer");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(255)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Mobile_No)
                    .HasColumnName("mobile_no")
                    .HasMaxLength(13)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsUnicode(true);

                entity
                    .Property(e => e.GST_No)
                    .HasColumnName("gst_no")
                    .HasMaxLength(15)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("m_supplier");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(255)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Mobile_No)
                    .HasColumnName("mobile_no")
                    .HasMaxLength(13)
                    .IsUnicode(true);

                entity.Property(e => e.Pan).HasColumnName("pan").HasMaxLength(255).IsUnicode(true);

                entity
                    .Property(e => e.GST_No)
                    .HasColumnName("gst_no")
                    .HasMaxLength(15)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<Gramage>(entity =>
            {
                entity.ToTable("m_gramage");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.GRM).HasColumnName("grm").HasMaxLength(200).IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<Width>(entity =>
            {
                entity.ToTable("m_width");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.GRM).HasColumnName("grm").HasMaxLength(200).IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<PVCproductList>(entity =>
            {
                entity.ToTable("m_pvcproducttable");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity
                    .Property(e => e.GramageMasterId)
                    .HasColumnName("gramage_master_id");
                entity
                    .Property(e => e.WidthMasterId)
                    .HasColumnName("width_master_id");

                entity
                    .Property(e => e.ColourMasterId)
                    .HasColumnName("colour_master_id");

                entity
                    .Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasMaxLength(15)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");


                // Foreign Keys

                entity.HasOne(e => e.Gramage).WithMany(s => s.PVCGramage).HasForeignKey(e => e.GramageMasterId);

                entity.HasOne(e => e.Width).WithMany(s => s.PVCWidth).HasForeignKey(e => e.WidthMasterId);

                entity.HasOne(e => e.Colour).WithMany(s => s.PVCColour).HasForeignKey(e => e.ColourMasterId);

            });

            modelBuilder.Entity<FGramage>(entity =>
            {
                entity.ToTable("m_fgramage");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.GRM).HasColumnName("grm").HasMaxLength(200).IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<FproductList>(entity =>
            {
                entity.ToTable("m_fproductlist");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200);

                entity.Property(e => e.FGramageMasterId).HasColumnName("fgramage_master_id");

                entity
                    .Property(e => e.ColourMasterId)
                    .HasColumnName("colour_master_id");
                  

                entity
                    .Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasMaxLength(15)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");


                entity.HasOne(e => e.FGramage).WithMany(s => s.Fgrmage).HasForeignKey(e => e.FGramageMasterId);


              entity.HasOne(e => e.Colour).WithMany(s => s.FColour).HasForeignKey(e => e.ColourMasterId);

            });

            modelBuilder.Entity<Quality>(entity =>
            {
                entity.ToTable("m_quality");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity
                    .Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasMaxLength(15)
                    .IsUnicode(true);

                entity.Property(e => e.GSMGLMMasterId) .HasColumnName("gsmglm_master_id");

                entity.Property(e => e.ColourMasterId).HasColumnName("colour_master_id");
                
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                   // Foreign Keys

                entity.HasOne(e => e.GSM).WithMany(s => s.gsmquality).HasForeignKey(e => e.GSMGLMMasterId);

                       entity.HasOne(e => e.Colour).WithMany(c => c.colourquality).HasForeignKey(e => e.ColourMasterId);
            });

            modelBuilder.Entity<GSM>(entity =>
            {
                entity.ToTable("m_gsm");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<FinalProduct>(entity =>
            {
                entity.ToTable("m_final_product");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.Final_Product)
                    .HasColumnName("final_product")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<FormulaMaster>(entity =>
            {
                entity.ToTable("m_formula_master");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity
                    .Property(e => e.FinalProductId)
                    .HasColumnName("final_product_id")
                    .HasMaxLength(200)
                    .IsUnicode(true);

                entity.Property(e => e.IsActive).HasColumnName("is_active");
            });

            modelBuilder.Entity<FormulaChemicalTransaction>(entity =>
            {
                entity.ToTable("m_formula_chemical_transaction");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.FormulaMasterId).HasColumnName("formula_master_id");

                entity.Property(e => e.ChemicalMasterId).HasColumnName("chemical_master_id");

                entity.Property(e => e.Qty).HasColumnName("qty");

                // Foreign Keys

                entity.HasOne(e => e.FormulaMaster).WithMany(f => f.formulaChemicalTransactions).HasForeignKey(e => e.FormulaMasterId);

                entity.HasOne(e => e.Chemical).WithMany(c => c.formulaChemicals).HasForeignKey(e => e.ChemicalMasterId);

            });
            

            modelBuilder.Entity<ChemicalInward>(entity =>
            {
                entity.ToTable("m_chemical_inword");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.ChemicalMasterId).HasColumnName("chemical_master_id");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.SupplierMasterId).HasColumnName("supplier_master_id");

                entity.Property(e => e.BatchNo).HasColumnName("batch_no");

                entity.Property(e => e.BillDate).HasColumnName("bill_date");

                entity.Property(e => e.ReceivedDate).HasColumnName("received_date");

                entity.Property(e => e.IsActive).HasColumnName("is_active");


                // Foreign Keys

                entity.HasOne(e => e.Supplier).WithMany(s => s.supplierInwards).HasForeignKey(e => e.SupplierMasterId);

                       entity.HasOne(e => e.Chemical).WithMany(c => c.chemicalInwards).HasForeignKey(e => e.ChemicalMasterId);

            });



              modelBuilder.Entity<PVCInward>(entity =>
            {
                entity.ToTable("m_pvc_inward");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.SupplierMasterId).HasColumnName("supplier_master_id");

                entity.Property(e => e.PVCMasterId).HasColumnName("pvc_master_id");

                entity.Property(e => e.New_RollNo).HasColumnName("new_rollno");

                entity.Property(e => e.BatchNo).HasColumnName("batch_no");

                entity.Property(e => e.Qty_kg).HasColumnName("qty_kg");

                entity.Property(e => e.Qty_Mtr).HasColumnName("qty_mtr");

                entity.Property(e => e.Comments).HasColumnName("comments");

                entity.Property(e => e.BillDate).HasColumnName("bill_date");

                entity.Property(e => e.ReceivedDate).HasColumnName("received_date");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                // Foreign Keys

                entity.HasOne(e => e.Supplier).WithMany(s => s.supplierPVCInwards).HasForeignKey(e => e.SupplierMasterId);

                entity.HasOne(e => e.PVC).WithMany(c => c.pvcInwards).HasForeignKey(e => e.PVCMasterId);

            });


             modelBuilder.Entity<FabricInward>(entity =>
            {
                entity.ToTable("m_fabric_inward");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();

                entity.Property(e => e.SupplierMasterId).HasColumnName("supplier_master_id");

                entity.Property(e => e.FabricMasterId).HasColumnName("fabric_master_id");

                entity.Property(e => e.BatchNo).HasColumnName("batch_no");

                entity.Property(e => e.QtyMTR).HasColumnName("qty_mtr");

                entity.Property(e => e.Comments).HasColumnName("comments");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                // Foreign Keys

                entity.HasOne(e => e.Supplier).WithMany(s => s.supplierFInwards).HasForeignKey(e => e.SupplierMasterId);

                entity.HasOne(e => e.Fabric).WithMany(c => c.fabricInwards).HasForeignKey(e => e.FabricMasterId);

            });
        }


        
    }
}
