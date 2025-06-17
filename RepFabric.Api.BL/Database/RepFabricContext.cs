using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using RepFabric.Api.Models.Database;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RepFabric.Api.BL.Database
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the RepFabric application.
    /// Manages the database connection and provides access to entity sets.
    /// </summary>
    public class RepFabricContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepFabricContext"/> class using the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public RepFabricContext(DbContextOptions<RepFabricContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TemplateMapping}"/> representing the TemplateMappings table.
        /// </summary>
        public DbSet<TemplateMapping> TemplateMappings { get; set; }

        /// <summary>
        /// Configures the entity mappings and relationships for the context.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TemplateMapping entity properties and keys
            modelBuilder.Entity<TemplateMapping>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TemplateFileName).IsRequired();
                entity.Property(e => e.MappingJson).IsRequired();
            });
        }
    }
}