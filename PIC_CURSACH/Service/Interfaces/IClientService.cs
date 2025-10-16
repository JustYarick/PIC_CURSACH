using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.Service.Interfaces;

public interface IClientService
{
    Task<List<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(int id);
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task DeleteAsync(int id);
    Task<bool> ExistsByPassportAsync(string passport);
}