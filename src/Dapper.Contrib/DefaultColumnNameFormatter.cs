namespace Dapper.Contrib.Extensions
{
    public class DefaultColumnNameFormatter : IColumnNameFormatter
    {

        public ColumnNameFormat ColumnNameFormat { get; set; }
        public DefaultColumnNameFormatter()
        {
        }

        public DefaultColumnNameFormatter(ColumnNameFormat columnNameFormat)
        {
            ColumnNameFormat = columnNameFormat;
        }

        public string Format(string name)
        {
            switch(ColumnNameFormat)
            {
                case ColumnNameFormat.LowerCase:
                    return name.ToLower();
                    break;
                default:
                    return name;
            }
        }
    }

}
