using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.Service.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
    Task<bool> ExistsByIdAsync(int id);
}