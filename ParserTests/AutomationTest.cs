using System;
using Parser;
using Contract;
using Moq;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ParserTests
{
    [TestClass]
    [DeploymentItem("Files\\Plugin\\Plugin.dll", "Plugins")]
    public class AutomationTest
    {
        Automation _automation;

        FileInfo _fileInfo;
        FileInfo[] _fileInfos;

        Mock<IFileListener> _mockFl;
        Mock<IConfig> _mockConfig;
        ObservableCollection<IRecord> _records;
        ObservableCollection<IProcFile> _procFiles;

        [TestInitialize]
        public void Initialize()
        {
           
            _mockConfig = new Mock<IConfig>();
            _mockFl = new Mock<IFileListener>();
            _records = new ObservableCollection<IRecord>();
            _procFiles = new ObservableCollection<IProcFile>();



            _fileInfo = new FileInfo(@"C:\Dir\XMLFile.xml");

            _fileInfos = new[] { _fileInfo };

            _mockConfig.Setup(x => x.WorkDir).Returns(@"C:\Dir");
            _mockConfig.Setup(x => x.ProcessingType).Returns(false);
            _mockConfig.Setup(x => x.Interval).Returns(new TimeSpan(0, 0, 30));

            _automation = new Automation(_mockFl.Object,
                                         _mockConfig.Object,
                                         _records,
                                         _procFiles);
        }


        [TestMethod]
        public void Automation_ReadeFilesList_Test()
        {
            _mockFl.Setup(m => m.ReadFilesAtt(It.IsAny<string>())).Returns(_fileInfos);

            _automation.Start();

            _mockFl.VerifyAll();

        }
    }
}
