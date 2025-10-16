using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.Service.Interfaces;

public interface IBranchService
{
    Task<List<Branch>> GetAllAsync();
    Task<Branch?> GetByIdAsync(int id);
    Task AddAsync(Branch branch);
    Task UpdateAsync(Branch branch);
    Task DeleteAsync(int id);
}