using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.Roles;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Infrastructure.Roles
{
    internal class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Configure table name
            builder.ToTable("Roles");

            // Configure primary key
            builder.HasKey(u => u.Id);

            // Configure properties
            builder.Property(u => u.Id)
                .HasConversion(id => id.AsString(), str => new RoleId(str));

            builder.Property(u => u.Description)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Description")
                .HasConversion(description => description.AsString(), str => new Description(str));

            // Configure indexes
            builder.HasIndex(u => u.Description).IsUnique();
        }
    }
}