using Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration
{
    public class AssetEntityConfiguration : IEntityTypeConfiguration<Asset>
    {
        //private static Expression<Func<Package, object>> IndexColumns { get; } = a => new { a.Source, a.StoreNumber, a.Sequence };
        //public static string IndexName = "idx_packages_unique_source_storenumber_sequence";

        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.ToTable("Assets");
            builder.UseXminAsConcurrencyToken();
            builder.Property(c => c.Name).HasMaxLength(50).IsConcurrencyToken();

            //builder.HasMany(m => m.Contents)
            //       .WithOne()
            //       .HasForeignKey(fk => fk.PackageId)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder
            //    .HasIndex(IndexColumns)
            //    .HasName(IndexName)
            //    .IsUnique();
        }
    }
}
