using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalisysDirectory.Tests
{
    [TestClass]
    class DirectoryParserTest
    {
        private readonly DirectoryParser _directoryParser;

        public DirectoryParserTest()
        {
            _directoryParser = new DirectoryParser("E:\\c#\\AnalisysDirectory\\Tests\\testDirectory");
        }

        [TestMethod]
        public void getExtension_ReturnTrue()
        {
           string result = _directoryParser.getExtension("PATH pdf");

            Assert.IsTrue(result==".pdf", "OK");
            
        }
    }
}
