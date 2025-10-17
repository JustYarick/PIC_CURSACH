using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.ViewModel.entityDetail
{
    public class DepositContractViewModel : INotifyPropertyChanged
    {
        public int ContractId { get; set; }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(nameof(Amount)); }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(nameof(StartDate)); }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(nameof(EndDate)); }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public ObservableCollection<Client> AvailableClients { get; set; }
        public ObservableCollection<Employee> AvailableEmployees { get; set; }
        public ObservableCollection<Branch> AvailableBranches { get; set; }
        public ObservableCollection<DepositType> AvailableDepositTypes { get; set; }

        private int? _selectedClientId;
        public int? SelectedClientId
        {
            get => _selectedClientId;
            set
            {
                _selectedClientId = value;
                OnPropertyChanged(nameof(SelectedClientId));
            }
        }

        private int? _selectedEmployeeId;
        public int? SelectedEmployeeId
        {
            get => _selectedEmployeeId;
            set
            {
                _selectedEmployeeId = value;
                OnPropertyChanged(nameof(SelectedEmployeeId));
            }
        }

        private int? _selectedBranchId;
        public int? SelectedBranchId
        {
            get => _selectedBranchId;
            set
            {
                _selectedBranchId = value;
                OnPropertyChanged(nameof(SelectedBranchId));
            }
        }

        private int? _selectedDepositTypeId;
        public int? SelectedDepositTypeId
        {
            get => _selectedDepositTypeId;
            set
            {
                _selectedDepositTypeId = value;
                OnPropertyChanged(nameof(SelectedDepositTypeId));
            }
        }

        public ObservableCollection<Document> Documents { get; set; }
        public ObservableCollection<DepositOperation> DepositOperations { get; set; }

        // Новые свойства для управления UI
        private bool _isEditable = true;
        public bool IsEditable
        {
            get => _isEditable;
            set { _isEditable = value; OnPropertyChanged(nameof(IsEditable)); }
        }

        private string _closeButtonText = "Отмена";
        public string CloseButtonText
        {
            get => _closeButtonText;
            set { _closeButtonText = value; OnPropertyChanged(nameof(CloseButtonText)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
