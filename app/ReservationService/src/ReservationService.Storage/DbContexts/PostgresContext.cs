using Common.Models.Enums;
using ReservationService.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace ReservationService.Storage.DbContexts;

public class PostgresContext(DbContextOptions<PostgresContext> options) : DbContext(options)
{
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Указать, как сохранять enum в базу данных
        modelBuilder.Entity<Reservation>()
            .Property(r => r.Status)
            .HasConversion<string>() // Сохраняем как строку
            .HasMaxLength(20); // Задаем максимальную длину

        // Опционально, можно установить ограничения на уровне базы данных
        modelBuilder.Entity<Reservation>()
            .ToTable(t => t.HasCheckConstraint("CHK_Reservation_Status", "\"Status\" IN ('RENTED', 'RETURNED', 'EXPIRED')"));
    }
}