using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.Service.Interfaces;

public interface IDocumentService
{
    Task<List<Document>> GetAllAsync();
    Task<Document?> GetByIdAsync(int id);
    Task<List<Document>> GetByContractIdAsync(int contractId);
    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(int id);
}