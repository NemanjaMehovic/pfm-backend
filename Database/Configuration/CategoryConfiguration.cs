using Microsoft.EntityFrameworkCore;
using pfm.Database.Entities;

namespace pfm.Database.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(x => x.code);
        builder.Property(x => x.code).IsRequired();
        builder.Property(x => x.parent_code);
        builder.Property(x => x.name).IsRequired();
    }
}