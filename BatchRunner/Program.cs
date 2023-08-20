namespace BatchRunner;
using BatchRunner.Hardware;
using BatchRunner.DataModel;


static class Program
{
    static iHardware hardware = new SeaMAX();

    static void Main(string[] args)
    {

        switch(args[0]) {
            case "pack":
                pack(args.Skip(1).ToArray());
                break;
            case "channels":
                channels();
                break;
            case "init-db":
                initDb();
                break;
            case "run":
                if (args.Length < 2) {
                    Console.WriteLine("Error: run command requires an argument but none given");
                    return;
                }
                run(args[1]);
                break;
            default:
                Console.WriteLine($"Error: argument {args[0]} not recognized");
                break;
        }

    }

    static void run(string recipeName) {
        var br = new BatchRecipe();

        hardware.Initialize();
        br.Run(recipeName, hardware);
    }

    static void pack(string[] channelNames) {
        foreach (var name in channelNames) {
            hardware.SetChannel(name, 1);
        }
        hardware.Pack();
    }

    static void channels() {
        foreach ((string name, int number) in hardware.ChannelItems()) {
            Console.WriteLine($"{number}: {name}");
        }
    }

    static void initDb() {
        var br = new BatchRecipe();
        br.CreateTable();
        Console.WriteLine("Database initialized");

        // Turn on both porch lights without delay
        br.CreateRecipe(
            "porch-lights-on",
            new Step[] {
                new Step("PorchLight1", ChannelState.On),
                new Step("PorchLight2", ChannelState.On, ApplyAfter: true),
            }
        );

        // Turn on lights in rooms with more than one light in sequence then sound the alarm.
        br.CreateRecipe(
            "sequential",
            new Step[] {
                // All these will turn on at the same time, then a 1 second delay
                new Step("KitchenLight1", ChannelState.On),
                new Step("PorchLight1", ChannelState.On),
                new Step("LivingRoomLight1", ChannelState.On),
                new Step("LibraryLight1", ChannelState.On, DelayAfter: 1000),

                // All these will turn on at the same time, then a 1 second delay
                new Step("KitchenLight2", ChannelState.On),
                new Step("PorchLight2", ChannelState.On),
                new Step("LivingRoomLight2", ChannelState.On),
                new Step("LibraryLight2", ChannelState.On, DelayAfter: 1000),

                // All these will turn on at the same time, then a 5 second delay
                new Step("KitchenLight3", ChannelState.On),
                new Step("LivingRoomLight3", ChannelState.On, DelayAfter: 5000),

                // Turn everything off in the reverse order of how they got turned on
                new Step("KitchenLight3", ChannelState.Off),
                new Step("LivingRoomLight3", ChannelState.Off, DelayAfter: 1000),

                new Step("KitchenLight2", ChannelState.Off),
                new Step("PorchLight2", ChannelState.Off),
                new Step("LivingRoomLight2", ChannelState.Off),
                new Step("LibraryLight2", ChannelState.Off, DelayAfter: 1000),

                new Step("KitchenLight1", ChannelState.Off),
                new Step("PorchLight1", ChannelState.Off),
                new Step("LivingRoomLight1", ChannelState.Off),
                new Step("LibraryLight1", ChannelState.Off, DelayAfter: 5000),

                // Turn on the alarm for five seconds
                new Step("AlarmBell", ChannelState.On, DelayAfter: 5000),
                new Step("AlarmBell", ChannelState.Off, ApplyAfter: true)
            }
        );
    }
}
