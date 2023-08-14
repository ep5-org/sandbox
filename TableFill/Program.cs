//  this program does not have exception handling yet

using System.Data.SqlClient;

namespace InsertRecords
{
    class Program
    {
        static void Main()
        {
            int recipeID;
            byte dayOfWeek;
            int channelNmbr;
            TimeOnly doAt;
            DateTime whenEntered;
            DateTime whenEdited;
            int enteredBy;
            int editedBy;
            bool digitalValue;
            string notes;
            int status;
            TimeSpan hopTime = TimeSpan.FromMilliseconds(100);

            SqlConnection connection = new("Server = .\\MSSQLSERVER02; Database = ep5BAS; Integrated Security = SSPI; Connection Timeout = 2; Timeout = 2");
            connection.Open();
            string query = "truncate table dbo.ControlEvent;";
            SqlCommand command = new(query, connection);
            command.ExecuteNonQuery();
            query = "INSERT INTO dbo.ControlEvent (recipeID, dayOfWeek, channelNmbr, doAt, whenEntered, whenEdited, enteredBy, editedBy, digitalValue, notes, status) " +
                "VALUES(@recipeID, @dayOfWeek, @channelNmbr, @doAt, @whenEntered, @whenEdited, @enteredBy, @editedBy, @digitalValue, @notes, @status)";
            command = new(query, connection);

            recipeID = 1;
            dayOfWeek = 0;
            channelNmbr = 0;        //  this starts with the first output point and quickly runs through all twenty-four in a sequential fashion
            doAt = TimeOnly.FromDateTime(DateTime.Now).Add(TimeSpan.FromSeconds(30));   //  this enables the program to be tested with current usable recipe data
            whenEntered = whenEdited = DateTime.Now;
            enteredBy = 1;
            editedBy = enteredBy;
            digitalValue = true;
            notes = "Test entry - disregard.";
            status = 1;

            command.Parameters.AddWithValue("@recipeID", recipeID);
            command.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
            command.Parameters.AddWithValue("@channelNmbr", channelNmbr);
            command.Parameters.AddWithValue("@doAt", doAt.ToString("HH:mm:ss.fff"));
            command.Parameters.AddWithValue("@whenEntered", whenEntered);
            command.Parameters.AddWithValue("@whenEdited", whenEdited);
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
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}
