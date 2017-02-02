using Organisation.BusinessLayer.DTO.V1_1;
using Organisation.IntegrationLayer;
using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;

namespace Organisation.SchedulingLayer
{
    public class UserDao
    {
        private string connectionString = null;
        private bool useSqlLite = false;

        // TODO: duplicate code for SQLite - can this by dynamified somehow... dynamics does not seem to be useful, as the constructors takes arguments... maybe something else
        // this TODO goes for all SQL related code (all 3 DAO classes)

        public UserDao()
        {
            connectionString = OrganisationRegistryProperties.GetInstance().DBConnectionString;
            if (connectionString.Equals("SQLITE"))
            {
                useSqlLite = true;
                connectionString = "Data Source=./STSOrgSync.sqlite;Version=3;Pooling=True;MaxPoolSize=10;foreign keys=true;";
            }

            CreateTableIfNotExists();
        }

        private void CreateTableIfNotExists()
        {
            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(UserStatements.CREATE_TABLE_SQLITE, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SQLiteCommand command = new SQLiteCommand(UserStatements.CREATE_CHILD_TABLE_SQLITE, connection))
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

                    using (SqlCommand command = new SqlCommand(UserStatements.CREATE_TABLE_MSSQL, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SqlCommand command = new SqlCommand(UserStatements.CREATE_CHILD_TABLE_MSSQL, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Save(UserRegistration user, OperationType operation)
        {
            long user_id = 0;

            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand(UserStatements.INSERT_SQLITE, connection))
                            {
                                command.Transaction = transaction;

                                command.Parameters.Add(new SQLiteParameter("@user_uuid", user.Uuid));
                                command.Parameters.Add(new SQLiteParameter("@user_shortkey", user.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_id", user.UserId ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_phone_uuid", user.Phone?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_phone_shortkey", user.Phone?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_phone_value", user.Phone?.Value ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@person_shortkey", user.Person.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@person_uuid", user.Person.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@person_name", user.Person.Name ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@person_cpr", user.Person.Cpr ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_email_uuid", user.Email?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_email_shortkey", user.Email?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_email_value", user.Email?.Value ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_location_shortkey", user.Location?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_location_value", user.Location?.Value ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@user_location_uuid", user.Location?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@operation", operation.ToString()));
                                command.ExecuteNonQuery();

                                user_id = connection.LastInsertRowId;
                            }

                            // insert positions
                            foreach (Position position in user.Positions ?? Enumerable.Empty<Position>())
                            {
                                using (SQLiteCommand command = new SQLiteCommand(UserStatements.INSERT_POSITION, connection))
                                {
                                    command.Transaction = transaction;

                                    command.Parameters.Add(new SQLiteParameter("@user_id", user_id));
                                    command.Parameters.Add(new SQLiteParameter("@uuid", position.Uuid ?? (object)DBNull.Value));
                                    command.Parameters.Add(new SQLiteParameter("@shortkey", position.ShortKey ?? (object)DBNull.Value));
                                    command.Parameters.Add(new SQLiteParameter("@name", position.Name));
                                    command.Parameters.Add(new SQLiteParameter("@orgunit_uuid", position.OrgUnitUuid));
                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();

                            throw;
                        }
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand(UserStatements.INSERT_MSSQL, connection))
                            {
                                command.Transaction = transaction;

                                command.Parameters.Add(new SqlParameter("@user_uuid", user.Uuid));
                                command.Parameters.Add(new SqlParameter("@user_shortkey", user.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_id", user.UserId ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_phone_uuid", user.Phone?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_phone_shortkey", user.Phone?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_phone_value", user.Phone?.Value ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@person_shortkey", user.Person.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@person_uuid", user.Person.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@person_name", user.Person.Name ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@person_cpr", user.Person.Cpr ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_email_uuid", user.Email?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_email_shortkey", user.Email?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_email_value", user.Email?.Value ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_location_shortkey", user.Location?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_location_value", user.Location?.Value ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@user_location_uuid", user.Location?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@operation", operation.ToString()));

                                user_id = (long)command.ExecuteScalar();
                            }

                            // insert positions
                            foreach (Position position in user.Positions ?? Enumerable.Empty<Position>())
                            {
                                using (SqlCommand command = new SqlCommand(UserStatements.INSERT_POSITION, connection))
                                {
                                    command.Transaction = transaction;

                                    command.Parameters.Add(new SqlParameter("@user_id", user_id));
                                    command.Parameters.Add(new SqlParameter("@uuid", position.Uuid ?? (object)DBNull.Value));
                                    command.Parameters.Add(new SqlParameter("@shortkey", position.ShortKey ?? (object)DBNull.Value));
                                    command.Parameters.Add(new SqlParameter("@name", position.Name));
                                    command.Parameters.Add(new SqlParameter("@orgunit_uuid", position.OrgUnitUuid));
                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();

                            throw;
                        }
                    }
                }   
            }
        }

        public UserRegistrationExtended GetOldestEntry()
        {
            UserRegistrationExtended user = null;
            long user_id = 0;

            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(UserStatements.SELECT_SQLITE, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new UserRegistrationExtended();
                                user_id = (long)reader["id"];

                                if (GetValue(reader, "user_phone_value") != null)
                                {
                                    user.Phone = new Address();
                                    user.Phone.Uuid = GetValue(reader, "user_phone_uuid");
                                    user.Phone.ShortKey = GetValue(reader, "user_phone_shortkey");
                                    user.Phone.Value = GetValue(reader, "user_phone_value");
                                }

                                if (GetValue(reader, "user_email_value") != null)
                                {
                                    user.Email = new Address();
                                    user.Email.Uuid = GetValue(reader, "user_email_uuid");
                                    user.Email.ShortKey = GetValue(reader, "user_email_shortkey");
                                    user.Email.Value = GetValue(reader, "user_email_value");
                                }

                                if (GetValue(reader, "user_location_value") != null)
                                {
                                    user.Location = new Address();
                                    user.Location.ShortKey = GetValue(reader, "user_location_shortkey");
                                    user.Location.Value = GetValue(reader, "user_location_value");
                                    user.Location.Uuid = GetValue(reader, "user_location_uuid");
                                }

                                user.UserId = GetValue(reader, "user_id");
                                user.Uuid = GetValue(reader, "user_uuid");
                                user.ShortKey = GetValue(reader, "user_shortkey");

                                user.Person.ShortKey = GetValue(reader, "person_shortkey");
                                user.Person.Uuid = GetValue(reader, "person_uuid");
                                user.Person.Name = GetValue(reader, "person_name");
                                user.Person.Cpr = GetValue(reader, "person_cpr");

                                // legacy read (TODO: remove this in some future version, when we remove these fields from the schema)
                                Position position = new Position();
                                position.Uuid = GetValue(reader, "position_uuid");
                                position.ShortKey = GetValue(reader, "position_shortkey");
                                position.OrgUnitUuid = GetValue(reader, "position_orgunit_uuid");
                                position.Name = GetValue(reader, "position_name");

                                if (!string.IsNullOrEmpty(position.OrgUnitUuid) && !string.IsNullOrEmpty(position.Name))
                                {
                                    user.Positions.Add(position);
                                }

                                user.Timestamp = (DateTime)reader["timestamp"];
                                user.Operation = (OperationType)Enum.Parse(typeof(OperationType), GetValue(reader, "operation"));
                            }
                        }
                    }

                    // read positions
                    if (user != null)
                    {
                        using (SQLiteCommand command = new SQLiteCommand(UserStatements.SELECT_POSITIONS, connection))
                        {
                            command.Parameters.Add(new SQLiteParameter("@id", user_id));

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Position position = new Position();
                                    position.Uuid = GetValue(reader, "uuid");
                                    position.ShortKey = GetValue(reader, "shortkey");
                                    position.OrgUnitUuid = GetValue(reader, "orgunit_uuid");
                                    position.Name = GetValue(reader, "name");

                                    user.Positions.Add(position);
                                }
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

                    using (SqlCommand command = new SqlCommand(UserStatements.SELECT_MSSQL, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new UserRegistrationExtended();
                                user_id = (long)reader["id"];

                                if (GetValue(reader, "user_phone_value") != null)
                                {
                                    user.Phone = new Address();
                                    user.Phone.Uuid = GetValue(reader, "user_phone_uuid");
                                    user.Phone.ShortKey = GetValue(reader, "user_phone_shortkey");
                                    user.Phone.Value = GetValue(reader, "user_phone_value");
                                }

                                if (GetValue(reader, "user_email_value") != null)
                                {
                                    user.Email = new Address();
                                    user.Email.Uuid = GetValue(reader, "user_email_uuid");
                                    user.Email.ShortKey = GetValue(reader, "user_email_shortkey");
                                    user.Email.Value = GetValue(reader, "user_email_value");
                                }

                                if (GetValue(reader, "user_location_value") != null)
                                {
                                    user.Location = new Address();
                                    user.Location.ShortKey = GetValue(reader, "user_location_shortkey");
                                    user.Location.Value = GetValue(reader, "user_location_value");
                                    user.Location.Uuid = GetValue(reader, "user_location_uuid");
                                }

                                user.UserId = GetValue(reader, "user_id");
                                user.Uuid = GetValue(reader, "user_uuid");
                                user.ShortKey = GetValue(reader, "user_shortkey");

                                user.Person.ShortKey = GetValue(reader, "person_shortkey");
                                user.Person.Uuid = GetValue(reader, "person_uuid");
                                user.Person.Name = GetValue(reader, "person_name");
                                user.Person.Cpr = GetValue(reader, "person_cpr");

                                // legacy read (TODO: remove this in some future version, when we remove these fields from the schema)
                                Position position = new Position();
                                position.Uuid = GetValue(reader, "position_uuid");
                                position.ShortKey = GetValue(reader, "position_shortkey");
                                position.OrgUnitUuid = GetValue(reader, "position_orgunit_uuid");
                                position.Name = GetValue(reader, "position_name");

                                if (!string.IsNullOrEmpty(position.OrgUnitUuid) && !string.IsNullOrEmpty(position.Name))
                                {
                                    user.Positions.Add(position);
                                }

                                user.Timestamp = (DateTime)reader["timestamp"];
                                user.Operation = (OperationType)Enum.Parse(typeof(OperationType), GetValue(reader, "operation"));

                                return user;
                            }
                        }
                    }

                    // read positions
                    if (user != null)
                    {
                        using (SqlCommand command = new SqlCommand(UserStatements.SELECT_POSITIONS, connection))
                        {
                            command.Parameters.Add(new SqlParameter("@id", user_id));

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Position position = new Position();
                                    position.Uuid = GetValue(reader, "uuid");
                                    position.ShortKey = GetValue(reader, "shortkey");
                                    position.OrgUnitUuid = GetValue(reader, "orgunit_uuid");
                                    position.Name = GetValue(reader, "name");

                                    user.Positions.Add(position);
                                }
                            }
                        }
                    }
                }
            }

            return user;
        }

        public void Delete(string uuid)
        {
            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(UserStatements.DELETE, connection))
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

                    using (SqlCommand command = new SqlCommand(UserStatements.DELETE, connection))
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
