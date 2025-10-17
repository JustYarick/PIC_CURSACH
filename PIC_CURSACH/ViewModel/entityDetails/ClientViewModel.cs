using System.Collections.ObjectModel;
using System.ComponentModel;
using PIC_CURSACH.Model.entity;

namespace PIC_CURSACH.ViewModel.entityDetail
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        public int ClientId { get; set; }

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

        private string _passport;
        public string Passport
        {
            get => _passport;
            set { _passport = value; OnPropertyChanged(nameof(Passport)); }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(nameof(Phone)); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
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
