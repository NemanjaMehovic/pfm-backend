using Microsoft.EntityFrameworkCore;
using pfm.Database.Entities;

namespace pfm.Database.Configuration;

public class TransactionSplitsConfiguration : IEntityTypeConfiguration<TransactionSplitsEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TransactionSplitsEntity> builder)
    {
        builder.ToTable("transaction_splits");

        builder.HasKey(x => new { x.transactionId, x.categoryId });
        builder.Property(x => x.transactionId).IsRequired();
        builder.Property(x => x.categoryId).IsRequired();
        builder.HasOne(x => x.transaction).WithMany(x => x.splits).HasForeignKey(x => x.transactionId).IsRequired();
        builder.HasOne(x => x.category).WithMany(x => x.splits).HasForeignKey(x=>x.categoryId).IsRequired();
    }
}