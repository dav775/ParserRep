using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser;

namespace ParserTests
{
    /// <summary>
    /// Сводное описание для ReadBrouckenConfig_GetError
    /// </summary>
    [TestClass]
    public class BinConfigTest
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
            File.Delete("BadConfig.bin");
        }


        [TestMethod]
        [DeploymentItem("Files\\Config\\BadConfig.bin")]
        [ExpectedException(typeof(Exception))]
        public void LoadBokenConfig_Bin()
        {
            //arrange
            IConfigManager ConfMan = new BinConfMan();

            //act
            ConfMan.ReadConfig(StartConf, @"BadConfig.bin");
        }

        [TestMethod]
        public void DefCreateSaveReadConfig_Bin()
        {
            //arrange
            IConfigManager ConfMan = new BinConfMan();

            //act
            IConfig ActConfig = ConfMan.ReadConfig(StartConf);

            //assert
            Assert.AreEqual(ExpConfig.WorkDir, ActConfig.WorkDir, "Рабочий каталог не соответсвует");
            Assert.AreEqual(ExpConfig.Interval, ActConfig.Interval, "Интервал не соответсвует");
            Assert.AreEqual(ExpConfig.ProcessingType, ActConfig.ProcessingType, "Тип обработки не соответсвует");
        }

    }
}
