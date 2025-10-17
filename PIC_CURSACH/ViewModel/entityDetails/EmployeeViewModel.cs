using System.Collections.ObjectModel;
using System.ComponentModel;
using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.ViewModel.entityDetail
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        public int EmployeeId { get; set; }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(nameof(FirstName)); }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(nameof(LastName)); }
        }

        private string _position;
        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(nameof(Position)); }
        }

        public ObservableCollection<DepositContract> DepositContracts { get; set; }

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