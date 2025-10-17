using System.Windows;
using System.Windows.Controls;

namespace PIC_CURSACH.View;

public partial class AdminWindow : Window
{
    private readonly HashSet<string> _loadedTabs = new();

    public AdminWindow()
    {
        InitializeComponent();

        // Загружаем первую вкладку при открытии окна
        LoadTab(ClientsTab);
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl tabControl && tabControl.SelectedItem is TabItem selectedTab)
        {
            LoadTab(selectedTab);
        }
    }

    private void LoadTab(TabItem tab)
    {
        if (_loadedTabs.Contains(tab.Name)) return;

        switch (tab.Name)
        {
            case "ClientsTab":
                tab.Content = new controls.ClientsTableControl
                {
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true
                };
                break;

            case "EmployeesTab":
                tab.Content = new controls.EmployeesTableControl
                {
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true
                };
                break;

            case "OperationsTab":
                tab.Content = new controls.DepositOperationsTableControl
                {
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true
                };
                break;

            case "TypesTab":
                tab.Content = new controls.DepositTypesTableControl
                {
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true
                };
                break;

            case "ContractsTab":
                tab.Content = new controls.DepositContractsTableControl
                {
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true
                };
                break;
            case "DocumentsTab":
                tab.Content = new controls.DocumentsTableControl
                {
                    CanAdd = true,
                    CanEdit = true,
                    CanDelete = true
                };
                break;

            case "AuditTab":
                tab.Content = new controls.AuditLogsTableControl();
                break;
        }

        _loadedTabs.Add(tab.Name);
    }
}
