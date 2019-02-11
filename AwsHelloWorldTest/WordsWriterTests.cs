using AwsHelloWorldMail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwsHelloWorldTest
{
    [TestClass]
    public class WordsWriterTests
    {
        [TestMethod]
        public void ShouldSendEmail()
        {
            WordsWriter.SayHello();
        }
    }
}
