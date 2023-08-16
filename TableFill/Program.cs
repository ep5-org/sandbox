//  this program does not have exception handling yet

using System.Data.SQLite;


namespace InsertRecords
{
    class Program
    {
        const string DROP_SQL = "drop table if exists ControlEvent";
        const string CREATE_SQL = @"
            CREATE TABLE ControlEvent(
                recipeID INTEGER NOT NULL,
                dayOfWeek INTEGER NOT NULL CHECK(dayOfWeek >= 0 AND dayOfWeek <= 6),
                channelNmbr INTEGER NOT NULL,
                doAt TIME NOT NULL,
                -- CURRENT_TIMESTAMP is only second precision
                whenEntered DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                whenEdited DATETIME NOT NULL,
                enteredBy INTEGER NOT NULL,
                editedBy INTEGER NOT NULL,
                digitalValue BOOLEAN NOT NULL,
                notes TEXT,
                status INTEGER NOT NULL
            )";
        const string INSERT_SQL = @"
            INSERT INTO ControlEvent (
                recipeID, dayOfWeek, channelNmbr, doAt, whenEdited, enteredBy,
                editedBy, digitalValue, notes, status
            ) VALUES(
                @recipeID, @dayOfWeek, @channelNmbr, @doAt, @whenEdited, @enteredBy, @editedBy,
                @digitalValue, @notes, @status
            )";
        static void Main()
        {
            int recipeID;
            byte dayOfWeek;
            int channelNmbr;
            TimeOnly doAt;
            int enteredBy;
            int editedBy;
            bool digitalValue;
            string notes;
            int status;
            TimeSpan hopTime = TimeSpan.FromMilliseconds(100);

            var connection = new SQLiteConnection("Data Source=ep5BAS.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = DROP_SQL;
            command.ExecuteNonQuery();

            command.CommandText = CREATE_SQL;
            command.ExecuteNonQuery();


            command.CommandText = INSERT_SQL;

            recipeID = 1;
            dayOfWeek = 0;
            channelNmbr = 0;        //  this starts with the first output point and quickly runs through all twenty-four in a sequential fashion
            doAt = TimeOnly.FromDateTime(DateTime.Now).Add(TimeSpan.FromSeconds(30));   //  this enables the program to be tested with current usable recipe data
            enteredBy = 1;
            editedBy = enteredBy;
            digitalValue = true;
            notes = "Test entry - disregard.";
            status = 1;

            command.Parameters.AddWithValue("@recipeID", recipeID);
            command.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
            command.Parameters.AddWithValue("@channelNmbr", channelNmbr);
            command.Parameters.AddWithValue("@doAt", doAt.ToString("HH:mm:ss.fff"));
            command.Parameters.AddWithValue("@whenEdited", DateTime.UtcNow);
            command.Parameters.AddWithValue("@enteredBy", enteredBy);
            command.Parameters.AddWithValue("@editedBy", editedBy);
            command.Parameters.AddWithValue("@digitalValue", digitalValue);
            command.Parameters.AddWithValue("@notes", notes);
            command.Parameters.AddWithValue("@status", status);
            command.ExecuteNonQuery();

            for (int indx = 1; indx < 48; ++indx)
            {
                if (indx % 2 == 0)
                {
                    channelNmbr++;
                    command.Parameters["@channelNmbr"].Value = channelNmbr;
                }
                if (digitalValue == true)
                    digitalValue = false;
                else
                    digitalValue = true;
                command.Parameters["@digitalValue"].Value = digitalValue;
                doAt = doAt.Add(hopTime);
                command.Parameters["@doAt"].Value = doAt.ToString("HH:mm:ss.fff");
                command.Parameters["@whenEdited"].Value = DateTime.UtcNow;
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}
