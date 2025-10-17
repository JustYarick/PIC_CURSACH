using System.Collections.ObjectModel;
using System.ComponentModel;
using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.ViewModel.entityDetail
{
    public class DepositTypeViewModel : INotifyPropertyChanged
    {
        public int TypeId { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private decimal _interestRate;
        public decimal InterestRate
        {
            get => _interestRate;
            set { _interestRate = value; OnPropertyChanged(nameof(InterestRate)); }
        }

        private decimal _minAmount;
        public decimal MinAmount
        {
            get => _minAmount;
            set { _minAmount = value; OnPropertyChanged(nameof(MinAmount)); }
        }

        private int _termDays;
        public int TermDays
        {
            get => _termDays;
            set { _termDays = value; OnPropertyChanged(nameof(TermDays)); }
        }

        public ObservableCollection<DepositContract> DepositContracts { get; set; }

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