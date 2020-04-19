using System.IO;
using Xunit;

namespace Lenimal.SingleToBulkTransformer.Test
{
    public class ProgramTest
    {
        [Fact]
        public void Test()
        {
            var input = "./Tests/test.sql";
            var output = "test";
            var fileName = "result";
            var splitCount = 10;

            Program.Run(input, output, fileName, null, splitCount);

            var expectedResult1 = File.ReadAllText("Tests/result_1.sql");
            var expectedResult2 = File.ReadAllText("Tests/result_2.sql");

            var result1 = File.ReadAllText("test/result_1.sql");
            var result2 = File.ReadAllText("test/result_2.sql");

            Assert.Equal(expectedResult1, result1);
            Assert.Equal(expectedResult2, result2);
        }
    }
}