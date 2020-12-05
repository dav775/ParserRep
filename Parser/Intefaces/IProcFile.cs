using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    //Интерфейс типа реализующего хранения сведений об обрабатываемом файле
    public interface IProcFile
    {
        //Декларирует наличие свойства для хранения имени файла
        string FileName { get; set; }
        //Декларирует наличие свойства
        int Completion { get; set; }
        //Декларирует наличие свойства
        string Message { get; set; }
    }
}
