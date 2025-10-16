using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.Service.Interfaces;

public interface IDepositOperationService
{
    Task<List<DepositOperation>> GetAllAsync();
    Task<DepositOperation?> GetByIdAsync(int id);
    Task AddAsync(DepositOperation operation);
    Task UpdateAsync(DepositOperation operation);
    Task DeleteAsync(int id);
}