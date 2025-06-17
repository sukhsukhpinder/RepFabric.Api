using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RepFabric.Api.BL.Database;
using RepFabric.Api.Models.Database;
using Xunit;

namespace RepFabric.Api.Tests.Tests.Database
{
    public class RepFabricContextTests
    {
        private DbContextOptions<RepFabricContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<RepFabricContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
                .Options;
        }

        [Fact]
        public void Constructor_SetsOptions()
        {
            var options = CreateOptions();
            var context = new RepFabricContext(options);

            Assert.NotNull(context);
            Assert.IsAssignableFrom<DbContext>(context);
        }

        [Fact]
        public void TemplateMappings_Property_IsDbSet()
        {
            var options = CreateOptions();
            using var context = new RepFabricContext(options);

            Assert.NotNull(context.TemplateMappings);
            Assert.IsAssignableFrom<DbSet<TemplateMapping>>(context.TemplateMappings);
        }

        [Fact]
        public void OnModelCreating_ConfiguresTemplateMappingEntity()
        {
            var options = CreateOptions();
            using var context = new RepFabricContext(options);

            var model = context.Model;
            var entityType = model.FindEntityType(typeof(TemplateMapping));
            Assert.NotNull(entityType);

            // Check key
            var key = entityType.FindPrimaryKey();
            Assert.NotNull(key);
            Assert.Contains(key.Properties, p => p.Name == "Id");

            // Check required properties
            var templateFileNameProp = entityType.FindProperty(nameof(TemplateMapping.TemplateFileName));
            var mappingJsonProp = entityType.FindProperty(nameof(TemplateMapping.MappingJson));
            Assert.NotNull(templateFileNameProp);
            Assert.NotNull(mappingJsonProp);
            Assert.False(templateFileNameProp.IsNullable);
            Assert.False(mappingJsonProp.IsNullable);
        }

        [Fact]
        public void Can_Add_And_Retrieve_TemplateMapping()
        {
            var options = CreateOptions();
            using (var context = new RepFabricContext(options))
            {
                var entity = new TemplateMapping
                {
                    TemplateFileName = "test.xlsm",
                    MappingJson = "{}"
                };
                context.TemplateMappings.Add(entity);
                context.SaveChanges();
            }

            using (var context = new RepFabricContext(options))
            {
                var entity = context.TemplateMappings.FirstOrDefault();
                Assert.NotNull(entity);
                Assert.Equal("test.xlsm", entity.TemplateFileName);
                Assert.Equal("{}", entity.MappingJson);
            }
        }
    }
}