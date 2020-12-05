using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plugin;
using Contract;
using System.Diagnostics;

namespace PluginTest
{
    [TestClass]
    [DeploymentItem("Files\\Good\\good.xml")]
    [DeploymentItem("Files\\Good\\good.csv")]
    [DeploymentItem("Files\\Good\\good.txt")]
    [DeploymentItem("Files\\Bad\\Data.accdb")]
    [DeploymentItem("Files\\Bad\\bad.csv")]
    [DeploymentItem("Files\\Bad\\bad.txt")]
    [DeploymentItem("Files\\Bad\\bad.xml")]
    public class PluginTest
    {
        private static List<IRecord> records;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            records = new List<IRecord>
            {
                new Record
                {
                    Date = new DateTime(2013,5,20),
                    Open = 30.16,
                    High = 30.39,
                    Low = 30.02,
                    Close = 30.17,
                    Volume = 1478200
                },
                new Record
                {
                    Date=new DateTime(2013,5,17),
                    Open=29.77,
                    High=30.26,
                    Low=29.77,
                    Close=30.26,
                    Volume=2481400
                },
                new Record
                 {
                    Date=new DateTime(2013,5,16),
                    Open=29.78,
                    High=29.94,
                    Low=29.55,
                    Close=29.67,
                    Volume=1077000
                },
                new Record
                {
                    Date=new DateTime(2013,5,15),
                    Open=29.63,
                    High=29.99,
                    Low=29.63,
                    Close=29.98,
                    Volume=928700
                }
            };
           
        }

        [ClassCleanup]
        public static void TestCleanUp()
        {
            File.Delete("good.txt");
            File.Delete("good.csv");
            File.Delete("good.xml");

            File.Delete("bad.txt");
            File.Delete("bad.csv");
            File.Delete("bad.xml");

            File.Delete("Data.accdb");
        }

        [TestMethod]
        public void AllItemsAreNotNull()
        {
            // Проверка, что все элементы коллекции созданы
            CollectionAssert.AllItemsAreNotNull(records, "Not null failed");
        }

        [TestMethod]
        public void AllItemsAreUnique()
        {
            // Проверка значений коллекции на уникальность
            CollectionAssert.AllItemsAreUnique(records, "Uniqueness failed");
        }

        [TestMethod]
        public void LoadXmlFile_GetRecordsList()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();
            List<IRecord> realrecords;

            //act
            realrecords = (List<IRecord>)plug.LoadData("good.xml");


            //assert
            for (int i=0; i<=3; i++)
            {
                Assert.AreEqual(records[i].Date, realrecords[i].Date);
                Assert.AreEqual(records[i].Open, realrecords[i].Open);
                Assert.AreEqual(records[i].Close, realrecords[i].Close);
                Assert.AreEqual(records[i].High, realrecords[i].High);
                Assert.AreEqual(records[i].Low, realrecords[i].Low);
            }
        }

        [TestMethod]
        public async Task LoadXmlFile_GetRecordsListAsync()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();
            List<IRecord> realrecords;

            //act
            realrecords = (List<IRecord>)await plug.LoadDataAsync("good.xml");

            //assert
            for (int i = 0; i <= 3; i++)
            {
                Assert.AreEqual(records[i].Date, realrecords[i].Date);
                Assert.AreEqual(records[i].Open, realrecords[i].Open);
                Assert.AreEqual(records[i].Close, realrecords[i].Close);
                Assert.AreEqual(records[i].High, realrecords[i].High);
                Assert.AreEqual(records[i].Low, realrecords[i].Low);
            }
        }

        [TestMethod]
        public void LoadCSVFile_GetRecordsList()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();
            List<IRecord> realrecords;

            //act
            realrecords = (List<IRecord>)plug.LoadData("good.csv");


            //assert
            for (int i = 0; i <= 3; i++)
            {
                Assert.AreEqual(records[i].Date, realrecords[i].Date);
                Assert.AreEqual(records[i].Open, realrecords[i].Open);
                Assert.AreEqual(records[i].Close, realrecords[i].Close);
                Assert.AreEqual(records[i].High, realrecords[i].High);
                Assert.AreEqual(records[i].Low, realrecords[i].Low);
            }
        }

        [TestMethod]
        public async Task LoadCSVFile_GetRecordsListAsync()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();
            List<IRecord> realrecords;

            //act
            realrecords = (List<IRecord>)await plug.LoadDataAsync("good.csv");


            //assert
            for (int i = 0; i <= 3; i++)
            {
                Assert.AreEqual(records[i].Date, realrecords[i].Date);
                Assert.AreEqual(records[i].Open, realrecords[i].Open);
                Assert.AreEqual(records[i].Close, realrecords[i].Close);
                Assert.AreEqual(records[i].High, realrecords[i].High);
                Assert.AreEqual(records[i].Low, realrecords[i].Low);
            }
        }

        [TestMethod]
        public void LoadTXTFile_GetRecordsList()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();
            List<IRecord> realrecords;

            //act
            realrecords = (List<IRecord>)plug.LoadData("good.txt");


            //assert
            for (int i = 0; i <= 3; i++)
            {
                Assert.AreEqual(records[i].Date, realrecords[i].Date);
                Assert.AreEqual(records[i].Open, realrecords[i].Open);
                Assert.AreEqual(records[i].Close, realrecords[i].Close);
                Assert.AreEqual(records[i].High, realrecords[i].High);
                Assert.AreEqual(records[i].Low, realrecords[i].Low);
            }
        }

        [TestMethod]
        public async Task LoadTXTFile_GetRecordsListAsync()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();
            List<IRecord> realrecords;

            //act
            realrecords = (List<IRecord>)await plug.LoadDataAsync("good.txt");


            //assert
            for (int i = 0; i <= 3; i++)
            {
                Assert.AreEqual(records[i].Date, realrecords[i].Date);
                Assert.AreEqual(records[i].Open, realrecords[i].Open);
                Assert.AreEqual(records[i].Close, realrecords[i].Close);
                Assert.AreEqual(records[i].High, realrecords[i].High);
                Assert.AreEqual(records[i].Low, realrecords[i].Low);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileTypeException))]
        public void LoadMSAccessFile_RiseError()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();

            //act
            plug.LoadData("Data.accdb");
        }

        [TestMethod]
        [ExpectedException(typeof(FileTypeException))]
        public async Task LoadMSAccessFile_RiseErrorAsync()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();

            //act
            await plug.LoadDataAsync("Data.accdb");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void LoadBadCSVFile_RiseError()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();

            //act
            plug.LoadData("bad.csv");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public async Task LoadBadCSVFile_RiseErrorAsync()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();

            //act
            await plug.LoadDataAsync("bad.csv");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void LoadBadTXTFile_RiseError()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();

            //act
            plug.LoadData("bad.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public async Task LoadBadTXTFile_RiseErrorAsync()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();

            //act
            await plug.LoadDataAsync("bad.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void LoadBadXMLFile_RiseError()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();

            //act
            plug.LoadData("bad.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public async Task LoadBadXMLFile_RiseErrorAsync()
        {
            //arrange
            Plugin.Plugin plug = new Plugin.Plugin();

            //act
            await plug.LoadDataAsync("bad.xml");
        }
    }
}

