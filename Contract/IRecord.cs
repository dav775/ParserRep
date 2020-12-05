using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    //Определяем интерфейса типа обрабатываемх данных
    public interface IRecord
    {
        //Декларирует наличие даты группы значений
        DateTime Date { get; set; }
        //Декларирует наличие значения при открытии сессии
        double Open { get; set; }
        //Декларирует наличие значения максимальное значение сессии
        double High { get; set; }
        //Декларирует наличие значения минимальное значение сессии
        double Low { get; set; }
        //Декларирует наличие значения мначение при закрытии сессии
        double Close { get; set; }
        //Декларирует наличие значения объема
        double Volume { get; set; }
    }
}
