namespace ManyToManyEF
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using AutoMapper;

    internal class Program
    {
        private static void Main()
        {
            var categoryId = 1;

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ProductsCategory, ProductDTO>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name));
            });


            using (
                var db =
                    new ApplicationDbContext("Data Source=192.168.1.37;Initial Catalog=FooDb;Integrated Security=SSPI;")
            )
            {
                var query = db.ProductsCategories
                    .Where(n => n.CategoryId == categoryId)
                    .ProjectToList<ProductDTO>();

                Debugger.Break();
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProductsCategory> ProductsCategories { get; set; }
    }

    public class ProductDTO
    {
        public string Name { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProductsCategory> ProductsCategories { get; set; }
    }

    public class ProductsCategory
    {
        public virtual int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public virtual int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            Database.Log = Console.WriteLine;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductsCategory> ProductsCategories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductsCategory>()
                .HasKey(id => new {id.ProductId, id.CategoryId});
        }
    }
}