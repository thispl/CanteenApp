using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using CanteenManagementSystem.CanteenManagement.Entities;

namespace CanteenManagementSystem.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class CanteenManagementSystemDbContext :
    AbpDbContext<CanteenManagementSystemDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<Employee> Employees { get; set; }
    public DbSet<CanteenCheckIn> CanteenCheckIns { get; set; }
    public DbSet<SyncState> SyncStates { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<TimeSchedule> TimeSchedules { get; set; }
    public DbSet<Designation> Designations { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Device> Devices { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public CanteenManagementSystemDbContext(DbContextOptions<CanteenManagementSystemDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        
        /* Configure your own tables/entities inside here */

        builder.Entity<Employee>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "Employees", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.EmployeeId).IsRequired().HasMaxLength(50);
            b.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            b.Property(e => e.Department).HasMaxLength(100);
            b.HasIndex(e => e.EmployeeId).IsUnique();
        });

        builder.Entity<CanteenCheckIn>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "CanteenCheckIns", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(c => c.EmployeeId).IsRequired().HasMaxLength(50);
            b.Property(c => c.DeviceId).IsRequired().HasMaxLength(50);
            b.Property(c => c.CheckInTime).IsRequired();
            b.HasIndex(c => new { c.EmployeeId, c.DeviceId, c.CheckInTime }).IsUnique();
            b.HasIndex(c => c.CheckInTime);
        });

        builder.Entity<SyncState>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "SyncStates", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(s => s.Key).IsRequired().HasMaxLength(100);
            b.Property(s => s.LastProcessedValue).IsRequired();
            b.HasIndex(s => s.Key).IsUnique();
        });

        builder.Entity<Category>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "Categories", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);
            b.Property(c => c.CategoryCode).HasMaxLength(50);
            b.HasIndex(c => c.CategoryCode).IsUnique();
        });

        builder.Entity<Department>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "Departments", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(d => d.Name).IsRequired().HasMaxLength(100);
            b.Property(d => d.CCCode).HasMaxLength(50);
            b.HasIndex(d => d.CCCode).IsUnique();
        });

        builder.Entity<Item>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "Items", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(i => i.Description).IsRequired().HasMaxLength(200);
            b.Property(i => i.Price).IsRequired().HasPrecision(18, 2);
        });

        builder.Entity<TimeSchedule>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "TimeSchedules", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(t => t.Name).IsRequired().HasMaxLength(100);
            b.Property(t => t.Code).HasMaxLength(50);
            b.Property(t => t.StartTime).IsRequired();
            b.Property(t => t.EndTime).IsRequired();
            b.HasIndex(t => t.Code).IsUnique();
        });

        builder.Entity<Designation>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "Designations", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(d => d.Title).IsRequired().HasMaxLength(100);
            b.Property(d => d.Code).HasMaxLength(50);
            b.Property(d => d.Description).HasMaxLength(200);
            b.HasIndex(d => d.Code).IsUnique();
        });

        builder.Entity<Company>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "Companies", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(c => c.Name).IsRequired().HasMaxLength(200);
            b.Property(c => c.Code).HasMaxLength(50);
            b.Property(c => c.Address).HasMaxLength(500);
            b.Property(c => c.Phone).HasMaxLength(50);
            b.Property(c => c.Email).HasMaxLength(100);
            b.Property(c => c.TaxNumber).HasMaxLength(100);
            b.Property(c => c.Website).HasMaxLength(200);
            b.HasIndex(c => c.Code).IsUnique();
        });

        builder.Entity<Device>(b =>
        {
            b.ToTable(CanteenManagementSystemConsts.DbTablePrefix + "Devices", CanteenManagementSystemConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(d => d.DeviceId).IsRequired().HasMaxLength(100);
            b.Property(d => d.Name).IsRequired().HasMaxLength(100);
            b.Property(d => d.IpAddress).HasMaxLength(50);
            b.Property(d => d.Location).HasMaxLength(200);
            b.Property(d => d.Model).HasMaxLength(100);
            b.Property(d => d.SerialNumber).HasMaxLength(100);
            b.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
            b.HasIndex(d => d.DeviceId).IsUnique();
        });
    }
}
