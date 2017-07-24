using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using pcrpg.src.Database.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;

namespace pcrpg.Database.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(string connectionString) : base(connectionString)
        {
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DefaultDbContext, MigrationConfiguration>());
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Characters> Characters { get; set; }
        public DbSet<CharacterTraits> CharacterTraits { get; set; }
        public DbSet<CharacterClothes> CharacterClothes { get; set; }
    }

    public class ContextFactory : IDbContextFactory<DefaultDbContext>
    {
        private static string ConnectionString;

        public static void SetConnectionParameters(string serverAddress, string username, string password, string database, uint port = 3306)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder()
            {
                Server = serverAddress,
                UserID = username,
                Password = password,
                Database = database,
                Port = port
            };

            ConnectionString = connectionStringBuilder.ToString();
        }

        private static DefaultDbContext _instance;

        public static DefaultDbContext Instance
        {
            get
            {
                if (_instance != null) return _instance;
                return _instance = new ContextFactory().Create();
            }
            private set { }
        }

        public DefaultDbContext Create()
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new InvalidOperationException("Please set the connection parameters before trying to instantiate a database connection.");

            return new DefaultDbContext(ConnectionString);
        }
    }

    internal sealed class MigrationConfiguration : DbMigrationsConfiguration<DefaultDbContext>
    {
        public MigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
        }
    }
}
