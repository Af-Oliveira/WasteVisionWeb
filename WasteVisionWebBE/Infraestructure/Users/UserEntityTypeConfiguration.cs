using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Roles;

namespace DDDSample1.Infrastructure.Users
{
    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure table name
            builder.ToTable("Users");

            // Configure primary key
            builder.HasKey(u => u.Id);

            // Configure properties
            builder.Property(u => u.Id)
                .HasConversion(id => id.AsString(), str => new UserId(str));

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasConversion(email => email.AsString(), str => new Email(str));


            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(500)
                .HasConversion(username => username.AsString(), str => new Username(str));

            builder.HasOne<Role>(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);


            // Configure indexes
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}