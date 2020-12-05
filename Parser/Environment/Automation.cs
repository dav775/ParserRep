using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using Contract;

namespace Parser
{
    public class Automation : IAutomation
    {
        //Импортируем тип IContract из библиотеки Contract
        [Import(typeof(IContract))]
        private IContract Contract { get; set; }

        //Поле для хранения ссылки на экземпляр класса DispatcherTimer
        private readonly DispatcherTimer DT;

        //Поле для хранения ссылки на экземпляр класса FileSystemWatcher
        private readonly FileSystemWatcher FSW;

        //Поле для хранения ссылки на объект-locker
        private readonly object Locker = new object();

        //Поле для хранения ссылки на объект-locker
        private readonly object Locker_ = new object();

        //Поле для хранения ссылки на экземпляр класса, реализующего интерфейc IFileListener
        private IFileListener FL;

        //Поле для хранения ссылки на экземпляр класса, реализующего интерфейc IConfig
        private IConfig Config;

        //Поле для хранения значения рабочего каталога
        private string WorkDir;

        //Поле для хранения ссылки на коллекцию с информацией об обрабатываемых файлах
        private List<FileInfo> FileQueue = new List<FileInfo>();

        //Поле для хранения ссылки на колекцию записей с извлекаемыми данными
        private ObservableCollection<IRecord> Records; // { get; }

        //Поле для хранения ссылки на колекцию записей обрабатываемых фалов
        private ObservableCollection<IProcFile> ProcFiles; // { get; }

        //Метод удаляет заданный файл
        private void DeleteFile(string FileName)
        {
            try
            {
                //Пробуем удалить файл, выполняя процедуру в отдельном потоке
                Task.Factory.StartNew(() => { File.Delete(FileName); });
            }

            catch (IOException err)
            {
                //Перехватываем ошибки удаления файла и передаем их на верхний уровень для обработки
                throw new Exception($"File deleting error {FileName} - ({err.Message})");
            }
        }

        //Метод, обрабатывающй файлы в рабочем каталоге
        private async Task FileProcAsync(string fullname, string shortname)
        {
            //создаем коллекцию для временного хранения данных из обрабатываемх файлов
            IList<IRecord> tmplr = new List<IRecord>();

            try
            {
                //Проверяем свободен ли файл и ожидаем его разблокировки
                if (await AwaitFileUnlock(fullname))
                {
                    //Сохраняем ID текущего потока и обнуляем индекс обрабатываемого файла коллекции
                    int CurThread = Thread.CurrentThread.ManagedThreadId;
                    int CurItem = 0;

                    //Обработчик события на изменения прогресса загрузки файла
                    void eventHandler(object obj, ProgressEventArgs ev)
                    {
                        //Проверяем, что событие получено для текущего потока 
                        if (CurThread == ev.ThreadId)
                        {
                            //Выполняем обновление данных в потоке GUI
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                                                           new Action(() =>
                                                                                           {
                                                                                               //Обновляем прогресс исполнения и информационное сообщение для обрабатываемого файла
                                                                                               ProcFiles[CurItem].Completion = ev.Progress;
                                                                                               ProcFiles[CurItem].Message = ev.Progress.ToString() + "%";
                                                                                           }));
#if DEBUG
                            Debug.WriteLine($"Из события процент загрузки файла: {fullname} - {ev.Progress}");
#endif
                        }
                    }

                    //Подписываемся на событие
                    Contract.ProgressUpdated += eventHandler;

                    try
                    {
                        //Создаем экземпляр класса ProcFile
                        ProcFile PF = new ProcFile()
                        {
                            //Устанавливаем значения его свойств
                            FileName = shortname,
                            Completion = 0
                        };

                        //Блокируем коллекцию ProcFiles на период добавления в неё новой записи
                        lock (Locker)
                        {
                            //Выполняем обновление данных в потоке GUI
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                                                                                            new Action(() =>
                                                                                            {
                                                                                                        //Добавлем элемент в колекцию с данными об обрабатываемх файлах
                                                                                                        ProcFiles.Add(PF);
                                                                                            }));
                            //Получаем ID добавленного элемента в коллекции
                            CurItem = ProcFiles.IndexOf(PF);
                        }

                        //Загружаем данные из файла
                        tmplr = Contract.LoadData(fullname);

                        //Удаляем обработанный файл
                        DeleteFile(fullname);

                    }
                    catch (FormatException)
                    {
                        //Выводим информационное сообщение об том, что файл содержит не верные данные
                        ProcFiles[CurItem].Message = "File has wrong data";
                        //Удаляем файл с поврежденными данными из рабочего каталога
                        DeleteFile(fullname);

                    }
                    catch (FileTypeException)
                    {
                        //Выводим информационное сообщение о том, что файл не поддерживается
                        ProcFiles[CurItem].Message = "File not supported";
                        //Удалем не поддерживаемый файл из рабочего каталога
                        DeleteFile(fullname);
                    }
                    catch
                    {
                        //Отлавливаем остальные исключения и передаем их обработку на верхний уровень
                        throw;
                    }
                    finally
                    {
                        //Отписываемся от события
                        Contract.ProgressUpdated -= eventHandler;
                    }

                    foreach (IRecord recs in tmplr)
                    {
                        //Добавляем данные из временной коллекции в рабочую коллекцию выполняя это в потоке GUI
                        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                                                    new Action(() =>
                                                                                    {
                                                                                        Records.Add(recs);
                                                                                    }));
                    }

                }

            }
            catch (Exception err)
            {
                //Отлавливаем исключения и передаем их обработку на верхний уровень
                throw new Exception($"File processing error (" + err.Message + ")");
            }

        }

        //Метод необходимо запустить один раз, сразу после запуска FileSystemWatcher,
        //он обрабатывает файлы, имеющиеся в рабочем каталоге
        private void AfterStart()
        {
            FileInfo[] DataFiles = FL.ReadFilesAtt(Config.WorkDir);

            try
            {
                foreach (FileInfo Fi in DataFiles)
                {
                    FileInfo TempFi = Fi;

                    string FullFileName = Config.WorkDir + TempFi.Name;

                    //Создаем Task чтобы передать выполнение обработки файла в отдельный поток
                    Task.Factory.StartNew(async () => { await FileProcAsync(FullFileName, TempFi.Name); });
                }
            }
            catch
            {
                throw;
            }
            

        }

        //Событие экземпляра класса DispatcherTimerа
        private void OnTick(object o, EventArgs e)
        {
            //Формируем список файлов для обработки
            FileInfo[] DataFiles = FL.ReadFilesAtt(Config.WorkDir);

            foreach (FileInfo Fi in DataFiles)
            {
                //Сохраняем информацию о текущем файле во временной переменной
                FileInfo TempFi = Fi;

                //Формируем полный путь к фалу
                string FullFileName = Config.WorkDir + TempFi.Name;

                //Таймер ведет отдельную коллекцию - очередь файлов на обработку
                //Это нужно, чтобы файлы которые уже попали в список файлов на обработку и не успели
                //обработаться до повторного срабатывания таймера, не добавлялиь в список повторно
                //Переменная count используется как индикатор наличия файла в очереди на обработку
                int Counter = 0;


                //Проверяем наличие файла в очереди на обработку по его имени,
                //если файла нет в очедери - добавляем
                //Переменная locker используется для блокировки коллекции на время добавления новой записи
                lock (Locker_)
                {
                    Counter = (from FQ in FileQueue where FQ.Name == TempFi.Name select FQ).Count();
                    if (Counter == 0) FileQueue.Add(TempFi);
                }

                //Ставим файл в очередь на обработку
                if (Counter == 0)
                {
                    //Создаем Task чтобы передать выполнение обработки файла в отдельный поток
                    Task FileLoadTask = Task.Factory.StartNew(async () => { await FileProcAsync(FullFileName, TempFi.Name); });

                    //FileLoadTask.Wait();

                    //lock (Locker)
                    //{
                    //    FileInfo _Fi = (from FQ in FileQueue where FQ.Name == TempFi.Name select FQ).First();
                    //    if (_Fi != null) FileQueue.Remove(TempFi);
                    //}

                    //Удаляем обработанный файл из очереди
                    FileLoadTask.ContinueWith(t =>
                    {
                        lock (Locker_)
                        {
                            FileInfo _Fi = (from FQ in FileQueue where FQ.Name == TempFi.Name select FQ).First();
                            if (_Fi != null) FileQueue.Remove(TempFi);
                        }
                    });

                }
            }
        }

        //Событие экземпляра класса FileSystemWatcher
        private void OnCreated(object o, FileSystemEventArgs e)
        {
            string FullFileName = e.FullPath;

            //Создаем Task чтобы передать выполнение обработки файла в отдельный поток
            Task.Factory.StartNew(async () => { await FileProcAsync(FullFileName, e.Name); });
        }

        //Метод ожидающий разблокировки заданного файла
        private Task<bool> AwaitFileUnlock(string fname)
        {
            //Создаём задачу-марионетку
            TaskCompletionSource<bool> FUnlock = new TaskCompletionSource<bool>();

            //Пробуем открыть файл на чтение-запись, 
            //при неудачной попытке ждем 0,5 секунды и пытаемся открыть снова
            while (!FileUnlocked(fname))
            {
                Task.Delay(500);
            }

            //При успешной попытке открытия файла завершаем задачу-марионетку
            //и устанавливаем для возвращаемого значения true
            FUnlock.SetResult(true);

            //возвращаем результат выполнения задачи
            return FUnlock.Task;
        }

        //Метод проверяющий разблокирован файл или нет
        private bool FileUnlocked(string fname)
        {
            try
            {
                //Пробуем открыть файл на чтение-запись, ели успешно - возвращаем true
                using (var FO = File.Open(fname, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return true;
                }
            }
            
            catch (IOException)
            {
                //Если попытка не увенчалась успехом - false
                return false;
            }
        }

        //Конструктор класса Automation
        public Automation(IFileListener fl, IConfig config, ObservableCollection<IRecord> records, ObservableCollection<IProcFile> procFiles)
        {
            try
            {
                //Выполняем импорт библиотеки Plugin
                Import.DoImport(this);

                Records = records;
                ProcFiles = procFiles;
                this.FL = fl;
                this.Config = config;

                //Создаём и инициализируем Timer
                DT = new DispatcherTimer();
                DT.Tick += OnTick;

                //Создаём и инициализируем FileSystemWatcher
                FSW = new FileSystemWatcher
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName,
                    Filter = "*.*"
                };
                FSW.Created += new FileSystemEventHandler(OnCreated);
            }
            catch
            {
                throw;
            }

        }

        //Метод запускающий обработку файлов
        public void Start()
        {
            //Получаем ссылку на рабочий каталог
            WorkDir = Config.WorkDir;

            //Если тип обработки установлен в true (используем таймер)
            if (Config.ProcessingType)
            {
                //Задаём интервал обработки и запускаем таймер
                DT.Interval = Config.Interval;
                DT.Start();
            }
            //Иначе - используем SystemFileWatcher
            else
            {
                //Указываем рабочий каталог
                FSW.Path = @"" + WorkDir;
                //Включаем обработку отслеживаемых событий в рабочем каталоге
                FSW.EnableRaisingEvents = true;
                //Выполняем обработку файлов, которые находились в рабочем каталоге до запуска
                //обработки отслеживаемых событий
                AfterStart();
            }
        }

        //Метод останавливающий обработку файлов
        public void Stop()
        {
            if (Config.ProcessingType) DT.Stop();
            else FSW.EnableRaisingEvents = false;
        }
    }
}
