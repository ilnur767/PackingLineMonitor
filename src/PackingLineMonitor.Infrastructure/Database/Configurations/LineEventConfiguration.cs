using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackingLineMonitor.Domain;

namespace PackingLineMonitor.Infrastructure.Database.Configurations;

public class LineEventConfiguration : IEntityTypeConfiguration<LineEvent>
{
    public void Configure(EntityTypeBuilder<LineEvent> builder)
    {
        builder.ToTable("line_events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Timestamp)
            .HasColumnName("timestamp");

        builder.Property(x => x.Status)
            .HasConversion(l => l.ToString(), l => Enum.Parse<LineStatus>(l))
            .HasColumnName("status");
    }
}