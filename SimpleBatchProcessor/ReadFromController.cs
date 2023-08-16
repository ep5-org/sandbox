
namespace ep5BAS
{
    internal class ReadFromController
    {
        public static int ReadIOcontroller() 
        {
            int returnCode = 0;

            //  read the output channels of the I/O controller
            returnCode = Data.SeaMAX_DeviceHandler.SM_ReadPIO(Data.previousSeaMAXdata);
            MoveDataToArray();
            return returnCode;
        }

        private static void MoveDataToArray()
        {
            ;
        }
    }
}
