using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

    builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.ProductDescription).IsRequired().HasMaxLength(100);
        builder.Property(i => i.IsCancelled).IsRequired();

        builder.Property(i => i.Quantity)
            .HasConversion(
                v => v.Value,
                v => new Quantity(v)
            )
            .HasColumnName("Quantity");

        builder.Property(i => i.UnitPrice)
            .HasConversion(
                v => v.Value,
                v => new Money(v)
            )
            .HasColumnName("UnitPrice")
            .HasPrecision(18, 2);


        builder.Property(i => i.Discount)
            .HasConversion(
                v => v.Value,
                v => new Percentage(v)
            )
            .HasColumnName("Discount")
            .HasPrecision(18, 2);


        builder.Property(i => i.TotalAmount)
            .HasConversion(
                v => v.Value,
                v => new Money(v)
            )
            .HasColumnName("TotalAmount")
            .HasPrecision(18, 2);
    }
}
