using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Service.Impl;

public class EmployeeService(DepositContext context) : IEmployeeService
{
    public async Task<List<Employee>> GetAllAsync()
    {
        var employees = context.Employees.FromSqlRaw("SELECT * FROM public.employees").ToList();
        return await context.Employees.AsNoTracking().ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await context.Employees.FindAsync(id);
    }

    public async Task AddAsync(Employee employee)
    {
        context.Employees.Add(employee);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        context.Employees.Update(employee);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await context.Employees.FindAsync(id);
        if (employee != null)
        {
            context.Employees.Remove(employee);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByIdAsync(int id)
    {
        return await context.Employees.AnyAsync(e => e.EmployeeId == id);
    }
}