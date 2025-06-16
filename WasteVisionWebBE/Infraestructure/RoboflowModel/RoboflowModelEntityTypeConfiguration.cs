using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.RoboflowModels;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Predictions;

namespace DDDSample1.Infrastructure.RoboflowModels
{
    internal class RoboflowModelEntityTypeConfiguration : IEntityTypeConfiguration<RoboflowModel>
    {
        public void Configure(EntityTypeBuilder<RoboflowModel> builder)
        {
            // Table
            builder.ToTable("RoboflowModels");

            // Key
            builder.HasKey(b => b.Id);
            
            // Properties
            builder.Property(b => b.Id)
                .HasConversion(
                    v => v.AsString(),
                    v => new RoboflowModelId(v));

            builder.Property(b => b.Description)
               .IsRequired()
               .HasMaxLength(255)
                .HasColumnName("Description")
                .HasConversion(description => description.AsString(), str => new Description(str));

            builder.Property(b => b.ApiKey)
               .IsRequired()
               .HasMaxLength(255)
                .HasColumnName("ApiKey")
                .HasConversion(apiKey => apiKey.AsString(), str => new ApiKey(str));
            
            builder.Property(b => b.LocalModelPath)
               .IsRequired()
               .HasMaxLength(255)
                .HasColumnName("LocalModelPath")
                .HasConversion(localModelPath => localModelPath.AsString(), str => new FilePath(str));

            builder.Property(b => b.ModelUrl)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("ModelUrl")
                .HasConversion(modelUrl => modelUrl.AsString(), str => new Url(str));
            
            builder.Property(b => b.EndPoint)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("EndPoint")
                .HasConversion(endPoint => endPoint.AsString(), str => new Url(str));
                
            builder.Property(b => b.Map)
                .IsRequired()
                .HasColumnName("Map")
                .HasConversion(map => map.Value, value => new NumberDouble(value));

            builder.Property(b => b.Recall)
                .IsRequired()
                .HasColumnName("Recall")
                .HasConversion(recall => recall.Value, value => new NumberDouble(value));

            builder.Property(b => b.Precision)
                .IsRequired()
                .HasColumnName("Precision")
                .HasConversion(precision => precision.Value, value => new NumberDouble(value));

            builder.Property(b => b.Active)
                .IsRequired();

            // ADD THIS: Configure the relationship from RoboflowModel to Predictions
            builder.HasMany<Prediction>()
                .WithOne(p => p.RoboflowModel)
                .HasForeignKey(p => p.RoboflowModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
