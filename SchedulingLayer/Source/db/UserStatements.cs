namespace Organisation.SchedulingLayer
{
    public class UserStatements
    {
        public const string CREATE_TABLE_MSSQL = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='users' and xtype='U') CREATE TABLE users (
	            id			            BIGINT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	            timestamp		        DATETIME2 NOT NULL DEFAULT GETDATE(),
	            user_uuid		        NVARCHAR(36) NOT NULL,
	            user_shortkey	    	NVARCHAR(50),
	            user_id			        NVARCHAR(64),
	            user_phone_uuid	    	NVARCHAR(36),
	            user_phone_shortkey 	NVARCHAR(50),
	            user_phone_value    	NVARCHAR(64),
	            user_email_uuid	    	NVARCHAR(36),
	            user_email_shortkey 	NVARCHAR(50),
	            user_email_value    	NVARCHAR(64),
	            user_location_uuid	    NVARCHAR(36),
	            user_location_shortkey	NVARCHAR(50),
	            user_location_value	    NVARCHAR(64),
	            position_uuid		    NVARCHAR(36),
	            position_shortkey	    NVARCHAR(50),
	            position_name		    NVARCHAR(64),
	            position_orgunit_uuid	NVARCHAR(36),
	            person_uuid		        NVARCHAR(36),
	            person_shortkey		    NVARCHAR(50),
	            person_name		        NVARCHAR(64),
	            person_cpr		        NVARCHAR(11),
	            operation		        NVARCHAR(16) NOT NULL,

	            CONSTRAINT user_operation_check CHECK (
		            operation IN('UPDATE', 'DELETE')
	            ),
	            CONSTRAINT user_delete_check CHECK (
		            operation = 'DELETE' OR (
			            user_id IS NOT NULL AND person_name IS NOT NULL
		            )
	            ))";

        public const string CREATE_CHILD_TABLE_MSSQL = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='user_positions' AND xtype='U') CREATE TABLE user_positions (
	            id			            BIGINT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	            user_id			        BIGINT NOT NULL FOREIGN KEY REFERENCES users(id) ON DELETE CASCADE,
	            uuid		            NVARCHAR(36),
	            shortkey	            NVARCHAR(50),
	            name		            NVARCHAR(64) NOT NULL,
	            orgunit_uuid	        NVARCHAR(36) NOT NULL
            );";

        public const string CREATE_TABLE_SQLITE = @"CREATE TABLE IF NOT EXISTS users (
	            id			            INTEGER PRIMARY KEY,
	            timestamp		        DATETIME DEFAULT CURRENT_TIMESTAMP,
	            user_uuid		        NVARCHAR(36) NOT NULL,
	            user_shortkey	    	NVARCHAR(50),
	            user_id			        NVARCHAR(64),
	            user_phone_uuid	    	NVARCHAR(36),
	            user_phone_shortkey 	NVARCHAR(50),
	            user_phone_value    	NVARCHAR(64),
	            user_email_uuid	    	NVARCHAR(36),
	            user_email_shortkey 	NVARCHAR(50),
	            user_email_value    	NVARCHAR(64),
	            user_location_uuid  	NVARCHAR(36),
	            user_location_shortkey	NVARCHAR(50),
	            user_location_value 	NVARCHAR(64),
	            position_uuid	    	NVARCHAR(36),
	            position_shortkey   	NVARCHAR(50),
	            position_name	    	NVARCHAR(64),
	            position_orgunit_uuid	NVARCHAR(36),
	            person_uuid	        	NVARCHAR(36),
	            person_shortkey 		NVARCHAR(50),
	            person_name	        	NVARCHAR(64),
	            person_cpr	        	NVARCHAR(11),
	            operation		        NVARCHAR(16) NOT NULL)";

        public const string CREATE_CHILD_TABLE_SQLITE = @"CREATE TABLE IF NOT EXISTS user_positions (
	            id			            INTEGER NOT NULL PRIMARY KEY,
	            user_id			        BIGINT NOT NULL,
	            uuid		            NVARCHAR(36),
	            shortkey	            NVARCHAR(50),
	            name		            NVARCHAR(64) NOT NULL,
	            orgunit_uuid        	NVARCHAR(36) NOT NULL,

                FOREIGN KEY(user_id) REFERENCES users(id) ON DELETE CASCADE
            );";


        public const string INSERT_MSSQL = INSERT_OUTPUT + " output INSERTED.ID " + INSERT_VALUES;
        public const string INSERT_SQLITE = INSERT_OUTPUT + " " + INSERT_VALUES;

        private const string INSERT_OUTPUT = @"insert into users (user_uuid,user_shortkey,user_id,user_phone_uuid,user_phone_shortkey,user_phone_value,user_email_uuid,user_email_shortkey,user_email_value, user_location_uuid, user_location_shortkey, user_location_value, person_uuid, person_shortkey, person_name,person_cpr,operation)";
        private const string INSERT_VALUES = @"values(@user_uuid, @user_shortkey, @user_id, @user_phone_uuid, @user_phone_shortkey, @user_phone_value, @user_email_uuid, @user_email_shortkey, @user_email_value, @user_location_uuid, @user_location_shortkey, @user_location_value, @person_uuid, @person_shortkey,@person_name, @person_cpr,@operation)";

        public const string INSERT_POSITION = @"
                insert into user_positions (
                    user_id,
                    uuid,
                    shortkey,
                    name,
                    orgunit_uuid)
                values (
                    @user_id,
                    @uuid,
                    @shortkey,
                    @name,
                    @orgunit_uuid)";

        public const string SELECT_MSSQL = @"select top(1) * from users order by timestamp";
        public const string SELECT_SQLITE = @"select * from users order by timestamp LIMIT 1";

        public const string SELECT_POSITIONS = @"select uuid, shortkey, name, orgunit_uuid from user_positions where user_id = @id";

        public const string DELETE = @"DELETE FROM users where id = @id";
    }
}
