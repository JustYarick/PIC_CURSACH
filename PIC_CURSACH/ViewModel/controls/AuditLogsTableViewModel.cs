using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;

namespace PIC_CURSACH.ViewModel.controls;

public partial class AuditLogsTableViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<AuditLog> auditLogs = new();
    [ObservableProperty] private AuditLog? selectedAuditLog;

    private readonly IAuditLogService _auditLogService;

    public AuditLogsTableViewModel(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    public async Task LoadDataAsync()
    {
        var data = await _auditLogService.GetAllAsync();
        AuditLogs = new ObservableCollection<AuditLog>(data);
    }
}