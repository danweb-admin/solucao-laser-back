using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
	public class CalendarEquipamentConsumableMapping : IEntityTypeConfiguration<CalendarEquipamentConsumable>
    {

        public void Configure(EntityTypeBuilder<CalendarEquipamentConsumable> builder)
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

            builder.Property(x => x.EquipamentId)
                .IsRequired();

            builder.Property(x => x.ConsumableId)
                .IsRequired();

            builder.Property(x => x.CalendarId)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasDefaultValue(0);

            builder.Property(c => c.Value)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.TotalValue)
                .HasColumnType("decimal(18,2)");
        }
    }
}

