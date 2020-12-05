using System.IO;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Parser
{
    //Класс реализующий импорт библитек-плагинов
    public class Import
    {
        //Метод выполняющий инпорт библиотек-плагинов в приложение
        public static void DoImport(object obj)
        {
            //Объект-агрегированный каталог комбинируемый из множества каталогов
            AggregateCatalog catalog = new AggregateCatalog();

            //Указываем каталог из которого будут импортироваться сборки 
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

            //Добавляем каталог в агрегированный каталог
            catalog.Catalogs.Add(new DirectoryCatalog(path));

            //Создаем композиционный контейнер 
            CompositionContainer container = new CompositionContainer(catalog);
            
            //Выполняем композицию объекта obj и классов импортируемых из указанного каталога
            container.ComposeParts(obj);
        }
    }
}
