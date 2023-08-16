//  needs exception handling

using static System.Console;

namespace ep5BAS
{
    internal class WriteToController
    {
        public static int WriteIOcontroller()
        {
            int returnCode;

            //  write the output channels of the I/O controller
            returnCode = Data.SeaMAX_DeviceHandler.SM_WritePIO(Data.SeaMAXdata);
            //  this is temporary
            //  it should be replaced by exception handling and event logging
            WriteLine("The output write returned {0}.", returnCode);
            return returnCode;
        }
    }
}
