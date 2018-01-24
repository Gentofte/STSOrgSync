namespace Organisation.SchedulingLayer
{
    public class OrgUnitStatements
    {
        public const string CREATE_TABLE_MSSQL = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='orgunits' AND xtype='U') CREATE TABLE orgunits (
	            id				            BIGINT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	            timestamp	        		DATETIME2 NOT NULL DEFAULT GETDATE(),
	            uuid	        			NVARCHAR(36) NOT NULL,
	            shortkey	        		NVARCHAR(50),
	            name		        		NVARCHAR(64),
	            parent_ou_uuid		    	NVARCHAR(36),
	            payout_ou_uuid		    	NVARCHAR(36),
	            los_shortname_uuid	    	NVARCHAR(36),
	            los_shortname_shortkey		NVARCHAR(50),
	            los_shortname_value	    	NVARCHAR(64),
	            phone_uuid		        	NVARCHAR(36),
	            phone_shortkey		    	NVARCHAR(50),
	            phone_value		        	NVARCHAR(64),
	            email_uuid	        		NVARCHAR(36),
	            email_shortkey	    		NVARCHAR(50),
	            email_value		        	NVARCHAR(64),
	            location_uuid	    		NVARCHAR(36),
	            location_shortkey	    	NVARCHAR(50),
	            location_value	    		NVARCHAR(64),
	            ean_uuid	        		NVARCHAR(36),
	            ean_shortkey		    	NVARCHAR(50),
	            ean_value	        		NVARCHAR(64),
	            post_uuid		        	NVARCHAR(36),
	            post_shortkey	    		NVARCHAR(50),
	            post_value		        	NVARCHAR(64),
	            contact_open_hours_uuid		NVARCHAR(36),
	            contact_open_hours_shortkey	NVARCHAR(50),
	            contact_open_hours_value	NVARCHAR(256),
	            email_remarks_uuid	    	NVARCHAR(36),
	            email_remarks_shortkey  	NVARCHAR(50),
	            email_remarks_value	    	NVARCHAR(256),
	            contact_uuid	        	NVARCHAR(36),
	            contact_shortkey        	NVARCHAR(50),
	            contact_value	        	NVARCHAR(256),
	            post_return_uuid		    NVARCHAR(36),
	            post_return_shortkey    	NVARCHAR(50),
	            post_return_value	    	NVARCHAR(256),
	            phone_open_hours_uuid		NVARCHAR(36),
	            phone_open_hours_shortkey	NVARCHAR(50),
	            phone_open_hours_value		NVARCHAR(256),
	            operation		        	NVARCHAR(16) NOT NULL,

	            CONSTRAINT ou_operation_check CHECK (
		            operation IN('UPDATE', 'DELETE')
	            ),
	            CONSTRAINT ou_delete_check CHECK (
		            operation = 'DELETE' OR (
			            name IS NOT NULL
		            )
	            )
            );";

        public static string CREATE_IT_SYSTEMS_TABLE_MSSQL = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='orgunits_itsystems' AND xtype='U') CREATE TABLE orgunits_itsystems (
	            id                  BIGINT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	            unit_id             BIGINT NOT NULL FOREIGN KEY REFERENCES orgunits(id) ON DELETE CASCADE,
	            itsystem_uuid		NVARCHAR(36) NOT NULL
            );";

        public static string CREATE_CONTACT_PLACES_TABLE_MSSQL = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='orgunits_contact_places' AND xtype='U') CREATE TABLE orgunits_contact_places (
	            id                  BIGINT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	            unit_id             BIGINT NOT NULL FOREIGN KEY REFERENCES orgunits(id) ON DELETE CASCADE,
	            contact_place_uuid	NVARCHAR(36) NOT NULL,
	            task            	NVARCHAR(36) NOT NULL
            );";

        public static string CREATE_TABLE_SQLITE = @"CREATE TABLE IF NOT EXISTS orgunits (
	            id				            INTEGER NOT NULL PRIMARY KEY,
	            timestamp                   DATETIME DEFAULT CURRENT_TIMESTAMP,
	            uuid                        NVARCHAR(36) NOT NULL,
	            shortkey                    NVARCHAR(50),
	            name                        NVARCHAR(64),
	            parent_ou_uuid	    		NVARCHAR(36),
	            payout_ou_uuid	    		NVARCHAR(36),
	            los_shortname_uuid		    NVARCHAR(36),
	            los_shortname_shortkey		NVARCHAR(50),
	            los_shortname_value		    NVARCHAR(64),
	            phone_uuid			        NVARCHAR(36),
	            phone_shortkey			    NVARCHAR(50),
	            phone_value			        NVARCHAR(64),
	            email_uuid			        NVARCHAR(36),
	            email_shortkey			    NVARCHAR(50),
	            email_value			        NVARCHAR(64),
	            location_uuid			    NVARCHAR(36),
	            location_shortkey		    NVARCHAR(50),
	            location_value			    NVARCHAR(64),
	            ean_uuid			        NVARCHAR(36),
	            ean_shortkey			    NVARCHAR(50),
	            ean_value			        NVARCHAR(64),
	            post_uuid			        NVARCHAR(36),
	            post_shortkey			    NVARCHAR(50),
	            post_value			        NVARCHAR(64),
	            contact_open_hours_uuid		NVARCHAR(36),
	            contact_open_hours_shortkey	NVARCHAR(50),
	            contact_open_hours_value	NVARCHAR(256),
	            email_remarks_uuid		    NVARCHAR(36),
	            email_remarks_shortkey	    NVARCHAR(50),
	            email_remarks_value		    NVARCHAR(256),
	            contact_uuid		        NVARCHAR(36),
	            contact_shortkey	        NVARCHAR(50),
	            contact_value		        NVARCHAR(256),
	            post_return_uuid		    NVARCHAR(36),
	            post_return_shortkey	    NVARCHAR(50),
	            post_return_value		    NVARCHAR(256),
	            phone_open_hours_uuid		NVARCHAR(36),
	            phone_open_hours_shortkey	NVARCHAR(50),
	            phone_open_hours_value		NVARCHAR(256),
	            operation			        NVARCHAR(16) NOT NULL,

	            CONSTRAINT ou_operation_check CHECK (
		            operation IN('UPDATE', 'DELETE')
	            ),
	            CONSTRAINT ou_delete_check CHECK (
		            operation = 'DELETE' OR (
			            name IS NOT NULL
		            )
	            )
            );";

        public static string CREATE_IT_SYSTEMS_TABLE_SQLITE = @"CREATE TABLE IF NOT EXISTS orgunits_itsystems (
	            id			        INTEGER NOT NULL PRIMARY KEY,
	            unit_id			    INTEGER NOT NULL,
	            itsystem_uuid		NVARCHAR(36) NOT NULL,

                FOREIGN KEY(unit_id) REFERENCES orgunits(id) ON DELETE CASCADE
            );";

        public static string CREATE_CONTACT_PLACES_TABLE_SQLLITE = @"CREATE TABLE IF NOT EXISTS orgunits_contact_places (
	            id			        INTEGER NOT NULL PRIMARY KEY,
	            unit_id			    INTEGER NOT NULL,
	            contact_place_uuid	NVARCHAR(36) NOT NULL,
	            task            	NVARCHAR(36) NOT NULL,

                FOREIGN KEY(unit_id) REFERENCES orgunits(id) ON DELETE CASCADE
            );";

        public const string INSERT_MSSQL = INSERT_OUTPUT + " output INSERTED.ID " + INSERT_VALUES;
        public const string INSERT_SQLITE = INSERT_OUTPUT + " " + INSERT_VALUES;

        private const string INSERT_OUTPUT = @"
                insert into orgunits (
                    uuid,
                    shortkey,
                    name,
                    parent_ou_uuid,
                    payout_ou_uuid,
                    los_shortname_uuid, los_shortname_shortkey, los_shortname_value,
                    phone_uuid, phone_shortkey, phone_value,
                    email_uuid, email_shortkey, email_value,
                    location_uuid, location_shortkey, location_value,
                    ean_uuid, ean_shortkey, ean_value,
                    post_uuid, post_shortkey, post_value,
                    contact_open_hours_uuid, contact_open_hours_shortkey, contact_open_hours_value,
                    email_remarks_uuid, email_remarks_shortkey, email_remarks_value,
                    contact_uuid, contact_shortkey, contact_value,
                    post_return_uuid, post_return_shortkey, post_return_value,
                    phone_open_hours_uuid, phone_open_hours_shortkey, phone_open_hours_value,
                    operation)";

        private const string INSERT_VALUES = @"
                values (
                    @uuid,
                    @shortkey,
                    @name,
                    @parent_ou_uuid,
                    @payout_ou_uuid,
                    @los_shortname_uuid, @los_shortname_shortkey, @los_shortname_value,
                    @phone_uuid, @phone_shortkey, @phone_value,
                    @email_uuid, @email_shortkey, @email_value,
                    @location_uuid, @location_shortkey, @location_value,
                    @ean_uuid, @ean_shortkey, @ean_value,
                    @post_uuid, @post_shortkey, @post_value,
                    @contact_open_hours_uuid, @contact_open_hours_shortkey, @contact_open_hours_value,
                    @email_remarks_uuid, @email_remarks_shortkey, @email_remarks_value,
                    @contact_uuid, @contact_shortkey, @contact_value,
                    @post_return_uuid, @post_return_shortkey, @post_return_value,
                    @phone_open_hours_uuid, @phone_open_hours_shortkey, @phone_open_hours_value,
                    @operation)";

        public const string INSERT_ITSYSTEMS = @"
                insert into orgunits_itsystems (
                    unit_id,
                    itsystem_uuid)
                values (
                    @orgunit_id,
                    @itsystem_uuid)";

        public const string INSERT_CONTACT_PLACES = @"
                insert into orgunits_contact_places (
                    unit_id,
                    task,
                    contact_place_uuid)
                values (
                    @orgunit_id,
                    @task,
                    @contact_place_uuid)";

        public const string SELECT_MSSQL = @"select top(1) * from orgunits order by timestamp";
        public const string SELECT_SQLITE = @"select * from orgunits order by timestamp LIMIT 1";

        public const string SELECT_ITSYSTEMS = @"select itsystem_uuid from orgunits_itsystems where unit_id = @id";
        public const string SELECT_CONTACT_PLACES = @"select contact_place_uuid, task from orgunits_contact_places where unit_id = @id";

        public const string DELETE = @"delete from orgunits where id = @id";
    }
}
