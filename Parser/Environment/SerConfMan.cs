using System;
using System.IO;
using System.Xml.Serialization;

namespace Parser
{

    public class SerConfMan : IConfigManager
    {
        
        //Конструктор без параметров
        public SerConfMan()
        { }

        public IConfig ReadConfig(IConfig config)
        {
            try
            {
                using (FileStream fs = new FileStream(@"ParserConfig.xml", FileMode.OpenOrCreate))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(Config));
                    config = (IConfig)formatter.Deserialize(fs);

                    //if (config.WorkDir == null)
                    //{
                    //    Uri workDir = new Uri("/", UriKind.Relative);
                    //    config.WorkDir = workDir.ToString();
                    //    config.Interval = new TimeSpan(0, 0, 30);
                    //    config.ProcessingType = false;

                    //}

                    return config;
                }
            }

            catch (FileNotFoundException e)
            {

                using (File.Create(@"ParserConfig.xml")) { }
                Uri workDir = new Uri(@"C:\Dir\", UriKind.Relative);
                config.WorkDir = workDir.ToString();
                config.Interval = new TimeSpan(0, 0, 30);
                config.ProcessingType = false;
                SaveConfig(config);

                return config;
            }

            catch (InvalidOperationException e)
            {
                if (config.WorkDir == null)
                {
                    Uri workDir = new Uri(@"C:\Dir\", UriKind.Relative);
                    config.WorkDir = workDir.ToString();
                    config.Interval = new TimeSpan(0, 0, 30);
                    config.ProcessingType = false;
                    SaveConfig(config);
                }

                return config;

                //throw new Exception("Deserialize error: (" + e.Message + ")");
            }

            catch (Exception e)
            {
                throw new Exception("Read settings error: (" + e.Message + ")");
            }
        }

        //Сохранение конфигурационной информации
        public void SaveConfig(IConfig config)
        {
            try
            {
                using (FileStream fs = new FileStream(@"ParserConfig.xml", FileMode.OpenOrCreate))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(Config));
                    formatter.Serialize(fs, config);
                }

            }
            catch (Exception err)
            {
                throw new Exception("Save settings error (" + err.Message + ")");
            }


        }
    }
}
