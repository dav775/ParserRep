using System;
using System.IO;
using Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ParserTests
{
    [TestClass]
    public class XmlConfigTest
    {

        private static IConfig StartConf, ExpConfig;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Uri workDir = new Uri(@"C:\Dir\", UriKind.Relative);

            StartConf = new Config
            {
                WorkDir = "",
                Interval = new TimeSpan(0, 0, 0),
                ProcessingType = true
            };

            ExpConfig = new Config
            {
                WorkDir = workDir.ToString(),
                Interval = new TimeSpan(0, 0, 30),
                ProcessingType = false
            };
        }

        [ClassCleanup]
        public static void TestCleanUp()
        {
            File.Delete("BadConfig.xml");
        }

        [TestMethod]
        [DeploymentItem("Files\\Config\\BadConfig.xml")]
        [ExpectedException(typeof(Exception))]
        public void LoadBokenConfig_Xml()
        {
            //arrange
            IConfigManager ConfMan = new XmlConfMan();

            //act
            ConfMan.ReadConfig(StartConf, @"BadConfig.xml");
        }

        [TestMethod]
        public void DefCreateSaveReadConfig_Xml()
        {
            //arrange
            IConfigManager ConfMan = new XmlConfMan();

            //act
            IConfig ActConfig = ConfMan.ReadConfig(StartConf);

            //assert
            Assert.AreEqual(ExpConfig.WorkDir, ActConfig.WorkDir, "Рабочий каталог не соответсвует");
            Assert.AreEqual(ExpConfig.Interval, ActConfig.Interval, "Интервал не соответсвует");
            Assert.AreEqual(ExpConfig.ProcessingType, ActConfig.ProcessingType, "Тип обработки не соответсвует");
        }

    }
}
