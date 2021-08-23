using System;
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
                try
                {
                    var manager = new Manager(1000, 1500, instancesNumber);
                    _managers.Add(manager);
                    manager.NewDataProcessed += AppendMessage;
                    await Task.Run(manager.Start);
                }
                catch (ArgumentOutOfRangeException)
                {
                    OutputTextBox.Text += "ERROR: the minRandom is greater than maxRandom\r\n";
                }
                catch (Exception exception)
                {
                    OutputTextBox.Text += $"ERROR: {exception.Message}";
                }
            }
            else
            {
                OutputTextBox.Text += $"ERROR: The input entered {inputText} is not a Real number\r\n";
            }
        }

        private void AppendMessage(ProcessedMessage processedMessage)
        {
            Dispatcher.Invoke(() =>
            {
                var output = "=====================================\r\n" +
                                $"Message Description: {processedMessage.Message.MessageText}\r\n" +
                                $"Message ID: {processedMessage.Message.Id}\r\n" +
                                $"Message SentTime: {processedMessage.Message.SentTime}\r\n" +
                                $"Processing Time: {processedMessage.ProcessingTime.Seconds}s {processedMessage.ProcessingTime.Milliseconds}ms\r\n" +
                                $"Processing Ended: {processedMessage.EndProcessingTime}\r\n" +
                                "=====================================\r\n";
                OutputTextBox.Text += output;
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
