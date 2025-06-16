using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.ObjectPredictions;
using DDDSample1.Domain.Predictions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Infrastructure.ObjectPredictions
{
    internal class ObjectPredictionEntityTypeConfiguration : IEntityTypeConfiguration<ObjectPrediction>
    {
        public void Configure(EntityTypeBuilder<ObjectPrediction> builder)
        {
            builder.ToTable("ObjectPredictions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.AsString(),
                    str => new ObjectPredictionId(str));

            

            builder.Property(p => p.X)
                .IsRequired()
                .HasColumnName("X")
                .HasConversion(x => x.Value, value => new NumberDouble(value));

            builder.Property(p => p.Y)
                .IsRequired()
                .HasColumnName("Y")
                .HasConversion(y => y.Value, value => new NumberDouble(value));

            builder.Property(p => p.Width)
                .IsRequired()
                .HasColumnName("Width")
                .HasConversion(width => width.Value, value => new NumberDouble(value));

            builder.Property(p => p.Height)
                .IsRequired()
                .HasColumnName("Height")
                .HasConversion(height => height.Value, value => new NumberDouble(value));

            builder.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Category")
                .HasConversion(category => category.AsString(), str => new Description(str));

            builder.Property(p => p.Confidence)
                .IsRequired()
                .HasColumnName("Confidence")
                .HasConversion(confidence => confidence.Value, value => new NumberDouble(value));

            // Configure relationship with navigation property
            builder.HasOne(op => op.Prediction)
                .WithMany(p => p.ObjectPredictions)
                .HasForeignKey(op => op.PredictionId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Add indexes
            builder.HasIndex(p => p.PredictionId);
            builder.HasIndex(p => p.Category);
        }
    }
}
