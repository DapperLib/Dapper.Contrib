using Microsoft.Data.Sqlite;
using MySqlConnector;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Xunit;
using Xunit.Sdk;

namespace Dapper.Tests.Contrib
{
    // The test suites here implement TestSuiteBase so that each provider runs
    // the entire set of tests without declarations per method
    // If we want to support a new provider, they need only be added here - not in multiple places

    [XunitTestCaseDiscoverer("Dapper.Tests.SkippableFactDiscoverer", "Dapper.Tests.Contrib")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SkippableFactAttribute : FactAttribute
    {
    }


    /// <summary>
    /// Disaplce parallel running of test suites. Because of the dependency on 
    /// the static property <see cref="DefaultTypeMap.MatchNamesWithUnderscores"/>
    /// parallel running wil break tests using different settings.
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/1408175/execute-unit-tests-serially-rather-than-in-parallel
    /// https://github.com/xunit/visualstudio.xunit/issues/191
    /// </remarks>
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class NonParallelCollectionDefinitionClass
    {
    }

    [Collection("Sequential")]
    public class SqlServerTestSuite : TestSuite
    {
        private const string DbName = "tempdb";
        public static string ConnectionString =>
            GetConnectionString("SqlServerConnectionString", $"Data Source=.;Initial Catalog={DbName};Integrated Security=True");

        public override IDbConnection GetConnection() => new SqlConnection(ConnectionString);

        static SqlServerTestSuite()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = false;

            using (var connection = new SqlConnection(ConnectionString))
            {
                // ReSharper disable once AccessToDisposedClosure
                void dropTable(string name) => connection.Execute($"IF OBJECT_ID('{name}', 'U') IS NOT NULL DROP TABLE [{name}]; ");
                connection.Open();
                dropTable("Stuff");
                connection.Execute("CREATE TABLE Stuff (TheId int IDENTITY(1,1) not null, Name nvarchar(100) not null, CreatedAt DateTime null);");
                dropTable("People");
                connection.Execute("CREATE TABLE People (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null);");
                dropTable("Users");
                connection.Execute("CREATE TABLE Users (Id int IDENTITY(1,1) not null, UserName nvarchar(100) not null, Age int not null);");
                dropTable("Automobiles");
                connection.Execute("CREATE TABLE Automobiles (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null);");
                dropTable("Results");
                connection.Execute("CREATE TABLE Results (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, [Order] int not null);");
                dropTable("ObjectX");
                connection.Execute("CREATE TABLE ObjectX (ObjectXId nvarchar(100) not null, Name nvarchar(100) not null);");
                dropTable("ObjectY");
                connection.Execute("CREATE TABLE ObjectY (ObjectYId int not null, Name nvarchar(100) not null);");
                dropTable("ObjectZ");
                connection.Execute("CREATE TABLE ObjectZ (Id int not null, Name nvarchar(100) not null);");
                dropTable("GenericType");
                connection.Execute("CREATE TABLE GenericType (Id nvarchar(100) not null, Name nvarchar(100) not null);");
                dropTable("NullableDates");
                connection.Execute("CREATE TABLE NullableDates (Id int IDENTITY(1,1) not null, DateValue DateTime null);");
            }
        }
    }

    [Collection("Sequential")]
    public class SqlServerUnderscoreTestSuite : TestSuite
    {
        private const string DbName = "tempdb";
        public static string ConnectionString =>
            GetConnectionString("SqlServerConnectionString", $"Data Source=.;Initial Catalog={DbName};Integrated Security=True");

        public override IDbConnection GetConnection() => new SqlConnection(ConnectionString);

        static SqlServerUnderscoreTestSuite()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            using (var connection = new SqlConnection(ConnectionString))
            {
                // ReSharper disable once AccessToDisposedClosure
                void dropTable(string name) => connection.Execute($"IF OBJECT_ID('{name}', 'U') IS NOT NULL DROP TABLE [{name}]; ");
                connection.Open();
                dropTable("Stuff");
                connection.Execute("CREATE TABLE Stuff (the_id int IDENTITY(1,1) not null, name nvarchar(100) not null, created_at DateTime null);");
                dropTable("People");
                connection.Execute("CREATE TABLE People (id int IDENTITY(1,1) not null, name nvarchar(100) not null);");
                dropTable("Users");
                connection.Execute("CREATE TABLE Users (id int IDENTITY(1,1) not null, user_name nvarchar(100) not null, age int not null);");
                dropTable("Automobiles");
                connection.Execute("CREATE TABLE Automobiles (id int IDENTITY(1,1) not null, name nvarchar(100) not null);");
                dropTable("Results");
                connection.Execute("CREATE TABLE Results (id int IDENTITY(1,1) not null, name nvarchar(100) not null, [order] int not null);");
                dropTable("ObjectX");
                connection.Execute("CREATE TABLE ObjectX (object_x_Id nvarchar(100) not null, name nvarchar(100) not null);");
                dropTable("ObjectY");
                connection.Execute("CREATE TABLE ObjectY (object_y_Id int not null, name nvarchar(100) not null);");
                dropTable("ObjectZ");
                connection.Execute("CREATE TABLE ObjectZ (id int not null, name nvarchar(100) not null);");
                dropTable("GenericType");
                connection.Execute("CREATE TABLE GenericType (id nvarchar(100) not null, name nvarchar(100) not null);");
                dropTable("NullableDates");
                connection.Execute("CREATE TABLE NullableDates (id int IDENTITY(1,1) not null, date_value DateTime null);");
            }
        }
    }

    [Collection("Sequential")]
    public class MySqlServerTestSuite : TestSuite
    {
        public static string ConnectionString { get; } =
            GetConnectionString("MySqlConnectionString", "Server=localhost;Database=tests;Uid=test;Pwd=pass;UseAffectedRows=false;");

        public override IDbConnection GetConnection()
        {
            if (_skip) Skip.Inconclusive("Skipping MySQL Tests - no server.");
            return new MySqlConnection(ConnectionString);
        }

        private static readonly bool _skip;

        static MySqlServerTestSuite()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = false;

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    // ReSharper disable once AccessToDisposedClosure
                    void dropTable(string name) => connection.Execute($"DROP TABLE IF EXISTS `{name}`;");
                    connection.Open();
                    dropTable("Stuff");
                    connection.Execute("CREATE TABLE Stuff (TheId int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null, Created DateTime null);");
                    dropTable("People");
                    connection.Execute("CREATE TABLE People (Id int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null);");
                    dropTable("Users");
                    connection.Execute("CREATE TABLE Users (Id int not null AUTO_INCREMENT PRIMARY KEY, UserName nvarchar(100) not null, Age int not null);");
                    dropTable("Automobiles");
                    connection.Execute("CREATE TABLE Automobiles (Id int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null);");
                    dropTable("Results");
                    connection.Execute("CREATE TABLE Results (Id int not null AUTO_INCREMENT PRIMARY KEY, Name nvarchar(100) not null, `Order` int not null);");
                    dropTable("ObjectX");
                    connection.Execute("CREATE TABLE ObjectX (ObjectXId nvarchar(100) not null, Name nvarchar(100) not null);");
                    dropTable("ObjectY");
                    connection.Execute("CREATE TABLE ObjectY (ObjectYId int not null, Name nvarchar(100) not null);");
                    dropTable("ObjectZ");
                    connection.Execute("CREATE TABLE ObjectZ (Id int not null, Name nvarchar(100) not null);");
                    dropTable("GenericType");
                    connection.Execute("CREATE TABLE GenericType (Id nvarchar(100) not null, Name nvarchar(100) not null);");
                    dropTable("NullableDates");
                    connection.Execute("CREATE TABLE NullableDates (Id int not null AUTO_INCREMENT PRIMARY KEY, DateValue DateTime);");
                }
            }
            catch (MySqlException e)
            {
                if (e.Message.Contains("Unable to connect"))
                    _skip = true;
                else
                    throw;
            }
        }
    }

    [Collection("Sequential")]
    public class SQLiteUnderscoreTestSuite : TestSuite
    {
        // Use a different file name to the SQLiteTestSuite to avoid the test runner attempting to interact with the same file.
        private const string FileName = "Test.DB.Undescore.sqlite";
        public static string ConnectionString => $"Filename=./{FileName};Mode=ReadWriteCreate;";
        public override IDbConnection GetConnection() => new SqliteConnection(ConnectionString);

        static SQLiteUnderscoreTestSuite()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute("CREATE TABLE Stuff (the_id integer primary key autoincrement not null, Name nvarchar(100) not null, created_at DateTime null) ");
                connection.Execute("CREATE TABLE People (id integer primary key autoincrement not null, name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE Users (id integer primary key autoincrement not null, user_name nvarchar(100) not null, age int not null) ");
                connection.Execute("CREATE TABLE Automobiles (id integer primary key autoincrement not null, name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE Results (id integer primary key autoincrement not null, name nvarchar(100) not null, [order] int not null) ");
                connection.Execute("CREATE TABLE ObjectX (object_x_id nvarchar(100) not null, name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE ObjectY (object_y_id integer not null, name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE ObjectZ (id integer not null, name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE GenericType (id nvarchar(100) not null, name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE NullableDates (id integer primary key autoincrement not null, date_value DateTime) ");
            }
        }
    }

    [Collection("Sequential")]
    public class SQLiteTestSuite : TestSuite
    {
        private const string FileName = "Test.DB.sqlite";
        public static string ConnectionString => $"Filename=./{FileName};Mode=ReadWriteCreate;";
        public override IDbConnection GetConnection() => new SqliteConnection(ConnectionString);

        static SQLiteTestSuite()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = false;

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute("CREATE TABLE Stuff (TheId integer primary key autoincrement not null, Name nvarchar(100) not null, CreatedAt DateTime null) ");
                connection.Execute("CREATE TABLE People (Id integer primary key autoincrement not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE Users (Id integer primary key autoincrement not null, UserName nvarchar(100) not null, Age int not null) ");
                connection.Execute("CREATE TABLE Automobiles (Id integer primary key autoincrement not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE Results (Id integer primary key autoincrement not null, Name nvarchar(100) not null, [Order] int not null) ");
                connection.Execute("CREATE TABLE ObjectX (ObjectXId nvarchar(100) not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE ObjectY (ObjectYId integer not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE ObjectZ (Id integer not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE GenericType (Id nvarchar(100) not null, Name nvarchar(100) not null) ");
                connection.Execute("CREATE TABLE NullableDates (Id integer primary key autoincrement not null, DateValue DateTime) ");
            }
        }
    }


#if SQLCE
   [Collection("Sequential")]
   public class SqlCETestSuite : TestSuite
    {
        const string FileName = "Test.DB.sdf";
        public static string ConnectionString => $"Data Source={FileName};";
        public override IDbConnection GetConnection() => new SqlCeConnection(ConnectionString);
            
        static SqlCETestSuite()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            var engine = new SqlCeEngine(ConnectionString);
            engine.CreateDatabase();
            using (var connection = new SqlCeConnection(ConnectionString))
            {
                connection.Open();
                connection.Execute(@"CREATE TABLE Stuff (TheId int IDENTITY(1,1) not null, Name nvarchar(100) not null, CreatedAt DateTime null) ");
                connection.Execute(@"CREATE TABLE People (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE Users (Id int IDENTITY(1,1) not null, User_Name nvarchar(100) not null, Age int not null) ");
                connection.Execute(@"CREATE TABLE Automobiles (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE Results (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, [Order] int not null) ");
                connection.Execute(@"CREATE TABLE ObjectX (ObjectXId nvarchar(100) not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE ObjectY (ObjectYId int not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE ObjectZ (Id int not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE GenericType (Id nvarchar(100) not null, Name nvarchar(100) not null) ");
                connection.Execute(@"CREATE TABLE NullableDates (Id int IDENTITY(1,1) not null, DateValue DateTime null) ");
            }
            Console.WriteLine("Created database");
        }
    }
#endif
}
