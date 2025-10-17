using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PIC_CURSACH.Model.entity;
using PIC_CURSACH.Service.Interfaces;
using PIC_CURSACH.ViewModel.entityDetail;

namespace PIC_CURSACH.View.entityDetails
{
    public partial class DepositTypeDetailsWindow : Window
    {
        private readonly IDepositTypeService _depositTypeService;
        private DepositType _depositType;
        private DepositTypeViewModel _viewModel;

        public DepositTypeDetailsWindow(DepositType selectedDepositType)
        {
            InitializeComponent();

            IServiceProvider serviceProvider = App.CurrentServiceConfigurator.Services;
            _depositTypeService = serviceProvider.GetRequiredService<IDepositTypeService>();

            Loaded += async (s, e) => await LoadDepositTypeAsync(selectedDepositType.TypeId);
        }

        private async Task LoadDepositTypeAsync(int typeId)
        {
            try
            {
                _depositType = await _depositTypeService.GetByIdAsync(typeId);

                if (_depositType == null)
                {
                    MessageBox.Show("Тип депозита не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                var contracts = _depositType.DepositContracts?.ToList() ?? new List<DepositContract>();

                _viewModel = new DepositTypeViewModel
                {
                    TypeId = _depositType.TypeId,
                    Name = _depositType.Name,
                    InterestRate = _depositType.InterestRate,
                    MinAmount = _depositType.MinAmount,
                    TermDays = _depositType.TermDays,
                    DepositContracts = new ObservableCollection<DepositContract>(contracts)
                };

                DataContext = _viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveButton.IsEnabled = false;

                // Проверяем обязательные поля
                if (string.IsNullOrWhiteSpace(_viewModel.Name))
                {
                    MessageBox.Show("Введите название типа депозита!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_viewModel.InterestRate <= 0)
                {
                    MessageBox.Show("Процентная ставка должна быть больше 0!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_viewModel.MinAmount < 0)
                {
                    MessageBox.Show("Минимальная сумма не может быть отрицательной!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_viewModel.TermDays <= 0)
                {
                    MessageBox.Show("Срок должен быть больше 0 дней!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Обновляем поля
                _depositType.Name = _viewModel.Name;
                _depositType.InterestRate = _viewModel.InterestRate;
                _depositType.MinAmount = _viewModel.MinAmount;
                _depositType.TermDays = _viewModel.TermDays;

                // Сохраняем
                await _depositTypeService.UpdateAsync(_depositType);

                MessageBox.Show("Изменения успешно сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                var errorMessage = $"Ошибка при сохранении:\n\n{ex.Message}";

                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nВнутренняя ошибка:\n{ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
