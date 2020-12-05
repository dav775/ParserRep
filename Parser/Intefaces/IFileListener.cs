using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Parser
{
    //Интерфейс типа реализующего работу с данными файлов на обработку
    public interface IFileListener
    {
        //Декларирует наличие метода чтения сведений о группе файлов
        FileInfo[] ReadFilesAtt(string wd);
        //Декларирует наличие метода чтения сведений о файле
        FileInfo ReadFileAtt(string wd);
    }
}
