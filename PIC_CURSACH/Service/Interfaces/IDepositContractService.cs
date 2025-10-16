using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.Service.Interfaces;

public interface IDepositContractService
{
    Task<List<DepositContract>> GetAllAsync();
    Task<DepositContract?> GetByIdAsync(int id);
    Task AddAsync(DepositContract contract);
    Task UpdateAsync(DepositContract contract);
    Task DeleteAsync(int id);
}
