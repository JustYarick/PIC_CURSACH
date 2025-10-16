using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Service.Impl;

public class DepositOperationService(DepositContext context) : IDepositOperationService
{

    public async Task<List<DepositOperation>> GetAllAsync()
    {
        return await context.DepositOperations
            .Include(x => x.DepositContract)
            .ToListAsync();
    }

    public async Task<DepositOperation?> GetByIdAsync(int id)
    {
        return await context.DepositOperations.FindAsync(id);
    }

    public async Task AddAsync(DepositOperation operation)
    {
        context.DepositOperations.Add(operation);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DepositOperation operation)
    {
        context.DepositOperations.Update(operation);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var operation = await context.DepositOperations.FindAsync(id);
        if (operation != null)
        {
            context.DepositOperations.Remove(operation);
            await context.SaveChangesAsync();
        }
    }
}