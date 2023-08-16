//  NOTE:   This code is incomplete and thus nonfunctional.
//          Once we have a working version, it will be updated.
using static System.Console;

namespace ep5BAS
{
    internal class ProcessThe
    {
        //  this logic is executed once per main event loop iteration
        public static int BusinessRules()
        {
            int returnCode = 0;

            if (!(Data.theList.Count > 1))
            {
                WriteLine("BINGO");
                _ = ReadKey(false);
                return returnCode;
            }
            return returnCode;
        }
    }
}
