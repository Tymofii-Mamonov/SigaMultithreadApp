using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace SigaMultithreadApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Manager> _managers = new List<Manager>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var inputText = DataSuppliersNumber.Text;
            var instancesNumberParsed = int.TryParse(inputText, out var instancesNumber);
            if (instancesNumberParsed && instancesNumber > 0)
            {
                var manager = new Manager(1000, 2500, instancesNumber);
                _managers.Add(manager);
                manager.NewDataProcessed += AppendMessage;
                await Task.Run(manager.Start);
            }
            else
            {
                OutputTextBox.Text += $"ERROR: The input entered {inputText} is not a Real number\r\n";
            }
        }

        private void AppendMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                OutputTextBox.Text += $"{message}\r\n";
            });

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var manager in _managers)
            {
                manager.Stop();
            }
        }
    }
}
