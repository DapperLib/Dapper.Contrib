namespace Dapper.Contrib.Extensions
{
    public class LowerCaseColumnNameFormatter : IColumnNameFormatter
    {
        public string Format(string name)
        {
            return name.ToLower();
        }
    }

}
