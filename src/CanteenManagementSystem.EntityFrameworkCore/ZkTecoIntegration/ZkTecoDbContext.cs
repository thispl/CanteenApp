using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace CanteenManagementSystem.EntityFrameworkCore.ZkTecoIntegration;

/// <summary>
/// Secondary read-only DbContext for ZKTeco external database (easywdms).
/// Maps to external tables: personnel_employee and iclock_transaction.
/// </summary>
[ConnectionStringName("ZkTeco")]
public class ZkTecoDbContext : AbpDbContext<ZkTecoDbContext>
{
    /// <summary>
    /// External personnel_employee table from ZKTeco system
    /// </summary>
    public DbSet<ExternalEmployee> PersonnelEmployees { get; set; }

    /// <summary>
    /// External iclock_transaction table from ZKTeco system
    /// </summary>
    public DbSet<ExternalTransaction> IclockTransactions { get; set; }

    public ZkTecoDbContext(DbContextOptions<ZkTecoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ExternalEmployee - maps to personnel_employee table
        modelBuilder.Entity<ExternalEmployee>(b =>
        {
            b.ToTable("personnel_employee", "dbo");
            b.HasKey(e => e.EnrollNumber);
            b.Property(e => e.EnrollNumber)
                .HasColumnName("emp_code")
                .HasMaxLength(50);
            b.Property(e => e.Name)
                .HasColumnName("first_name")
                .HasMaxLength(100);
        });

        // Configure ExternalTransaction - maps to iclock_transaction table
        modelBuilder.Entity<ExternalTransaction>(b =>
        {
            b.ToTable("iclock_transaction", "dbo");
            b.HasKey(t => t.Id);
            b.Property(t => t.Id)
                .HasColumnName("id");
            b.Property(t => t.Pin)
                .HasColumnName("emp_code")
                .HasMaxLength(50);
            b.Property(t => t.AuthDeviceId)
                .HasColumnName("terminal_alias")
                .HasMaxLength(50);
            b.Property(t => t.AuthTime)
                .HasColumnName("punch_time");
        });
    }
}
