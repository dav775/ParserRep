using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Parser
{
    //Класс управления конфигурацией приложения хранимой в бинарном формате
    public class BinConfMan : IConfigManager
    {
        //Конфигурация по умолчанию
        private IConfig DefConfig()
        {
            IConfig Config = new ConfigBin();
            Uri workDir = new Uri(@"C:\Dir\", UriKind.Relative);
            Config.WorkDir = workDir.ToString();
            Config.Interval = new TimeSpan(0, 0, 30);
            Config.ProcessingType = false;

            return Config;
        }

        //Создание файла конфигурации
        private void CreateConfigFile(IConfig config, string file)
        {
            try
            {
                using (File.Create(file)) { }
                SaveConfig(config, file);
            }
            //Перехватываем ошибки и передаём их обработку на верхний уровень
            catch (Exception e)
            {
                throw new Exception("Creating configuration file error (" + e.Message + ")");
            }
            
        }

        //Конструктор без параметров
        public BinConfMan()
        { }

        //Чтение конфигурационной информации
        public IConfig ReadConfig(IConfig config, string file)
        {
            BinaryFormatter Formatter = new BinaryFormatter();

            if (file == "Config.") file += "bin";

            try
            {
                using (FileStream FS = new FileStream(file, FileMode.Open))
                {
                    config = (IConfig)Formatter.Deserialize(FS);
                    config.Interval = (TimeSpan)Formatter.Deserialize(FS);
                }

                if ((config.WorkDir == null) || (config.Interval == null))
                {
                    config = DefConfig();
                    SaveConfig(config, file);
                }

                return config;
            }
            //Перехватываем ошибку при отсутствии файла конфигурации
            catch (FileNotFoundException)
            {
                //Создаем конфигурацию по умолчанию и возвращаем её в приложение
                config = DefConfig();
                CreateConfigFile(config, file);

                return config;
            }

            //Перехватываем остальные ошибки и передаём их обработку на верхний уровень
            catch (Exception err)
            {
                throw new Exception("Reading configuration file error (" + err.Message + ")");
            }
        }

        //Сохранение конфигурационной информации
        public void SaveConfig(IConfig config, string file)
        {
            BinaryFormatter Formatter = new BinaryFormatter();

            if (file == "Config.") file += "bin";

            try
            {
                using (FileStream FS = new FileStream(file, FileMode.OpenOrCreate))
                {
                    Formatter.Serialize(FS, config);
                    Formatter.Serialize(FS, config.Interval);

                }

            }
            //Перехватываем ошибки и передаём их обработку на верхний уровень
            catch (Exception err)
            {
                throw new Exception("Saveing configuration file error (" + err.Message + ")");
            }


        }
    }
}
