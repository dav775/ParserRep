using System;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser;
using System.Collections.Generic;
using System.IO;

namespace ParserTests
{
    [TestClass]
    [DeploymentItem("Files\\Plugin\\Plugin.dll", "Plugins")]
    public class AppViewModelTest
    {
        AppViewModel _appViewModel;

        Mock<IConfig> _mockConfig;
        Mock<IConfigManager> _mockconfigManager;
        Mock<IFileListener> _mockfileListener;

        [TestInitialize]
        public void Initialize()
        {
            _mockConfig = new Mock<IConfig>();
            _mockconfigManager = new Mock<IConfigManager>();
            _mockfileListener = new Mock<IFileListener>();

            _mockConfig.Setup(x => x.WorkDir).Returns(@"C:\Dir");
            _mockConfig.Setup(x => x.ProcessingType).Returns(false);
            _mockConfig.Setup(x => x.Interval).Returns(new TimeSpan(0, 0, 30));

            _mockconfigManager
               .Setup(m => m.ReadConfig(It.IsAny<IConfig>(), It.IsAny<string>())).Returns(_mockConfig.Object);

            _appViewModel = new AppViewModel(_mockConfig.Object,
                                 _mockconfigManager.Object,
                                 _mockfileListener.Object);
        }

        [TestMethod]
        public void AppViewModel_ReadeConfig_Test()
        {
            _mockconfigManager.Verify(m => m.ReadConfig(It.IsAny<IConfig>(), It.IsAny<string>()));

        }
    }
}
