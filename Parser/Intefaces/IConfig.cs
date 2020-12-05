using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    //Интерфейс типа реализующего хранение конфигурации
    public interface IConfig
    {
        //Декларирует наличие свойства типа обработки
        bool ProcessingType { get; set; }
        //Декларирует наличие свойства рабочего каталога
        string WorkDir { get; set; }
        //Декларирует наличие свойства интервала обработки
        TimeSpan Interval { get; set; }
    }
}
