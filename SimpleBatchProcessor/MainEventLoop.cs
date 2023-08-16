
using static System.Console;
using static ep5BAS.Data;

namespace ep5BAS
{
    public class MainEventLoop
    {
        public static int ProcessRecipe()
        {
            int returnCode;

            WriteLine("Starting the recipe...\n");
            TimeOnly timeTrigger = new();
            TimeOnly rightNow = new();
            //  create the output stateImage with 24 points
            for (byte indx = 0; indx < 24; indx++)
            {
                digitalPoints[indx] = new DigitalPoint
                {
                    channelID = indx,
                    currentState = false,
                    commandedState = false,
                    flagOne = 0,
                    flagTwo = 0
                };
            }

            //  read the input and output states from the I/O controller and populate the arrays with the results
            returnCode = ReadFromController.ReadIOcontroller();     //  for now, this is a placeholder
            if (returnCode < SUCCESS)
            {
                WriteLine("Error in reading inputs and outputs from the I/O controller.");
                WriteLine("The error code is {0}.", returnCode);
                return returnCode;
            }

            //  set up for the main event loop
            WriteLine("\n\nRunning the main event loop...\n\n");
            foreach (Step step in theList)      //  this processes the entire recipe step by step
            {
                timeTrigger = step.doAt;
                rightNow = TimeOnly.FromDateTime(DateTime.Now);
                while (TimeOnly.FromDateTime(DateTime.Now) < timeTrigger)
                {
                    //rightNow = TimeOnly.FromDateTime(DateTime.Now);
                    if (KeyAvailable)           //  put into separate thread?
                    {
                        char c = ReadKey(true).KeyChar;
                        if ((c == 'q') || (c == 'Q'))
                            return FAILURE;
                    }
                }
                WriteLine("time = {0}\tchannel = {1};\tstate = {2}", timeTrigger.ToString("HH:mm:ss.fff"), step.channelNmbr, step.digitalValue);
                digitalPoints[step.channelNmbr].channelID = step.channelNmbr;
                digitalPoints[step.channelNmbr].currentState = step.digitalValue;
                WriteLine("{0}\t{1}", digitalPoints[step.channelNmbr].channelID, digitalPoints[step.channelNmbr].currentState);
                //  these two operate in tandem and are I/O hardware-specific
                Assemble.TheOutputArray(digitalPoints);
                WriteToController.WriteIOcontroller();
            }

            return SUCCESS;
        }
    }
}
