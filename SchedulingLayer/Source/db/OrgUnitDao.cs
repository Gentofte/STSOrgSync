using Organisation.BusinessLayer.DTO.V1_1;
using Organisation.IntegrationLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;

namespace Organisation.SchedulingLayer
{
    public class OrgUnitDao
    {
        private string connectionString = null;
        private bool useSqlLite = false;

        public OrgUnitDao()
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

                    using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.CREATE_TABLE_SQLITE, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.CREATE_IT_SYSTEMS_TABLE_SQLITE, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.CREATE_CONTACT_PLACES_TABLE_SQLLITE, connection))
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

                    using (SqlCommand command = new SqlCommand(OrgUnitStatements.CREATE_TABLE_MSSQL, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SqlCommand command = new SqlCommand(OrgUnitStatements.CREATE_IT_SYSTEMS_TABLE_MSSQL, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SqlCommand command = new SqlCommand(OrgUnitStatements.CREATE_CONTACT_PLACES_TABLE_MSSQL, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Save(OrgUnitRegistration ou, OperationType operation)
        {
            long orgunit_id = 0;

            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.INSERT_SQLITE, connection))
                            {
                                command.Transaction = transaction;

                                command.Parameters.Add(new SQLiteParameter("@uuid", ou.Uuid));
                                command.Parameters.Add(new SQLiteParameter("@shortkey", ou.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@name", ou.Name ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@parent_ou_uuid", ou.ParentOrgUnitUuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@payout_ou_uuid", ou.PayoutUnitUuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@operation", operation.ToString()));

                                command.Parameters.Add(new SQLiteParameter("@los_shortname_uuid", ou.LOSShortName?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@los_shortname_shortkey", ou.LOSShortName?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@los_shortname_value", ou.LOSShortName?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@phone_uuid", ou.Phone?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@phone_shortkey", ou.Phone?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@phone_value", ou.Phone?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@email_uuid", ou.Email?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@email_shortkey", ou.Email?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@email_value", ou.Email?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@location_uuid", ou.Location?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@location_shortkey", ou.Location?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@location_value", ou.Location?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@ean_uuid", ou.Ean?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@ean_shortkey", ou.Ean?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@ean_value", ou.Ean?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@contact_open_hours_uuid", ou.ContactOpenHours?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@contact_open_hours_shortkey", ou.ContactOpenHours?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@contact_open_hours_value", ou.ContactOpenHours?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@email_remarks_uuid", ou.EmailRemarks?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@email_remarks_shortkey", ou.EmailRemarks?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@email_remarks_value", ou.EmailRemarks?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@contact_uuid", ou.Contact?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@contact_shortkey", ou.Contact?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@contact_value", ou.Contact?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@post_return_uuid", ou.PostReturn?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@post_return_shortkey", ou.PostReturn?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@post_return_value", ou.PostReturn?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@phone_open_hours_uuid", ou.PhoneOpenHours?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@phone_open_hours_shortkey", ou.PhoneOpenHours?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@phone_open_hours_value", ou.PhoneOpenHours?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SQLiteParameter("@post_uuid", ou.Post?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@post_shortkey", ou.Post?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SQLiteParameter("@post_value", ou.Post?.Value ?? (object)DBNull.Value));

                                command.ExecuteNonQuery();

                                orgunit_id = connection.LastInsertRowId;
                            }

                            // insert itsystems
                            foreach (string itSystemUuid in ou.ItSystemUuids ?? Enumerable.Empty<string>())
                            {
                                using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.INSERT_ITSYSTEMS, connection))
                                {
                                    command.Transaction = transaction;

                                    command.Parameters.Add(new SQLiteParameter("@orgunit_id", orgunit_id));
                                    command.Parameters.Add(new SQLiteParameter("@itsystem_uuid", itSystemUuid));

                                    command.ExecuteNonQuery();
                                }
                            }

                            // insert contact places
                            foreach (ContactPlace contactPlace in ou.ContactPlaces ?? Enumerable.Empty<ContactPlace>())
                            {
                                foreach (string task in contactPlace.Tasks ?? Enumerable.Empty<string>())
                                {
                                    using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.INSERT_CONTACT_PLACES, connection))
                                    {
                                        command.Transaction = transaction;

                                        command.Parameters.Add(new SQLiteParameter("@orgunit_id", orgunit_id));
                                        command.Parameters.Add(new SQLiteParameter("@contact_place_uuid", contactPlace.OrgUnitUuid));
                                        command.Parameters.Add(new SQLiteParameter("@task", task));

                                        command.ExecuteNonQuery();
                                    }
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
                            using (SqlCommand command = new SqlCommand(OrgUnitStatements.INSERT_MSSQL, connection))
                            {
                                command.Transaction = transaction;

                                command.Parameters.Add(new SqlParameter("@uuid", ou.Uuid));
                                command.Parameters.Add(new SqlParameter("@shortkey", ou.ShortKey)); // ?? (object) DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@name", ou.Name ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@parent_ou_uuid", ou.ParentOrgUnitUuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@payout_ou_uuid", ou.PayoutUnitUuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@operation", operation.ToString()));

                                command.Parameters.Add(new SqlParameter("@los_shortname_uuid", ou.LOSShortName?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@los_shortname_shortkey", ou.LOSShortName?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@los_shortname_value", ou.LOSShortName?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@phone_uuid", ou.Phone?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@phone_shortkey", ou.Phone?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@phone_value", ou.Phone?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@email_uuid", ou.Email?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@email_shortkey", ou.Email?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@email_value", ou.Email?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@location_uuid", ou.Location?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@location_shortkey", ou.Location?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@location_value", ou.Location?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@ean_uuid", ou.Ean?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@ean_shortkey", ou.Ean?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@ean_value", ou.Ean?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@contact_open_hours_uuid", ou.ContactOpenHours?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@contact_open_hours_shortkey", ou.ContactOpenHours?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@contact_open_hours_value", ou.ContactOpenHours?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@email_remarks_uuid", ou.EmailRemarks?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@email_remarks_shortkey", ou.EmailRemarks?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@email_remarks_value", ou.EmailRemarks?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@contact_uuid", ou.Contact?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@contact_shortkey", ou.Contact?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@contact_value", ou.Contact?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@post_return_uuid", ou.PostReturn?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@post_return_shortkey", ou.PostReturn?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@post_return_value", ou.PostReturn?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@phone_open_hours_uuid", ou.PhoneOpenHours?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@phone_open_hours_shortkey", ou.PhoneOpenHours?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@phone_open_hours_value", ou.PhoneOpenHours?.Value ?? (object)DBNull.Value));

                                command.Parameters.Add(new SqlParameter("@post_uuid", ou.Post?.Uuid ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@post_shortkey", ou.Post?.ShortKey ?? (object)DBNull.Value));
                                command.Parameters.Add(new SqlParameter("@post_value", ou.Post?.Value ?? (object)DBNull.Value));

                                orgunit_id = (long)command.ExecuteScalar();
                            }

                            // insert itsystems
                            foreach (string itSystemUuid in ou.ItSystemUuids ?? Enumerable.Empty<string>())
                            {
                                using (SqlCommand command = new SqlCommand(OrgUnitStatements.INSERT_ITSYSTEMS, connection))
                                {
                                    command.Transaction = transaction;

                                    command.Parameters.Add(new SqlParameter("@orgunit_id", orgunit_id));
                                    command.Parameters.Add(new SqlParameter("@itsystem_uuid", itSystemUuid));

                                    command.ExecuteNonQuery();
                                }
                            }

                            // insert contact places
                            foreach (ContactPlace contactPlace in ou.ContactPlaces ?? Enumerable.Empty<ContactPlace>())
                            {
                                foreach (string task in contactPlace.Tasks ?? Enumerable.Empty<string>())
                                {
                                    using (SqlCommand command = new SqlCommand(OrgUnitStatements.INSERT_CONTACT_PLACES, connection))
                                    {
                                        command.Transaction = transaction;

                                        command.Parameters.Add(new SqlParameter("@orgunit_id", orgunit_id));
                                        command.Parameters.Add(new SqlParameter("@contact_place_uuid", contactPlace.OrgUnitUuid));
                                        command.Parameters.Add(new SqlParameter("@task", task));

                                        command.ExecuteNonQuery();
                                    }
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

        public OrgUnitRegistrationExtended GetOldestEntry()
        {
            OrgUnitRegistrationExtended orgUnit = null;
            long orgUnitId = 0;

            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.SELECT_SQLITE, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                orgUnitId = (long)reader["id"];

                                orgUnit = new OrgUnitRegistrationExtended();
                                orgUnit.Id = orgUnitId;
                                orgUnit.Uuid = GetValue(reader, "uuid");
                                orgUnit.ShortKey = GetValue(reader, "shortkey");
                                orgUnit.Name = GetValue(reader, "name");
                                orgUnit.ParentOrgUnitUuid = GetValue(reader, "parent_ou_uuid");
                                orgUnit.PayoutUnitUuid = GetValue(reader, "payout_ou_uuid");

                                orgUnit.LOSShortName = new Address();
                                orgUnit.LOSShortName.Uuid = GetValue(reader, "los_shortname_uuid");
                                orgUnit.LOSShortName.ShortKey = GetValue(reader, "los_shortname_shortkey");
                                orgUnit.LOSShortName.Value = GetValue(reader, "los_shortname_value");

                                orgUnit.Phone = new Address();
                                orgUnit.Phone.Uuid = GetValue(reader, "phone_uuid");
                                orgUnit.Phone.ShortKey = GetValue(reader, "phone_shortkey");
                                orgUnit.Phone.Value = GetValue(reader, "phone_value");

                                orgUnit.Email = new Address();
                                orgUnit.Email.Uuid = GetValue(reader, "email_uuid");
                                orgUnit.Email.ShortKey = GetValue(reader, "email_shortkey");
                                orgUnit.Email.Value = GetValue(reader, "email_value");

                                orgUnit.Location = new Address();
                                orgUnit.Location.Uuid = GetValue(reader, "location_uuid");
                                orgUnit.Location.ShortKey = GetValue(reader, "location_shortkey");
                                orgUnit.Location.Value = GetValue(reader, "location_value");

                                orgUnit.Ean = new Address();
                                orgUnit.Ean.Uuid = GetValue(reader, "ean_uuid");
                                orgUnit.Ean.ShortKey = GetValue(reader, "ean_shortkey");
                                orgUnit.Ean.Value = GetValue(reader, "ean_value");

                                orgUnit.Post = new Address();
                                orgUnit.Post.Uuid = GetValue(reader, "post_uuid");
                                orgUnit.Post.ShortKey = GetValue(reader, "post_shortkey");
                                orgUnit.Post.Value = GetValue(reader, "post_value");

                                orgUnit.ContactOpenHours = new Address();
                                orgUnit.ContactOpenHours.Uuid = GetValue(reader, "contact_open_hours_uuid");
                                orgUnit.ContactOpenHours.ShortKey = GetValue(reader, "contact_open_hours_shortkey");
                                orgUnit.ContactOpenHours.Value = GetValue(reader, "contact_open_hours_value");

                                orgUnit.EmailRemarks = new Address();
                                orgUnit.EmailRemarks.Uuid = GetValue(reader, "email_remarks_uuid");
                                orgUnit.EmailRemarks.ShortKey = GetValue(reader, "email_remarks_shortkey");
                                orgUnit.EmailRemarks.Value = GetValue(reader, "email_remarks_value");

                                orgUnit.Contact = new Address();
                                orgUnit.Contact.Uuid = GetValue(reader, "contact_uuid");
                                orgUnit.Contact.ShortKey = GetValue(reader, "contact_shortkey");
                                orgUnit.Contact.Value = GetValue(reader, "contact_value");

                                orgUnit.PostReturn = new Address();
                                orgUnit.PostReturn.Uuid = GetValue(reader, "post_return_uuid");
                                orgUnit.PostReturn.ShortKey = GetValue(reader, "post_return_shortkey");
                                orgUnit.PostReturn.Value = GetValue(reader, "post_return_value");

                                orgUnit.PhoneOpenHours = new Address();
                                orgUnit.PhoneOpenHours.Uuid = GetValue(reader, "phone_open_hours_uuid");
                                orgUnit.PhoneOpenHours.ShortKey = GetValue(reader, "phone_open_hours_shortkey");
                                orgUnit.PhoneOpenHours.Value = GetValue(reader, "phone_open_hours_value");

                                orgUnit.Operation = (OperationType)Enum.Parse(typeof(OperationType), GetValue(reader, "operation"));
                            }
                        }
                    }

                    // read itsystems
                    if (orgUnit != null)
                    {
                        orgUnit.ItSystemUuids = new List<string>();

                        using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.SELECT_ITSYSTEMS, connection))
                        {
                            command.Parameters.Add(new SQLiteParameter("@id", orgUnitId));

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    orgUnit.ItSystemUuids.Add(GetValue(reader, "itsystem_uuid"));
                                }
                            }
                        }
                    }

                    // read contact places
                    if (orgUnit != null)
                    {
                        using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.SELECT_CONTACT_PLACES, connection))
                        {
                            command.Parameters.Add(new SQLiteParameter("@id", orgUnitId));

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string contactPlaceUuid = GetValue(reader, "contact_place_uuid");
                                    string task = GetValue(reader, "task");

                                    ContactPlace contactPlace = null;
                                    foreach (ContactPlace cp in orgUnit.ContactPlaces)
                                    {
                                        if (cp.OrgUnitUuid.Equals(contactPlaceUuid))
                                        {
                                            contactPlace = cp;
                                            break;
                                        }
                                    }

                                    if (contactPlace == null)
                                    {
                                        contactPlace = new ContactPlace();
                                        contactPlace.OrgUnitUuid = contactPlaceUuid;
                                        contactPlace.Tasks = new List<string>();

                                        orgUnit.ContactPlaces.Add(contactPlace);
                                    }

                                    contactPlace.Tasks.Add(task);
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

                    using (SqlCommand command = new SqlCommand(OrgUnitStatements.SELECT_MSSQL, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                orgUnitId = (long)reader["id"];

                                orgUnit = new OrgUnitRegistrationExtended();
                                orgUnit.Id = orgUnitId;
                                orgUnit.Uuid = GetValue(reader, "uuid");
                                orgUnit.ShortKey = GetValue(reader, "shortkey");
                                orgUnit.Name = GetValue(reader, "name");
                                orgUnit.ParentOrgUnitUuid = GetValue(reader, "parent_ou_uuid");
                                orgUnit.PayoutUnitUuid = GetValue(reader, "payout_ou_uuid");

                                orgUnit.LOSShortName = new Address();
                                orgUnit.LOSShortName.Uuid = GetValue(reader, "los_shortname_uuid");
                                orgUnit.LOSShortName.ShortKey = GetValue(reader, "los_shortname_shortkey");
                                orgUnit.LOSShortName.Value = GetValue(reader, "los_shortname_value");

                                orgUnit.Phone = new Address();
                                orgUnit.Phone.Uuid = GetValue(reader, "phone_uuid");
                                orgUnit.Phone.ShortKey = GetValue(reader, "phone_shortkey");
                                orgUnit.Phone.Value = GetValue(reader, "phone_value");

                                orgUnit.Email = new Address();
                                orgUnit.Email.Uuid = GetValue(reader, "email_uuid");
                                orgUnit.Email.ShortKey = GetValue(reader, "email_shortkey");
                                orgUnit.Email.Value = GetValue(reader, "email_value");

                                orgUnit.Location = new Address();
                                orgUnit.Location.Uuid = GetValue(reader, "location_uuid");
                                orgUnit.Location.ShortKey = GetValue(reader, "location_shortkey");
                                orgUnit.Location.Value = GetValue(reader, "location_value");

                                orgUnit.Ean = new Address();
                                orgUnit.Ean.Uuid = GetValue(reader, "ean_uuid");
                                orgUnit.Ean.ShortKey = GetValue(reader, "ean_shortkey");
                                orgUnit.Ean.Value = GetValue(reader, "ean_value");

                                orgUnit.Post = new Address();
                                orgUnit.Post.Uuid = GetValue(reader, "post_uuid");
                                orgUnit.Post.ShortKey = GetValue(reader, "post_shortkey");
                                orgUnit.Post.Value = GetValue(reader, "post_value");

                                orgUnit.ContactOpenHours = new Address();
                                orgUnit.ContactOpenHours.Uuid = GetValue(reader, "contact_open_hours_uuid");
                                orgUnit.ContactOpenHours.ShortKey = GetValue(reader, "contact_open_hours_shortkey");
                                orgUnit.ContactOpenHours.Value = GetValue(reader, "contact_open_hours_value");

                                orgUnit.EmailRemarks = new Address();
                                orgUnit.EmailRemarks.Uuid = GetValue(reader, "email_remarks_uuid");
                                orgUnit.EmailRemarks.ShortKey = GetValue(reader, "email_remarks_shortkey");
                                orgUnit.EmailRemarks.Value = GetValue(reader, "email_remarks_value");

                                orgUnit.Contact = new Address();
                                orgUnit.Contact.Uuid = GetValue(reader, "contact_uuid");
                                orgUnit.Contact.ShortKey = GetValue(reader, "contact_shortkey");
                                orgUnit.Contact.Value = GetValue(reader, "contact_value");

                                orgUnit.PostReturn = new Address();
                                orgUnit.PostReturn.Uuid = GetValue(reader, "post_return_uuid");
                                orgUnit.PostReturn.ShortKey = GetValue(reader, "post_return_shortkey");
                                orgUnit.PostReturn.Value = GetValue(reader, "post_return_value");

                                orgUnit.PhoneOpenHours = new Address();
                                orgUnit.PhoneOpenHours.Uuid = GetValue(reader, "phone_open_hours_uuid");
                                orgUnit.PhoneOpenHours.ShortKey = GetValue(reader, "phone_open_hours_shortkey");
                                orgUnit.PhoneOpenHours.Value = GetValue(reader, "phone_open_hours_value");

                                orgUnit.Operation = (OperationType)Enum.Parse(typeof(OperationType), GetValue(reader, "operation"));
                            }
                        }
                    }

                    // read itsystems
                    if (orgUnit != null)
                    {
                        orgUnit.ItSystemUuids = new List<string>();
                        using (SqlCommand command = new SqlCommand(OrgUnitStatements.SELECT_ITSYSTEMS, connection))
                        {
                            command.Parameters.Add(new SqlParameter("@id", orgUnitId));

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    orgUnit.ItSystemUuids.Add(GetValue(reader, "itsystem_uuid"));
                                }
                            }
                        }
                    }

                    // read contact places
                    if (orgUnit != null)
                    {
                        using (SqlCommand command = new SqlCommand(OrgUnitStatements.SELECT_CONTACT_PLACES, connection))
                        {
                            command.Parameters.Add(new SqlParameter("@id", orgUnitId));

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string contactPlaceUuid = GetValue(reader, "contact_place_uuid");
                                    string task = GetValue(reader, "task");

                                    ContactPlace contactPlace = null;
                                    foreach (ContactPlace cp in orgUnit.ContactPlaces)
                                    {
                                        if (cp.OrgUnitUuid.Equals(contactPlaceUuid))
                                        {
                                            contactPlace = cp;
                                            break;
                                        }
                                    }

                                    if (contactPlace == null)
                                    {
                                        contactPlace = new ContactPlace();
                                        contactPlace.Tasks = new List<string>();
                                        contactPlace.OrgUnitUuid = contactPlaceUuid;
                                        orgUnit.ContactPlaces.Add(contactPlace);
                                    }

                                    contactPlace.Tasks.Add(task);
                                }
                            }
                        }
                    }
                }
            }

            return orgUnit;
        }

        public void Delete(long id)
        {
            if (useSqlLite)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(OrgUnitStatements.DELETE, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@id", id));
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(OrgUnitStatements.DELETE, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@id", id));
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
