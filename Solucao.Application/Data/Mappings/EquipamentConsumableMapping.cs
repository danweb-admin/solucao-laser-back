using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class EquipamentConsumableMapping : IEntityTypeConfiguration<EquipamentConsumable>
    {
        public void Configure(EntityTypeBuilder<EquipamentConsumable> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime");

            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .IsRequired();

            builder.Property(c => c.Value)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.EquipamentId)
                .IsRequired();

            builder.Property(x => x.ConsumableId)
                .IsRequired();
        }
    }
}

