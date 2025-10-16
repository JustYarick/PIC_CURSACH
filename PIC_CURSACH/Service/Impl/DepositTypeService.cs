using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Service.Impl;

public class DepositTypeService : IDepositTypeService
{
    private readonly DepositContext _context;

    public DepositTypeService(DepositContext context)
    {
        _context = context;
    }

    public async Task<List<DepositType>> GetAllAsync()
    {
        return await _context.DepositTypes.ToListAsync();
    }

    public async Task<DepositType?> GetByIdAsync(int id)
    {
        return await _context.DepositTypes.FindAsync(id);
    }

    public async Task AddAsync(DepositType depositType)
    {
        _context.DepositTypes.Add(depositType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DepositType depositType)
    {
        _context.DepositTypes.Update(depositType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var depositType = await _context.DepositTypes.FindAsync(id);
        if (depositType != null)
        {
            _context.DepositTypes.Remove(depositType);
            await _context.SaveChangesAsync();
        }
    }
}