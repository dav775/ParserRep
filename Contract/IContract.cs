using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contract
{
    //Интерфейс контракта реализуемого плагином
    public interface IContract
    {
        //Декларирует наличие метода осуществляющего синхронную загрузку данных
        IList<IRecord> LoadData(string FileName);
        //Декларирует наличие метода осуществляющего асинхронную загрузку данных
        Task<IList<IRecord>> LoadDataAsync(string FileName);
        //Декларирует наличие члена-события
        event EventHandler<ProgressEventArgs> ProgressUpdated;
    }

    //Описание типа данных передаваемых получателям уведомления о событии
    public class ProgressEventArgs : EventArgs
    {
        //Процент загрузки файла
        public int Progress { get; set; }
        //Id потока в котором идет загрузка файла
        public int ThreadId { get; set; }
    }

    //Определение типа исключения возбуждаемого в случае поступлении на обработку файла не поддерживаемого типа
    public class FileTypeException: Exception
    {
        //Создаем тип наследник Exception
        public FileTypeException(string message): base(message)
        { }
    }
}
