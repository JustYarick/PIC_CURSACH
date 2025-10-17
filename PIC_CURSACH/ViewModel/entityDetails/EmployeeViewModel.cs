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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}