using Xunit;

namespace Dapper.Tests.Contrib
{
    public class PascalToSnakeCasingTests
    {
        [Fact]
        public void ConvertToSnakeCase()
        {
            Assert.Equal("full_name", ColumnMapping.PascalCaseToSnakeCase("FullName"));
            Assert.Equal("the_id", ColumnMapping.PascalCaseToSnakeCase("TheId"));
            Assert.Equal("i_d", ColumnMapping.PascalCaseToSnakeCase("ID"));
        }
    }
}
