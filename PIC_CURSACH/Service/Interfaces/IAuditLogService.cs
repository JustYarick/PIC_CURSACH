using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.Service.Interfaces;

public interface IAuditLogService
{
    Task<List<AuditLog>> GetAllAsync();
    Task<AuditLog?> GetByIdAsync(int id);
    Task<List<AuditLog>> GetByTableNameAsync(string tableName);
    Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task AddAsync(AuditLog log);
}