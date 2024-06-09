using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
	public class HistoryMapping : IEntityTypeConfiguration<History>
	{
        public void Configure(EntityTypeBuilder<History> builder)
        {
            builder.Property(c => c.Id)
                            .HasColumnName("Id");

            builder.Property(c => c.Operation)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.OperationDate)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.RecordId)
                .IsRequired();

            builder.Property(c => c.TableName)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.Message)
                .HasColumnType("varchar(250)")
                .HasMaxLength(250);

            builder.Ignore(x => x.Active);
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.UpdatedAt);
        }
    }
}

