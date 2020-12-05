 using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Contract;
using System.Diagnostics;
using System.Globalization;


/// <summary>
/// Обрабатываем файлы треёх форматов: txt, csv и xml
/// Обработка производится синхронно или асинхронно в зависимости от используемого метода
/// 
/// </summary>

namespace Plugin
{
    //Экспортируем интерфейс IContract из библиотеки контракта
    [Export(typeof(IContract))]
    public class Plugin : IContract
    {
        //Определняем поля для хранения значения разделителя целой и дробной части
        private readonly IFormatProvider Formatter=null;// = new NumberFormatInfo { NumberDecimalSeparator = "," };
        private readonly IFormatProvider CSVFormatter = new NumberFormatInfo { NumberDecimalSeparator = "." };

        //Синхронный метод загрузки и обработки 
        public IList<IRecord> LoadData(string filepath)
        {
            //Инициализируем коллекцию в которую будут загружаться данные
            IList<IRecord> TmpRecords = new List<IRecord>();

            //Открываем поток для чтения файла 
            using (FileStream FS = new FileStream(@filepath, FileMode.Open))
            //Выполняем буферизацию считываемых данных в файловом потоке
            using (BufferedStream BS = new BufferedStream(FS, 16384))
            //Передаем данные стримридер
            using (StreamReader SR = new StreamReader(BS))
            {
               //Устанавливаем для текущего потока нейтральную культуру (актуально при обработке чисел с дробной частью идаты и времени)
               //Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
               //Определяем и обнуляем переменные прогересса
                int Progress = 0,
                    CurrentProgress = 0,
                    //Определяем и сохраняем в переменную id текущего потока 
                    ThreadId = Thread.CurrentThread.ManagedThreadId;
                //Определяем и сохраняем размер потока
                long FileLength = FS.Length;
                //Определяем переменную для обрабатываемой строки
                string Line;

                try
                {
                    //Выбираем тип обрабатываемого файла
                    switch (filepath.Substring(filepath.Length - 4, 4))
                    {

                        case ".txt":
                            //Инициализируем переменную обрабатываемой строки
                            Line = "";
                            //В цикле проходим по всем строкам, сохраненным в потоке
                            while ((Line = SR.ReadLine()) != null)
                            {
                                //Разбираем строку на массив строк-значений
                                string[] recdata = Line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                                //создаем экземпляр объекта Record и записваем в него саойства соответствующие данные массива
                                Record rec = new Record
                                {
                                    Date = Convert.ToDateTime(recdata[0]),
                                    Open = Convert.ToDouble(recdata[1], Formatter),
                                    High = Convert.ToDouble(recdata[2], Formatter),
                                    Low = Convert.ToDouble(recdata[3], Formatter),
                                    Close = Convert.ToDouble(recdata[4], Formatter),
                                    Volume = Convert.ToDouble(recdata[5], Formatter)
                                };

                                //Добавляем созданный объект в коллекцию
                                TmpRecords.Add(rec);

                                //Расчитываем прогресс загрузки
                                CurrentProgress = (int)Math.Round((double)(BS.Position * 100 / FileLength));

                                //Если прогресс увеличился генерируем событие передав данные текущего прогресса и Id потока в котором обрабатывается файл
                                if (CurrentProgress > Progress)
                                {
                                    Progress = CurrentProgress;

#if DEBUG
                                    Debug.WriteLine($"Реальный прогресс {filepath} - {Progress}");
#endif

                                    OnProgressUpdated(new ProgressEventArgs { Progress = Progress, ThreadId = ThreadId });
                                }
                            }

                            //Возвращаем результат обработки файла
                            return TmpRecords;

                        case ".csv":
                            //Инициализируем переменную обрабатываемой строки
                            Line = "";
                            //В цикле проходим по всем строкам, сохраненным в потоке
                            while ((Line = SR.ReadLine()) != null)
                            {

                                //Разбираем строку на массив строк-значений
                                string[] recdata = Line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                //создаем экземпляр объекта Record и записваем в него саойства соответствующие данные массива
                                Record rec = new Record
                                {
                                    Date = Convert.ToDateTime(recdata[0]),
                                    Open = Convert.ToDouble(recdata[1], CSVFormatter),
                                    High = Convert.ToDouble(recdata[2], CSVFormatter),
                                    Low = Convert.ToDouble(recdata[3], CSVFormatter),
                                    Close = Convert.ToDouble(recdata[4], CSVFormatter),
                                    Volume = Convert.ToDouble(recdata[5], CSVFormatter)
                                };

                                //Добавляем созданный объект в коллекцию
                                TmpRecords.Add(rec);

                                //Расчитываем прогресс загрузки
                                CurrentProgress = (int)Math.Round((double)(BS.Position * 100 / FileLength));

                                //Если прогресс увеличился генерируем событие передав данные текущего прогресса и Id потока в котором обрабатывается файл
                                if (CurrentProgress > Progress)
                                {
                                    Progress = CurrentProgress;

    #if DEBUG
                                    Debug.WriteLine($"Реальный прогресс {filepath} - {Progress}");
    #endif

                                    OnProgressUpdated(new ProgressEventArgs { Progress = Progress, ThreadId = ThreadId });
                                }
                            }

                            //Возвращаем результат обработки файла
                            return TmpRecords;

                        case ".xml":
                            //Для XML файла определяем и инициализируем переменную типа xmlreader
                            XmlReader XML = null;
                            //Настраиваем xmlreader
                            XmlReaderSettings Settings = new XmlReaderSettings
                            {
                                Async = false
                            };

                            XML = XmlReader.Create(SR, Settings);
                            while (XML.Read())
                            {
                                //создаем экземпляр объекта Record
                                Record rec = new Record();
                                //Перебираем элекменты XML документа, находим те, которые содержат атрибуты и считываем их
                                switch (XML.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        {
                                            if (XML.HasAttributes)
                                            {
                                                while (XML.MoveToNextAttribute())
                                                {
                                                    switch (XML.Name)
                                                    {
                                                        //Записваем в саойства объекта типа Record соответствующие данные
                                                        case "date":
                                                            rec.Date = Convert.ToDateTime(XML.Value);
                                                            break;

                                                        case "open":
                                                            rec.Open = Convert.ToDouble(XML.Value, Formatter);
                                                            break;

                                                        case "high":
                                                            rec.High = Convert.ToDouble(XML.Value, Formatter);
                                                            break;

                                                        case "low":
                                                            rec.Low = Convert.ToDouble(XML.Value, Formatter);
                                                            break;

                                                        case "close":
                                                            rec.Close = Convert.ToDouble(XML.Value, Formatter);
                                                            break;

                                                        case "volume":
                                                            rec.Volume = Convert.ToDouble(XML.Value, Formatter);
                                                            break;
                                                            //В случае отсутсвия атрибута с заданным именем возбуждаем исключение
                                                        default:
                                                            throw new Exception();
                                                    }
                                                }

                                                //Возвращаем результат обработки файла
                                                TmpRecords.Add(rec);

                                                //Расчитываем прогресс загрузки
                                                CurrentProgress = (int)Math.Round((double)(BS.Position * 100 / FileLength));

                                                //Если прогресс увеличился генерируем событие передав данные текущего прогресса и Id потока в котором обрабатывается файл
                                                if (CurrentProgress > Progress)
                                                {
                                                    Progress = CurrentProgress;

#if DEBUG
                                                    Debug.WriteLine($"Реальный прогресс {filepath} - {Progress}");
#endif

                                                    OnProgressUpdated(new ProgressEventArgs { Progress = Progress, ThreadId = ThreadId });
                                                }
                                            }

                                            break;
                                        }
                                }
                            }

                            //Возвращаем результат обработки файла
                            return TmpRecords;
                            //Возбуждаем исключение если на обработку попадает файл не поддерживаемого типа
                        default:
                            throw new FileTypeException($"{filepath}");
                    }
                }
                //Перехваетываем исключение обработки файла не поддерживаемого типа и передаём его обработку на верхний уровень
                catch (FileTypeException)
                {
                    throw new FileTypeException($"{filepath}");
                }
                //Перехваетываем исключение обработки файла с неверной структурой и передаём его обработку на верхний уровень
                catch (FormatException err)
                {
                    throw new FormatException($" {filepath} - ({err.Message})");
                }
                //Перехваетываем иные исключения и передаём его обработку на верхний уровень
                catch (Exception err)
                {
                    throw new Exception($"File processing error  {filepath} - ({err.Message})");
                }

            }
        }

        //Асинхронный метод загрузки и обработки (описание логики аналогичное синхронному методу)
        public async Task<IList<IRecord>> LoadDataAsync(string filepath)
        {

            IList<IRecord> TmpRecords = new List<IRecord>();

            using (FileStream FS = new FileStream(@filepath, FileMode.Open))
            using (BufferedStream BS = new BufferedStream(FS, 1024))
            using (StreamReader SR = new StreamReader(BS))
            {
                int Progress = 0, 
                    CurrentProgress = 0,
                    ThreadId = Thread.CurrentThread.ManagedThreadId;
                long FileLength = FS.Length;


                switch (filepath.Substring(filepath.Length - 4, 4))
                {
                    case ".txt":
                        try
                        {

                            string line = "";

                            while ((line = await SR.ReadLineAsync()) != null)
                            {

                                string[] recdata = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                                Record rec = new Record
                                {
                                    Date = Convert.ToDateTime(recdata[0]),
                                    Open = Convert.ToDouble(recdata[1], Formatter),
                                    High = Convert.ToDouble(recdata[2], Formatter),
                                    Low = Convert.ToDouble(recdata[3], Formatter),
                                    Close = Convert.ToDouble(recdata[4], Formatter),
                                    Volume = Convert.ToDouble(recdata[5], Formatter)
                                };

                                TmpRecords.Add(rec);

                                CurrentProgress = (int)Math.Round((double)(BS.Position * 100 / FileLength));

                                if (CurrentProgress>Progress)
                                {
                                    Progress = CurrentProgress;
                                    
                                    #if DEBUG
                                        Debug.WriteLine($"Реальный прогресс {filepath} - {Progress}");
                                    #endif

                                    OnProgressUpdated(new ProgressEventArgs { Progress = Progress, ThreadId = ThreadId });
                                }
                            }

                            //OnProgressUpdated(new ProgressEventArgs { Progress = 100 });
                            return TmpRecords;
                        }

                        catch (FormatException err)
                        {
                            throw new FormatException($" {filepath} - ({err.Message})");
                        }
                        catch (Exception err)
                        {
                            throw new Exception($"File processing error  {filepath} - ({err.Message})");
                        }

                    case ".csv":
                        try
                        {

                            string line = "";

                            while ((line = await SR.ReadLineAsync()) != null)
                            {

                                string[] recdata = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                Record rec = new Record
                                {
                                    Date = Convert.ToDateTime(recdata[0]),
                                    Open = Convert.ToDouble(recdata[1], CSVFormatter),
                                    High = Convert.ToDouble(recdata[2], CSVFormatter),
                                    Low = Convert.ToDouble(recdata[3], CSVFormatter),
                                    Close = Convert.ToDouble(recdata[4], CSVFormatter),
                                    Volume = Convert.ToDouble(recdata[5], CSVFormatter)
                                };

                                TmpRecords.Add(rec);


                                CurrentProgress = (int)Math.Round((double)(BS.Position * 100 / FileLength));

                                if (CurrentProgress > Progress)
                                {
                                    Progress = CurrentProgress;

                                    #if DEBUG
                                        Debug.WriteLine($"Реальный прогресс {filepath} - {Progress}");
                                    #endif

                                    OnProgressUpdated(new ProgressEventArgs { Progress = Progress, ThreadId = ThreadId });
                                }
                            }

                            //OnProgressUpdated(new ProgressEventArgs { Progress = 100 });
                            return TmpRecords;
                        }

                        catch (FormatException err)
                        {
                            throw new FormatException($" {filepath} - ({err.Message})");
                        }
                        catch (Exception err)
                        {
                            throw new Exception($"File processing error  {filepath} - ({err.Message})");
                        }

                    case ".xml":
                        try
                        {
                            XmlReader XML = null;
                            XmlReaderSettings Settings = new XmlReaderSettings
                            {
                                Async = true
                            };

                            XML = XmlReader.Create(SR, Settings);

                            while (await XML.ReadAsync())
                            {

                                Record rec = new Record();

                                switch (XML.NodeType)
                                {

                                    case XmlNodeType.Element:
                                        {
                                            if (XML.HasAttributes)
                                            {
                                                while (XML.MoveToNextAttribute())
                                                {
                                                    switch (XML.Name)
                                                    {

                                                        case "date":
                                                            rec.Date = Convert.ToDateTime(XML.Value);
                                                            break;

                                                        case "open":
                                                            rec.Open = Convert.ToDouble(XML.Value, Formatter);
                                                            break;

                                                        case "high":
                                                            rec.High = Convert.ToDouble(XML.Value, Formatter);
                                                            break;

                                                        case "low":
                                                            rec.Low = Convert.ToDouble(XML.Value, Formatter);
                                                            break;

                                                        case "close":
                                                            rec.Close = Convert.ToDouble(XML.Value, Formatter);
                                                            break;

                                                        case "volume":
                                                            rec.Volume = Convert.ToDouble(XML.Value, Formatter);
                                                            break;
                                                        default:
                                                            throw new Exception();

                                                    }
                                                }

                                                TmpRecords.Add(rec);

                                                double temp = FS.Position * 100 / FileLength;
                                                CurrentProgress = (int)Math.Round((double)(BS.Position * 100 / FileLength));

                                                if (CurrentProgress > Progress)
                                                {
                                                    Progress = CurrentProgress;

                                                    #if DEBUG
                                                        Debug.WriteLine($"Реальный прогресс {filepath} - {Progress}");
                                                    #endif

                                                    OnProgressUpdated(new ProgressEventArgs { Progress = Progress, ThreadId = ThreadId});
                                                }
                                            }

                                            break;
                                        }
                                }
                            }

                            //OnProgressUpdated(new ProgressEventArgs { Progress = 100 });
                            return TmpRecords;

                        }
                        catch (FormatException err)
                        {
                            throw new FormatException($" {filepath} - ({err.Message})");
                        }
                        catch (Exception err)
                        {
                            throw new Exception($"File processing error  {filepath} - ({err.Message})");
                        }

                    default:
                        throw new FileTypeException($"{filepath}");
                }
            }
        }

        //Метод гененирующий событие изменения прогресса загрузки файла потокобезопасным способом
        protected virtual void OnProgressUpdated(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> handler = Interlocked.CompareExchange(ref ProgressUpdated, null, null);
            handler?.Invoke(this, e);
        }

        //Делегат события генерируемого при изменении прогресса загрузки файла
        public event EventHandler<ProgressEventArgs> ProgressUpdated;

    }


}
