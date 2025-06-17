using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using RepFabric.Api.Models.Database;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RepFabric.Api.BL.Database
{
    public class RepFabricContext : DbContext
    {
        public RepFabricContext(DbContextOptions<RepFabricContext> options)
            : base(options)
        {
        }

        public DbSet<TemplateMapping> TemplateMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: configure TemplateMapping table name or properties if needed
            modelBuilder.Entity<TemplateMapping>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TemplateFileName).IsRequired();
                entity.Property(e => e.MappingJson).IsRequired();
            });
        }
    }
}