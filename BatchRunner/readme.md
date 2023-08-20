Show channels available in hardware:

```
$ dotnet run channels
0: KitchenLight1
1: KitchenLight2
2: KitchenLight3
3: PorchLight1
4: PorchLight2

<...snip...>
```

"Turn on" channels and show the bytes that would be sent to the hardware:

```
$ dotnet run pack KitchenLight1 PorchLight1 AlarmBell
Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00001001 (9)
  - KitchenLight1 (0): on
  - PorchLight1 (3): on
  Bank 1: 00000000 (0)
  Bank 2: 10000000 (128)
  - AlarmBell (23): on
  Packed bytes[] = [9, 0, 128]
```

Initialize the database and run a simple recipe:

```
$ dotnet run init-db
Database initialized


$ dotnet run run porch-lights-on
Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00000000 (0)
  Bank 1: 00000000 (0)
  Bank 2: 00000000 (0)
  Packed bytes[] = [0, 0, 0]
Apply: sending bytes to Hardware
Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00011000 (24)
  - PorchLight1 (3): on
  - PorchLight2 (4): on
  Bank 1: 00000000 (0)
  Bank 2: 00000000 (0)
  Packed bytes[] = [24, 0, 0]
Apply: sending bytes to Hardware
```

Run a more complex recipe:

```
$ dotnet run run sequential
Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00000000 (0)
  Bank 1: 00000000 (0)
  Bank 2: 00000000 (0)
  Packed bytes[] = [0, 0, 0]
Apply: sending bytes to Hardware
Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00001001 (9)
  - KitchenLight1 (0): on
  - PorchLight1 (3): on
  Bank 1: 00010001 (17)
  - LivingRoomLight1 (8): on
  - LibraryLight1 (12): on
  Bank 2: 00000000 (0)
  Packed bytes[] = [9, 17, 0]
Apply: sending bytes to Hardware
Delaying (ms): 1000
Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00011011 (27)
  - KitchenLight1 (0): on
  - KitchenLight2 (1): on
  - PorchLight1 (3): on
  - PorchLight2 (4): on
  Bank 1: 00110011 (51)
  - LivingRoomLight1 (8): on
  - LivingRoomLight2 (9): on
  - LibraryLight1 (12): on
  - LibraryLight2 (13): on
  Bank 2: 00000000 (0)
  Packed bytes[] = [27, 51, 0]
Apply: sending bytes to Hardware
Delaying (ms): 1000

...snip...

Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00000000 (0)
  Bank 1: 00000000 (0)
  Bank 2: 00000000 (0)
  Packed bytes[] = [0, 0, 0]
Apply: sending bytes to Hardware
Delaying (ms): 5000
Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00000000 (0)
  Bank 1: 00000000 (0)
  Bank 2: 10000000 (128)
  - AlarmBell (23): on
  Packed bytes[] = [0, 0, 128]
Apply: sending bytes to Hardware
Delaying (ms): 5000
Packing channels: MSB -> LSB, i.e. Bank 0 is channels 7 -> 0
  Bank 0: 00000000 (0)
  Bank 1: 00000000 (0)
  Bank 2: 00000000 (0)
  Packed bytes[] = [0, 0, 0]
Apply: sending bytes to Hardware
```
