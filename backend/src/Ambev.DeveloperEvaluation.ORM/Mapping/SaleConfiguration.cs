using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

    builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleDate).IsRequired();
        builder.Property(s => s.CustomerId).IsRequired();
        builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Branch).IsRequired().HasMaxLength(100);
        builder.Property(s => s.IsCancelled).IsRequired();


        builder.Property(s => s.TotalAmount)
            .HasConversion(
                v => v.Value,
                v => new Money(v)
            )
            .HasColumnName("TotalAmount")
            .HasPrecision(18, 2);

        builder.HasMany(s => s.SaleItems)
               .WithOne()
               .HasForeignKey("SaleId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.SaleItems)
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
