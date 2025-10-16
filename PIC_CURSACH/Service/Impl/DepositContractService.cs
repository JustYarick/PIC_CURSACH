using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Service.Impl;

public class DepositContractService(DepositContext context) : IDepositContractService
{
    public async Task<List<DepositContract>> GetAllAsync()
        => await context.DepositContracts.AsNoTracking().Include(x => x.Client).Include(x => x.DepositType)
            .Include(x => x.Employee).Include(x => x.Branch).ToListAsync();
    public async Task<DepositContract?> GetByIdAsync(int id)
        => await context.DepositContracts.FindAsync(id);
    public async Task AddAsync(DepositContract contract)
    {
        context.DepositContracts.Add(contract);
        await context.SaveChangesAsync();
    }
    public async Task UpdateAsync(DepositContract contract)
    {
        context.DepositContracts.Update(contract);
        await context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var contract = await context.DepositContracts.FindAsync(id);
        if (contract != null)
        {
            context.DepositContracts.Remove(contract);
            await context.SaveChangesAsync();
        }
    }
}
