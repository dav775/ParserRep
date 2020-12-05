using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parser
{
    //Класс реализующий интерфейс IFileListener
    public class FileListener : IFileListener
    {
        //метод считывающий информацию о файлах в рабочем каталоге
        public FileInfo[] ReadFilesAtt(string wd)
        {
            try
            {
                DirectoryInfo DI = new DirectoryInfo(wd);
                return DI.GetFiles("*");
            }
            catch (Exception err)
            {
                throw new Exception ("Get files info error (" + err.Message + ")");
            }
        }

        //метод считывающий информацию о файле в рабочем каталоге
        public FileInfo ReadFileAtt(string fname)
        {
            try
            {
                FileInfo FInfo = new FileInfo(fname);
                return FInfo;
            }
            catch (Exception err)
            {
                throw new Exception("Get file info error (" + err.Message + ")");
            }
        }
    }
}
