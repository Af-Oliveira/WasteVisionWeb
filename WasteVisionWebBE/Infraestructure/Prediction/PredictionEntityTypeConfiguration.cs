using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.Predictions;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.ObjectPredictions;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Infrastructure.Predictions
{
    internal class PredictionEntityTypeConfiguration : IEntityTypeConfiguration<Prediction>
    {
        public void Configure(EntityTypeBuilder<Prediction> builder)
        {
            // Configure table name
            builder.ToTable("Predictions");

            // Configure primary key
            builder.HasKey(p => p.Id);

            // Configure properties
            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.AsString(),
                    str => new PredictionId(str));

            builder.Property(p => p.OriginalImageUrl)
                .IsRequired()
                .HasMaxLength(1024)
                .HasColumnName("OriginalImageUrl")
                .HasConversion(originalImageUrl => originalImageUrl.AsString(), str => new Url(str));

            builder.Property(p => p.ProcessedImageUrl)
                .IsRequired()
                .HasMaxLength(1024)
                .HasColumnName("ProcessedImageUrl")
                .HasConversion(processedImageUrl => processedImageUrl.AsString(), str => new Url(str));

            builder.Property(p => p.Date)
                .IsRequired()
                .HasColumnName("Date")
                .HasConversion(date => date.Value, value => new Date(value));

            builder.HasOne<User>(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<RoboflowModel>(p => p.RoboflowModel)
                .WithMany()
                .HasForeignKey(p => p.RoboflowModelId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<ObjectPrediction>(p => p.ObjectPredictions)
                .WithOne(op => op.Prediction) // Assuming ObjectPrediction has Prediction navigation property
                .HasForeignKey(op => op.PredictionId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure indexes for better query performance
            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.RoboflowModelId);
            builder.HasIndex(p => p.Date);
        }
    }
}
