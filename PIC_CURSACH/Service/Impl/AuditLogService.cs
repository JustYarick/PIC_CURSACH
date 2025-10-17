using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Service.Impl;

public class AuditLogService : IAuditLogService
{
    private readonly DepositContext _context;

    public AuditLogService(DepositContext context)
    {
        _context = context;
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }

    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        return await _context.AuditLogs.FindAsync(id);
    }

    public async Task<List<AuditLog>> GetByTableNameAsync(string tableName)
    {
        return await _context.AuditLogs
            .Where(a => a.TableName == tableName)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }

    public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.AuditLogs
            .Where(a => a.ChangedAt >= startDate && a.ChangedAt <= endDate)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync();
    }

    public async Task AddAsync(AuditLog log)
    {
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}