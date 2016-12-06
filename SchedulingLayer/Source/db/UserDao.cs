using Organisation.BusinessLayer;
using Organisation.IntegrationLayer;
using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.SQLite;

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
                }
            }
        }

        public void Save(UserRegistration user, OperationType operation)
        {
            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(UserStatements.INSERT, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@user_uuid", user.UserUuid));
                        command.Parameters.Add(new SQLiteParameter("@user_shortkey", user.UserShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_id", user.UserId ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_phone_uuid", user.Phone?.Uuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_phone_shortkey", user.Phone?.ShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_phone_value", user.Phone?.Value ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@person_shortkey", user.PersonShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@person_uuid", user.PersonUuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@person_name", user.PersonName ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@person_cpr", user.PersonCpr ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_email_uuid", user.Email?.Uuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_email_shortkey", user.Email?.ShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_email_value", user.Email?.Value ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_location_shortkey", user.Location?.ShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_location_value", user.Location?.Value ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@user_location_uuid", user.Location?.Uuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@position_uuid", user.PositionUuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@position_shortkey", user.PositionShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@position_orgunit_uuid", user.PositionOrgUnitUuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@position_name", user.PositionName ?? (object) DBNull.Value));
                        command.Parameters.Add(new SQLiteParameter("@operation", operation.ToString()));

                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(UserStatements.INSERT, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@user_uuid", user.UserUuid));
                        command.Parameters.Add(new SqlParameter("@user_shortkey", user.UserShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_id", user.UserId ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_phone_uuid", user.Phone?.Uuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_phone_shortkey", user.Phone?.ShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_phone_value", user.Phone?.Value ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@person_shortkey", user.PersonShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@person_uuid", user.PersonUuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@person_name", user.PersonName ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@person_cpr", user.PersonCpr ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_email_uuid", user.Email?.Uuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_email_shortkey", user.Email?.ShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_email_value", user.Email?.Value ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_location_shortkey", user.Location?.ShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_location_value", user.Location?.Value ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@user_location_uuid", user.Location?.Uuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@position_uuid", user.PositionUuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@position_shortkey", user.PositionShortKey ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@position_orgunit_uuid", user.PositionOrgUnitUuid ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@position_name", user.PositionName ?? (object) DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@operation", operation.ToString()));

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public UserRegistrationExtended GetOldestEntry()
        {
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
                                UserRegistrationExtended user = new UserRegistrationExtended();

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
                                user.UserUuid = GetValue(reader, "user_uuid");
                                user.UserShortKey = GetValue(reader, "user_shortkey");

                                user.PersonShortKey = GetValue(reader, "person_shortkey");
                                user.PersonUuid = GetValue(reader, "person_uuid");
                                user.PersonName = GetValue(reader, "person_name");
                                user.PersonCpr = GetValue(reader, "person_cpr");

                                user.PositionUuid = GetValue(reader, "position_uuid");
                                user.PositionShortKey = GetValue(reader, "position_shortkey");
                                user.PositionOrgUnitUuid = GetValue(reader, "position_orgunit_uuid");
                                user.PositionName = GetValue(reader, "position_name");

                                user.Timestamp = (DateTime)reader["timestamp"];
                                user.Operation = (OperationType)Enum.Parse(typeof(OperationType), GetValue(reader, "operation"));

                                return user;
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
                                UserRegistrationExtended user = new UserRegistrationExtended();

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
                                user.UserUuid = GetValue(reader, "user_uuid");
                                user.UserShortKey = GetValue(reader, "user_shortkey");

                                user.PersonShortKey = GetValue(reader, "person_shortkey");
                                user.PersonUuid = GetValue(reader, "person_uuid");
                                user.PersonName = GetValue(reader, "person_name");
                                user.PersonCpr = GetValue(reader, "person_cpr");

                                user.PositionUuid = GetValue(reader, "position_uuid");
                                user.PositionShortKey = GetValue(reader, "position_shortkey");
                                user.PositionOrgUnitUuid = GetValue(reader, "position_orgunit_uuid");
                                user.PositionName = GetValue(reader, "position_name");

                                user.Timestamp = (DateTime)reader["timestamp"];
                                user.Operation = (OperationType)Enum.Parse(typeof(OperationType), GetValue(reader, "operation"));

                                return user;
                            }
                        }
                    }
                }
            }

            return null;
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
