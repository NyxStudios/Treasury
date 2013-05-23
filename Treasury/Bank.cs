using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace Treasury
{
    class Bank
    {
        /// <summary>
        /// Bank Name
        /// </summary>
        public string BankName { get; set; }
        
        /// <summary>
        /// Connection to the database
        /// </summary>
        private IDbConnection _db;

        public Bank(String name)
        {
            BankName = name;
            InitializeDB();
        }

        /// <summary>
        /// Returns the name of the bank with spaces replaced with underscores.
        /// </summary>
        /// <returns></returns>
        private string SanitizeName()
        {
            return BankName.Replace(" ", "_");
        }


        /// <summary>
        /// Initializes this banks database for access later
        /// </summary>
        private void InitializeDB()
        {
            if (Treasury.Config.StorageType.ToLower() == "sqlite")
            {
                string sql = Path.Combine(TShock.SavePath, "treasury.sqlite");
                _db = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
            }
            else if (Treasury.Config.StorageType.ToLower() == "mysql")
            {
                try
                {
                    var hostport = Treasury.Config.MySqlHost.Split(':');
                    _db = new MySqlConnection();
                    _db.ConnectionString =
                        String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                                      hostport[0],
                                      hostport.Length > 1 ? hostport[1] : "3306",
                                      "treasury",
                                      Treasury.Config.MySqlUsername,
                                      Treasury.Config.MySqlPassword
                            );
                }
                catch (MySqlException ex)
                {
                    Log.Error(ex.ToString());
                    throw new Exception("MySql not setup correctly");
                }
            }
            else
            {
                throw new Exception("Invalid storage type");
            }

            var table = new SqlTable(SanitizeName(),
                                     new SqlColumn("AccountName", MySqlDbType.VarChar, 56) { Length = 56, Primary = true },
                                     new SqlColumn("Amount", MySqlDbType.Int32) { DefaultValue = "0" }
                );
            var creator = new SqlTableCreator(_db,
                                              _db.GetSqlType() == SqlType.Sqlite
                                                ? (IQueryBuilder)new SqliteQueryCreator()
                                                : new MysqlQueryCreator());
            creator.EnsureExists(table);
        }

        public void UpdateAccount(string name, int new_amount)
        {
            string query = "UPDATE @0 SET Amount=@1 WHERE AccountName=@2";

            if (!DoesAccountExist(name))
            {
                query = "INSERT INTO @0 (AccountName, Amount) VALUES (@1, @2);";
            }

            if (_db.Query(query, SanitizeName(), name, new_amount) != 1)
            {
                throw new InvalidOperationException(String.Format("Failed to update user account: {0}", name));
            }
        }

        public bool DoesAccountExist(string name)
        {
            try
            {
                using (var reader = _db.QueryReader("SELECT * FROM @0 WHERE AccountName=@1", SanitizeName(), name))
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("There was an error checking for player: {0}", name));
            }
            return false;
        }

        public int GetAccountAmount(string name)
        {
            string query = "Select Amount from @0 WHERE AccountName=@1";
            using(var reader = _db.QueryReader(query, SanitizeName(), name))
            {
                if(reader.Read())
                {
                    return reader.Get<Int32>("Amount");
                }
            }

            throw new InvalidOperationException(String.Format("Could not retrieve account info for: {0}", name));
        }
    }
}
