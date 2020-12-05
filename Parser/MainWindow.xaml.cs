using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Parser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           
            InitializeComponent();

            try
            {
                //Создаем объект ViewModel
                AppViewModel appViewModel = new AppViewModel(new Config(), new BinConfMan(), new FileListener());

                //Запускием процесс обработки файлов
                appViewModel.automation.Start();

                //Определяем контекст данных приложения
                DataContext = appViewModel;
            }
            catch (Exception err)
            {
                MessageBox.Show($"{err.Message}", "Parser v2", button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }

        }

    }
}
