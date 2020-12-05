using System;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Contract;

namespace Parser
{
    //Класс с описанием уровня ViewModel 
    public class AppViewModel : INotifyPropertyChanged
    {

        //Поле для хранения ссылки на экземпляр класса, реализующего интерфейc IConfig
        private IConfig config;
        //Поле для хранения ссылки на экземпляр класса, реализующего интерфейc IConfigManager
        private IConfigManager configManager;
        //Поле для хранения ссылки на экземпляр класса, реализующего интерфейc IFileListener
        private IFileListener fileListener;

        //Поле для хранения значения часов
        private int hH;
        //Поле для хранения значения минут
        private int mM;
        //Поле для хранения значения секунд
        private int sS;
        //Поле для хранения значения высоты экспандера
        private int gridHeigth=40;
        //Поле для хранения значения рабочего каталога
        private string workDirectory;
        //Поле для хранения значения типа обработки файлов (DispatcherTimer или FileSystemWatcher)
        private bool processingType;
        //Поле для хранения значения надписи на кнопке выбора типа обработки фалов 
        private string processingTypeButtonContent;

        //Поле для хранения ссылки на метод, выполняемый при смене интервала обработки файлов
        private Command changeInterval;
        //Поле для хранения ссылки на метод, выполняемый при смене рабочего каталога
        private Command changeDirectory;
        //Поле для хранения ссылки на метод, выполняемый при смене типа обработки файлов
        private Command changeProcessingType;
        //Поле для хранения ссылки на метод, выполняемый при развертывании экспандера
        private Command doExpanded;
        //Поле для хранения ссылки на метод, выполняемый при свертывании экспандера
        private Command doCollapsed;

        //Свойство со ссылкой на массв значей часов
        public int[] HHDS { get; private set; }
        //Свойство со ссылкой на массв значей минут/секунд
        public int[] MSDS { get; private set; }

        //Поле для хранения ссылки на экземпляр класса, реализующего интерфейc IAutomation
        public IAutomation automation;

        //Свойство со ссылкой на коллекцию с обрабатываемыми данными
        public ObservableCollection<IRecord> Records { get; set; }
        //Свойство со ссылкой на коллекцию с перечнем обрабатываемх файлов
        public ObservableCollection<IProcFile> ProcFiles { get; set; }

        //Конструктор класса AppViewModel
        public AppViewModel(IConfig config, IConfigManager configManager, IFileListener fileListener)
        {
            try
            {

                //инициализируем значения текущей конфигурации
                this.configManager = configManager;
                this.fileListener = fileListener;
                this.config = configManager.ReadConfig(config);
                
                //Инициализируем колекцию обрабатываемых данных
                Records = new ObservableCollection<IRecord>();

                //Инициализируем колекцию с перечнем обрабатываемх файлов
                ProcFiles = new ObservableCollection<IProcFile>();

                //Создаем екземпляр класса Automation
                automation = new Automation(this.fileListener, this.config, Records, ProcFiles);
            }
            catch
            {
                //Отлавливаем исключения и передаем их на верхний уровень обработки
                throw;
            }

            //Устанавливаем значения свойств
            //Инициализируем заполняя значениями массивы с перечнем допустимых для ввода значний 
            //для листбоксов часы, минуты, секунды
            DataInit dataInit = new DataInit();
            HHDS = dataInit.HR;
            MSDS = dataInit.MS;
            //Устанавливаем начальные значения в листбоксах: часы, минуты, секунды, 
            //заполняя их значениями из текущей конфигурации
            HHVal = this.config.Interval.Hours;
            MMVal = this.config.Interval.Minutes;
            SSVal = this.config.Interval.Seconds;
            WorkDirectory = this.config.WorkDir;
            ProcessingType = this.config.ProcessingType;
            //Задаем значение надписи на кнопке выбора типа обработки фалов в зависимости от значения свойства ProcessingType
            if (ProcessingType)
                ProcessingTypeButtonContent = "Timer is tarted";
            else
                ProcessingTypeButtonContent = "File system watcher is started";
        }
        
        //Свойство для значения часов
        public int HHVal
        {
            get { return hH; }
            set
            {
                hH = value;
                OnPropertyChanged("HRVal");
            }
        }

        //Свойство для значения минут
        public int MMVal
        {
            get { return mM; }
            set
            {
                mM = value;
                OnPropertyChanged("MMVal");
            }
        }

        //Свойство для значения секунд
        public int SSVal
        {
            get { return sS; }
            set
            {
                sS = value;
                OnPropertyChanged("SSVal");
            }
        }

        //Свойство для значения высоты экспандера
        public int GridHiegthVal
        {
            get { return gridHeigth; }
            set
            {
                gridHeigth = value;
                OnPropertyChanged("GridHiegthVal");
            }
        }

        //Свойство для значения рабочего каталога
        public string WorkDirectory
        {
            get { return workDirectory; }
            set
            {
                workDirectory = value;
                OnPropertyChanged("Workdirectory");
            }
        }

        //Свойство для значения надписи на кнопке выбора типа обработки файлов
        public string ProcessingTypeButtonContent
        {
            get { return processingTypeButtonContent; }
            set
            {
                processingTypeButtonContent = value;
                OnPropertyChanged("ProcessingTypeButtonContent");
            }
        }
        
        //Свойство для значения типа обработки файлов
        public bool ProcessingType
        {
            get { return processingType; }
            set
            {
                processingType = value;
                OnPropertyChanged("ProcessingType");
            }
        }

        //Команда выполняемая при нажатии кнопки Set
        public Command ChangeInterval
        {
            get
            {
                return changeInterval ??
                      (changeInterval = new Command((o) => 
                      {
                          TimeSpan oldInterval = config.Interval;
                          TimeSpan newInterval = new TimeSpan(hH, mM, sS);
                          try
                          {
                              automation.Stop();
                              if (newInterval > new TimeSpan(0, 0, 0))
                              {
                                  config.Interval = newInterval;
                                  automation.Start();
                                  configManager.SaveConfig(config);
                              }
                              else
                              {
                                  config.ProcessingType = false;
                                  automation.Start();
                                  configManager.SaveConfig(config);
                              }
                          }
                          catch (Exception e)
                          {
                              config.Interval = oldInterval;
                              automation.Start();
                              throw new Exception($"Interval change error: {e.Message}");
                          }
                      }));
            }
        }

        //Команда выполняемая при нажатии кнопки Select
        public Command ChangeDirectory
        {
            get
            {
                return changeDirectory ??
                      (changeDirectory = new Command((o) =>
                      {
                          string oldDirectory = config.WorkDir;

                          FolderBrowserDialog fbDlg = new FolderBrowserDialog()
                          {
                              SelectedPath = oldDirectory
                          };

                          DialogResult drlt = fbDlg.ShowDialog();

                          try
                          {
                              if (drlt == DialogResult.OK)
                              {

                                  try
                                  {
                                      if (fbDlg.SelectedPath != null)
                                      {
                                          automation.Stop();
                                          config.WorkDir = fbDlg.SelectedPath + @"\";
                                          WorkDirectory = config.WorkDir;
                                          automation.Start();
                                          configManager.SaveConfig(config);
                                      }

                                  }

                                  catch (Exception e)
                                  {
                                      config.WorkDir = oldDirectory;
                                      automation.Start();

                                      throw new Exception($"Directory change error: {e.Message}");
                                  }

                              }
                              else if (drlt == DialogResult.Cancel)
                              {
                                  config.WorkDir = oldDirectory;
                                  automation.Start();
                              }
                          }

                          catch (Exception e)
                          {
                              throw new Exception($"Directory change error: {e.Message}");
                          }

                      }));
            }
        }

        //Команда выполняемая при нажатии кнопки выбора типа обработки файлов
        public Command ChangeProcessType
        {
            get
            {
                return changeProcessingType ??
                      (changeProcessingType = new Command((o) =>
                      {
                          bool oldProcessingType = processingType;

                          try
                          {
                              automation.Stop();
                              if (processingType)
                              {
                                  config.ProcessingType = processingType;
                                  ProcessingTypeButtonContent = "Timer is started";

                              }
                              else
                              {
                                  config.ProcessingType = processingType;
                                  ProcessingTypeButtonContent = "File system watcher is started";
                              }


                              automation.Start();
                              configManager.SaveConfig(config);

                          }
                          catch (Exception e)
                          {
                              config.ProcessingType = oldProcessingType;
                              automation.Start();
                              throw new Exception($"Process type change error: {e.Message}");
                          }
                      }));
            }
        }

        //Команда выполняемая при нажатии кнопки при развертывании экспандера
        public Command Expanded
        {
            get
            {
                return doExpanded ?? (doExpanded = new Command((o)=>
                {
                    GridHiegthVal = 125;
                }));
            }
        }

        //Команда выполняемая при нажатии кнопки при свертывании экспандера
        public Command Collapsed
        {
            get
            {
                return doCollapsed ?? (doCollapsed = new Command((o) =>
                {
                    GridHiegthVal = 40;
                }));
            }
        }

        //Делегат события
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChangedEventHandler handler = Interlocked.CompareExchange(ref PropertyChanged, null, null);
            handler?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
