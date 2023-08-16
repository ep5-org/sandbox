using static System.Console;

namespace ep5BAS
{
    internal class Initializer
    {
        public static int InitializeIO(ref byte[] SeaMAXdata)           //  set things up
        {
            {
                byte[] SeaMAXpresets = new byte[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                //  from left to right
                byte[] SeaMAXdirections = new byte[12] { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
                int errorNumber;

                try
                {
                    if (!Data.SeaMAX_DeviceHandler.IsEthernetInitialized)
                    {
                        WriteLine("Opening the Ethernet I/O processor...");
                        Data.SeaMAX_DeviceHandler.SME_Initialize();
                        WriteLine("Opened the Ethernet I/O processor.");
                    }

                    //  do the initial search for modules on the network
                    WriteLine("Beginning scan for I/O controllers...");
                    int ModuleCount = Data.SeaMAX_DeviceHandler.SME_SearchForModules();
                    WriteLine("Modules found = {0}", ModuleCount.ToString());
                    if (ModuleCount == 0)
                    {
                        WriteLine("No I/O devices were found.");
                        WriteLine("Press the <ANY> key to continue....");
                        _ = ReadKey();
                        return 9;
                    }
                    else if (ModuleCount < 0)
                    {
                        WriteLine("Error " + ModuleCount.ToString() + " searching for devices.");
                        _ = ReadKey();
                        return 9;
                    }

                    WriteLine(ModuleCount.ToString() + " device(s) found.");
                    //  select the first device found
                    errorNumber = Data.SeaMAX_DeviceHandler.SME_FirstModule();
                    if (errorNumber < 0)
                    {
                        WriteLine("Error selecting first device.");
                        _ = ReadKey();
                        return 9;
                    }

                    //  ping the device to ensure that it is still available
                    errorNumber = Data.SeaMAX_DeviceHandler.SME_Ping();

                    //  save the IP address
                    string ip = "";
                    string netmask = "";
                    string gateway = "";
                    Data.SeaMAX_DeviceHandler.SME_GetNetworkConfig(ref ip, ref netmask, ref gateway);
                    if (errorNumber < 1)
                    {
                        WriteLine("The device at " + ip + " failed to respond.");
                        return 9;
                    }

                    string name = "";
                    errorNumber = Data.SeaMAX_DeviceHandler.SME_GetName(ref name);
                    if (errorNumber < 0)
                    {
                        WriteLine("Could not retrieve name of device at " + ip);
                        return 9;
                    }

                    WriteLine("\nThe I/O processor at " + ip + " is identified as " + name + ".");
                    WriteLine("It uses net mask {0} and gateway {1}.", netmask, gateway);

                    errorNumber = Data.SeaMAX_DeviceHandler.SM_Open(ip);
                    if (errorNumber < 0)
                    {
                        WriteLine("Open error = " + errorNumber + ".");
                    }

                    if (Data.SeaMAX_DeviceHandler.IsSeaMAXOpen)
                        WriteLine("SeaMAX is open.");

                    WriteLine("Calling GetPIOSettings");
                    //set up the array which holds the pio directions
                    byte[] direction = new byte[12];
                    //set up the array which holds the pio output presets
                    byte[] presets = new byte[12];
                    //GetPIOSettings(SeaMAX_DeviceHandler, direction, presets);
                    WriteLine("Back from GetPIOSettings");

                    //  set up the hardware with presets
                    errorNumber = Data.SeaMAX_DeviceHandler.SM_SetPIOPresets(SeaMAXpresets);
                    WriteLine("Set presets return value = " + errorNumber + ".");
                    errorNumber = Data.SeaMAX_DeviceHandler.SM_SetPIODirection(SeaMAXdirections);
                    WriteLine("Set direction return value = " + errorNumber + ".");
                }
                catch (Exception e)
                {
                    WriteLine(e.ToString());
                }
                finally
                {
                    //	reset the three digital output ports to OFF
                    SeaMAXdata[0] = 0;
                    SeaMAXdata[1] = 0;
                    SeaMAXdata[2] = 0;
                    //errorNumber = Program.SeaMAX_DeviceHandler.SM_WritePIO(SeaMAXdata);
                    errorNumber = WriteToController.WriteIOcontroller();
                    if (errorNumber < 0)
                    {
                        WriteLine("\n\nWriting the digital outputs returned error code: {0}\n\n", errorNumber);
                        _ = ReadKey();
                    }
                    else
                        WriteLine("\nEnd of initialization...\n");
                }

                return errorNumber;
            }
        }
    }
}
