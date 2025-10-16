using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Service.Impl;

public class ClientService(DepositContext context) : IClientService
{

    public async Task<List<Client>> GetAllAsync()
    {
        return await context.Clients.AsNoTracking().ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await context.Clients.FindAsync(id);
    }

    public async Task AddAsync(Client client)
    {
        context.Clients.Add(client);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Client client)
    {
        context.Clients.Update(client);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var client = await context.Clients.FindAsync(id);
        if (client != null)
        {
            context.Clients.Remove(client);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByPassportAsync(string passport)
    {
        return await context.Clients.AnyAsync(c => c.Passport == passport);
    }
}