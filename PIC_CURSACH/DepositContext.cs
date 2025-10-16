using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH;

public class DepositContext : DbContext
{
    public DepositContext(DbContextOptions<DepositContext> options) : base(options) {}

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Branch> Branches { get; set; } = null!;
    public DbSet<DepositType> DepositTypes { get; set; } = null!;
    public DbSet<DepositContract> DepositContracts { get; set; } = null!;
    public DbSet<DepositOperation> DepositOperations { get; set; } = null!;
    public DbSet<InterestAccrual> InterestAccruals { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}