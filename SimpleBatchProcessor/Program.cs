
using System.Data.SqlClient;
using Sealevel;
using static System.Console;
using static ep5BAS.Data;
using static Sealevel.SeaMAX;

namespace ep5BAS
{
    //  NOTE: this version of the program contains references to inputs, but it does not yet deal with inputs
    class Program
    {
        static int Main()
        {
            int errorCode;
            byte recipeID = 1;

            // BackgroundColor = ConsoleColor.Blue;
            // ForegroundColor = ConsoleColor.White;
            // Clear();

            WriteLine("\nInitializing everything . . . please wait; this will not take long.\n");
            //  zero the data array to be written and initialize the I/O controller
            Array.Clear(SeaMAXdata, 0, SeaMAXdata.Length);
            errorCode = Initializer.InitializeIO(ref SeaMAXdata);
            if (errorCode != SUCCESS)
            {
                WriteLine("Failed initialization; program terminates here...");
                return errorCode;
            }

            //  verify that there is a valid recipe to run
            WriteLine("\nCalling the record counter...\n");
            errorCode = ScanRecipeSteps(recipeID);
            if (errorCode < SUCCESS)
            {
                WriteLine("Failed recipe scan; program terminates here...");
                return errorCode;
            }

            //  start the main event loop and continue it 'til shutdown
            errorCode = MainEventLoop.ProcessRecipe();
            return errorCode;
        }

        //  read through the recipe step records for this ID and store all data in a List<Step>
        private static int ScanRecipeSteps(byte recipeID)
        {
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

            /**********************************************************************
             *
             * Implement the option to set the starting time at Now() + variable
             *
             * *******************************************************************/

            Step recipeStep = new();
            SqlConnection connection = new("Data Source = .\\MSSQLSERVER02; Database = ep5BAS; Integrated Security = SSPI;");
            connection.Open();
            using SqlCommand command = new("select * from dbo.ControlEvent where recipeID = " + recipeID +
                "order by recipeID, dayOfWeek, doAt", connection);
            SqlDataReader dataReader = command.ExecuteReader();
            //  scan SQL table for records for this recipe
            while (dataReader.Read() != false)
            {
                dayOfWeek = (byte)dataReader["dayOfWeek"];
                channelNmbr = (int)dataReader["channelNmbr"];
                recipeStep.dayOfWeek = dayOfWeek;
                recipeStep.channelNmbr = channelNmbr;
                theList.Add(recipeStep);
            }
            dataReader.Close();
            connection.Close();
            WriteLine("\nThis recipe contains {0} steps.\n", theList.Count);
            return SUCCESS;
        }
    }
}
