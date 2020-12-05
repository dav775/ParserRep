using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    //Интерфейс типа реализующего работу с конфигурацией приложения
    public interface IConfigManager
    {
        //Декларирует наличие метода чтения конфигурации
        IConfig ReadConfig(IConfig obj, string file=@"Config.");
        //Декларирует наличие метода сохранения конфигурации
        void SaveConfig(IConfig obj, string file=@"Config.");
    }
}
