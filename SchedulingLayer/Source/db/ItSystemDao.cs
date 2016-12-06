using Organisation.BusinessLayer;
using Organisation.IntegrationLayer;
using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.SqlTypes;

namespace Organisation.SchedulingLayer
{
    public class ItSystemDao
    {
        private string connectionString = null;
        private bool useSqlLite = false;

        public ItSystemDao()
        {
            connectionString = OrganisationRegistryProperties.GetInstance().DBConnectionString;
            if (connectionString.Equals("SQLITE"))
            {
                useSqlLite = true;
                connectionString = "Data Source=./STSOrgSync.sqlite;Version=3;Pooling=True;MaxPoolSize=10;foreign keys=true;";
            }

            CreateTableIfNotExists();
        }

        public void CreateTableIfNotExists()
        {
            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(ItSystemStatements.CREATE_TABLE_SQLITE, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(ItSystemStatements.CREATE_TABLE_MSSQL, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Save(ItSystemRegistration itSystem, OperationType operation)
        {
            long itsystem_id;

            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(ItSystemStatements.INSERT, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@uuid", itSystem?.Uuid));
                        command.Parameters.Add(new SQLiteParameter("@shortkey", itSystem?.SystemShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@jump_url", itSystem?.JumpUrl ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@operation", operation.ToString()));

                        itsystem_id = (long)command.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(ItSystemStatements.INSERT, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@uuid", itSystem?.Uuid));
                        command.Parameters.Add(new SqlParameter("@shortkey", itSystem?.SystemShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@jump_url", itSystem?.JumpUrl ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@operation", operation.ToString()));

                        itsystem_id = (long)command.ExecuteScalar();
                    }
                }
            }
        }

        // TODO: it-systems are never read - the Scheduler should be updated to fix this
        public ItSystemRegistrationExtended GetOldestEntry()
        {
            ItSystemRegistrationExtended itSystem = null;
            long itSystemId = 0;

            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(ItSystemStatements.SELECT_SQLITE, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                itSystem = new ItSystemRegistrationExtended();
                                itSystem.Uuid = GetValue(reader, "uuid");
                                itSystem.SystemShortKey = GetValue(reader, "shortkey");
                                itSystem.JumpUrl = GetValue(reader, "jump_url");
                                itSystem.Operation = (OperationType)Enum.Parse(typeof(OperationType), GetValue(reader, "operation"));
                                itSystemId = (long)reader["id"];
                            }
                        }
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(ItSystemStatements.SELECT_MSSQL, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                itSystem = new ItSystemRegistrationExtended();
                                itSystem.Uuid = GetValue(reader, "uuid");
                                itSystem.SystemShortKey = GetValue(reader, "shortkey");
                                itSystem.JumpUrl = GetValue(reader, "jump_url");
                                itSystem.Operation = (OperationType)Enum.Parse(typeof(OperationType), GetValue(reader, "operation"));
                                itSystemId = (long)reader["id"];
                            }
                        }
                    }
                }
            }

            return itSystem;
        }

        public void Delete(string uuid)
        {
            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(ItSystemStatements.DELETE, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@uuid", uuid));
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(ItSystemStatements.DELETE, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@uuid", uuid));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private string GetValue(SqlDataReader reader, string key)
        {
            if (reader[key] is DBNull)
            {
                return null;
            }

            return (string)reader[key];
        }

        private string GetValue(SQLiteDataReader reader, string key)
        {
            if (reader.IsDBNull(reader.GetOrdinal(key)))
            {
                return null;
            }

            return (string)reader[key];
        }
    }
}
