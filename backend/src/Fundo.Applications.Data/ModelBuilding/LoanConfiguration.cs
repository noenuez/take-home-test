using Fundo.Applications.Data.Migrations;
using Fundo.Applications.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fundo.Applications.Data.ModelBuilding;

public sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans", DatabaseSchema.Fundo);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedBy).HasMaxLength(150).IsRequired();
        builder.Property(x => x.UpdatedBy).HasMaxLength(150);

        builder.Property(x => x.ApplicantName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Amount).HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(x => x.CurrentBalance).HasColumnType("numeric(18,2)").IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ApplicantName);
    }
}
