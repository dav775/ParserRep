using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Contract;

namespace Parser
{
    //Интерфейс типа реализующего логику обработки данных 
    public interface IAutomation
    {
        //Декларирует наличие метода запуска процесса обработки данных
        void Start();
        //Декларирует наличие метода остановки процесса обработки данных
        void Stop();
    }
}
