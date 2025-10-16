using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.Service.Interfaces;

public interface IDepositTypeService
{
    Task<List<DepositType>> GetAllAsync();
    Task<DepositType?> GetByIdAsync(int id);
    Task AddAsync(DepositType depositType);
    Task UpdateAsync(DepositType depositType);
    Task DeleteAsync(int id);
}