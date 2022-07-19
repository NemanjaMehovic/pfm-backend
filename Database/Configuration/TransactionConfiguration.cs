using Microsoft.EntityFrameworkCore;
using pfm.Database.Entities;

namespace pfm.Database.Configuration;

public class TransactionConfiguration : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).IsRequired();
        builder.Property(x => x.beneficiary_name);
        builder.Property(x => x.date).IsRequired();
        builder.Property(x => x.direction).HasConversion<string>().IsRequired();
        builder.Property(x => x.amount).IsRequired();
        builder.Property(x => x.description);
        builder.Property(x => x.currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.mcc).HasMaxLength(4);
        builder.Property(x => x.kind).HasConversion<string>().IsRequired();
        builder.Property(x => x.categoryId);
        builder.HasOne(x => x.category).WithMany(x => x.transactions).HasForeignKey(x => x.categoryId);
    }
}