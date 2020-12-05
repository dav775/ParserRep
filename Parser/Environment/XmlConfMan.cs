using System;

using System.Xml.Linq;
using System.IO;

namespace Parser
{
    //Класс управления конфигурацией приложения хранимой в формате XML
    public class XmlConfMan : IConfigManager
    {
        //Конфигурация по умолчанию
        private IConfig DefConfig()
        {
            IConfig Config = new Config();
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
                XDocument XML = new XDocument(
                                new XElement("configuration",
                                               new XElement("WorkDir", config.WorkDir),
                                               new XElement("ProcessingType", config.ProcessingType.ToString()),
                                               new XElement("Interval",
                                                                new XAttribute("hh", config.Interval.Hours.ToString()),
                                                                new XAttribute("mm", config.Interval.Minutes.ToString()),
                                                                new XAttribute("ss", config.Interval.Seconds.ToString())
                                                           )
                                             )
                              );

                XML.Save(file);
            }
            //Перехватываем ошибки и передаём их обработку на верхний уровень
            catch (Exception err)
            {
                throw new Exception("Creating configuration file error (" + err.Message + ")"); 
            }

        }

        //Констркутор
        public XmlConfMan()
        { }

        //Чтение конфигурационной информации
        public IConfig ReadConfig(IConfig config, string file)
        {
            if (file == "Config.") file += "xml";

            try
            {
                //
                XDocument xml = XDocument.Load(file);

                if ((xml != null) && (xml.Root != null))
                {
                    config.ProcessingType = bool.Parse(xml.Element("configuration").Element("ProcessingType").Value);
                    config.WorkDir = @"" + xml.Element("configuration").Element("WorkDir").Value;
                    config.Interval = new TimeSpan(int.Parse(xml.Element("configuration").Element("Interval").Attribute("hh").Value),
                                                    int.Parse(xml.Element("configuration").Element("Interval").Attribute("mm").Value),
                                                    int.Parse(xml.Element("configuration").Element("Interval").Attribute("ss").Value));
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
            if (file == "Config.") file += "xml";

            try
            {
                XDocument XML = XDocument.Load(file);

                XML.Element("configuration").Element("ProcessingType").Value = Convert.ToString(config.ProcessingType);
                XML.Element("configuration").Element("WorkDir").Value = config.WorkDir;
                XML.Element("configuration").Element("Interval").Attribute("hh").Value = Convert.ToString(config.Interval.Hours);
                XML.Element("configuration").Element("Interval").Attribute("mm").Value = Convert.ToString(config.Interval.Minutes);
                XML.Element("configuration").Element("Interval").Attribute("ss").Value = Convert.ToString(config.Interval.Seconds);
                XML.Save(file);
            }
            //Перехватываем ошибки и передаём их обработку на верхний уровень
            catch (Exception err)
            {
                throw new Exception($"Saveing configuration file error (" + err.Message + ")");
            }

        }
    }
}
