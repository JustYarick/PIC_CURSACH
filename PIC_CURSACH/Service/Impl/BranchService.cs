using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Service.Impl;

public class BranchService : IBranchService
{
    private readonly DepositContext _context;

    public BranchService(DepositContext context)
    {
        _context = context;
    }

    public async Task<List<Branch>> GetAllAsync()
    {
        return await _context.Branches.AsNoTracking().ToListAsync();
    }

    public async Task<Branch?> GetByIdAsync(int id)
    {
        return await _context.Branches.FindAsync(id);
    }

    public async Task AddAsync(Branch branch)
    {
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Branch branch)
    {
        _context.Branches.Update(branch);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch != null)
        {
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
        }
    }
}