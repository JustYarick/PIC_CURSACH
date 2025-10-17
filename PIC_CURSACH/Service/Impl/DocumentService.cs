using Microsoft.EntityFrameworkCore;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.Service.Impl;

public class DocumentService : IDocumentService
{
    private readonly DepositContext _context;

    public DocumentService(DepositContext context)
    {
        _context = context;
    }

    public async Task<List<Document>> GetAllAsync()
    {
        return await _context.Documents
            .Include(d => d.Contract)
            .ToListAsync();
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.Contract)
            .FirstOrDefaultAsync(d => d.DocumentId == id);
    }

    public async Task<List<Document>> GetByContractIdAsync(int contractId)
    {
        return await _context.Documents
            .Where(d => d.ContractId == contractId)
            .ToListAsync();
    }

    public async Task AddAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Document document)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document != null)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}