using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;

namespace Plugin
{
    //Класс наследуемый от интерфейса библиотеки контракта описывает структуру данных, получаемх в ходе обработки файлов 
    public class Record : IRecord
    {
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }
}
