using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetLive.House.Search.Models
{
    public class BuildShopDbContext : DbContext
    {
        public BuildShopDbContext(DbContextOptions<BuildShopDbContext> options)
          : base(options) { }


        public DbSet<BuildingBaseInfo> buildingBaseInfos { get; set; }


        ////链接方式1
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseMySQL(@"server=localhost;uid=root;pwd='';
        //            port=3306;database=essearch;sslmode=Preferred;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetValueConverter(new BoolToIntConverter());
                    }
                }
            }
                builder.Entity<BuildingBaseInfo>(b => {
                b.ToTable("xkj_fy_buildingbaseinfos");
                b.Property<bool>("IsDeleted");
            });           

        }

        public class BoolToIntConverter : ValueConverter<bool, int>
        {
            public BoolToIntConverter(ConverterMappingHints mappingHints = null)
                : base(
                      v => Convert.ToInt32(v),
                      v => Convert.ToBoolean(v),
                      mappingHints)
            {
            }
            public static ValueConverterInfo DefaultInfo { get; }
                = new ValueConverterInfo(typeof(bool), typeof(int), i => new BoolToIntConverter(i.MappingHints));
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {

        }

    }
}
