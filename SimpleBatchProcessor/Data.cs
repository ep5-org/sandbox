using Sealevel;

namespace ep5BAS
{
    public static class Data
    {
        //  the current actual machine state is stored in two 24-byte arrays containing the values of all input and all output channels
        //      this maps 1:1 to the physical hardware
        //      note that the hardware supports reading the outputs at any time; thus, a full read operation scans all 48 channels

        //  once per main event loop iteration, all outputs are set as required by the recipe and/or program business rules
        //      this data is placed in a 24-byte array which can then be compared to the existing output machine state to
        //      determine whether a new write operation is or is not required for this iteration
        public static byte[] newMachineState = new byte[48];
        public static bool readNextRecord = true;
        public static int indexer = 0;
        public static byte[] SeaMAXdata = new byte[3];
        //  this is the previous data that was written to the I/O controller on the preceding iteration
        public static byte[] previousSeaMAXdata = new byte[3];
        public static List<Step> theList = new();
        public static SeaMAX SeaMAX_DeviceHandler = new();
        public struct Step
        {
            public byte dayOfWeek;
            public int channelNmbr;
            public TimeOnly doAt;
            public bool digitalValue;
        }
        public struct DigitalPoint          //  these comprise the state image
        {
            public int channelID;           //  must match existing channel ID number from application
            public bool currentState;     
            public bool commandedState;   
            public ushort flagOne;
            public ushort flagTwo;
        }
        static public DigitalPoint[] digitalPoints = new DigitalPoint[IOpointCount];    //  the state image
        public const byte IOpointCount = 48;
        public const int SUCCESS = 0;
        public const int FAILURE = 99;
        enum DigitalChannel
        {
            KitchenLight1 = 0,
            KitchenLight2 = 1,
            KitchenLight3 = 2,
            PorchLight1 = 3,
            PorchLight2 = 4,
            PantryLight = 5,
            Radio = 6,
            ServerRoomLight = 7,
            LivingRoomLight1 = 8,
            LivingRoomLight2 = 9,
            LivingRoomLight3 = 10,
            HallLight = 11,
            LibraryLight1 = 12,
            LibraryLight2 = 13,
            DownstairsStoreroomLight = 14,
            SPARE_OUTPUT1 = 15,
            MasterBedroomLight = 16,
            UpstairsStoreroomLight = 17,
            SecondBedroomLight = 18,
            AtticLight = 19,
            AtticEntranceLight = 20,
            UpstairsBathroomLight = 21,
            SPARE_OUTPUT2 = 22,
            AlarmBell = 23,
            //  inputs
            AC_INPUT1 = 24,
            AC_INPUT2 = 25,
            AC_INPUT3 = 26,
            AC_INPUT4 = 27,
            AC_INPUT5 = 28,
            AC_INPUT6 = 29,
            AC_INPUT7 = 30,
            AC_INPUT8 = 31,
            AC_INPUT9 = 32,
            AC_INPUT10 = 33,
            AC_INPUT11 = 34,
            AC_INPUT12 = 35,
            AC_INPUT13 = 36,
            AC_INPUT14 = 37,
            AC_INPUT15 = 38,
            AC_INPUT16 = 39,
            DC_INPUT1 = 40,
            DC_INPUT2 = 41,
            DC_INPUT3 = 42,
            DC_INPUT4 = 43,
            DC_INPUT5 = 44,
            DC_INPUT6 = 45,
            DC_INPUT7 = 46,
            DC_INPUT8 = 47
        }
    }
}
