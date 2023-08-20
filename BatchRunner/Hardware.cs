namespace BatchRunner.Hardware;

using System.Collections;
using BatchRunner.Extensions;

public interface iHardware {
    public void Initialize();
    public void SetChannel(string channelName, int value);
    public byte[] Pack();
    public (string, int)[] ChannelItems();
    public void Apply();
}

class SeaMAX: iHardware {
    BitArray channelState;
    Dictionary<int, string> channelLabels;
    public static readonly Dictionary<string, int> Channels = new()
    {
        {"KitchenLight1", 0},
        {"KitchenLight2", 1},
        {"KitchenLight3", 2},
        {"PorchLight1", 3},
        {"PorchLight2", 4},
        {"PantryLight", 5},
        {"Radio", 6},
        {"ServerRoomLight", 7},
        {"LivingRoomLight1", 8},
        {"LivingRoomLight2", 9},
        {"LivingRoomLight3", 10},
        {"HallLight", 11},
        {"LibraryLight1", 12},
        {"LibraryLight2", 13},
        {"DownstairsStoreroomLight", 14},
        {"SPARE_OUTPUT1", 15},
        {"MasterBedroomLight", 16},
        {"UpstairsStoreroomLight", 17},
        {"SecondBedroomLight", 18},
        {"AtticLight", 19},
        {"AtticEntranceLight", 20},
        {"UpstairsBathroomLight", 21},
        {"SPARE_OUTPUT2", 22},
        {"AlarmBell", 23},
    };

    public SeaMAX() {
        channelState = new BitArray(Channels.Count);
        channelLabels = Channels.ToDictionary(x => x.Value, x => x.Key);
    }

    int bankCount => Channels.Count / 8;

    IEnumerable<int> bankRange => Enumerable.Range(0, bankCount);

    public void Initialize(){
        // TODO: setup the SeaMAX API and connect to device

        // channelState is initialized with False by default so applying now should set all
        // channels to 0 unless SetChannel() has been called for some reason before Initialize().
        Apply();
    }

    public void SetChannel(string channelName, int value){
        int channelNum = Channels[channelName];
        channelState.Set(channelNum, value != 0);
    }

    public byte[] Pack() {
        var bytes = new byte[bankCount];
        Console.WriteLine("Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0");

        foreach (int bank in bankRange) {
            var offset = bank * 8;
            bytes[bank] = channelState.ExtractByteReversed(offset);

            Console.Write($"  Bank {bank}: ");
            foreach (var channel in Enumerable.Range(offset, 8).Reverse()) {
                var repr = channelState[channel] ? "1" : "0";
                Console.Write(repr);
            }
            Console.WriteLine($" ({bytes[bank]})");

            foreach (var channel in Enumerable.Range(offset, 8)) {
                if (channelState[channel]) {
                    Console.WriteLine($"  - {channelLabels[channel]} ({channel}): on");
                }
            }
        }
        Console.WriteLine($"  Packed bytes[] = [{string.Join(", ", bytes)}]");
        return bytes;
    }

    public (string, int)[] ChannelItems() {
        return Channels.Select(kvp => (Name: kvp.Key, Number: kvp.Value)).ToArray();
    }

    public void Apply(){
        var bytes = Pack();
        Console.WriteLine("Apply: sending bytes to Hardware");
        // TODO: actually send the bytes to the hardware using the SeaMAX API
    }
}
