using System.Windows;
using System.Windows.Controls;
using PIC_CURSACH.View;

namespace PIC_CURSACH
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ClientsBtn.Click += (s, e) =>
            {
                MainContentFrame.Content = new ClientsUserControl();
            };

            DepositsBtn.Click += (s, e) =>
            {
                MainContentFrame.Content = new TextBlock { Text = "Модуль Депозиты в разработке..." };
            };

            MainContentFrame.Content = new ClientsUserControl();
        }
    }
}