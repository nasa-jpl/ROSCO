# RockDetector Unit Tests

To run the unit test suite

1. Copy the file `ESP_046282_north_botRst.pgm` to the `RockDetector/test` directory.
2. Run `./RockDetector/build/bin/RunTests ./RockDetector/test` (Linux, Mac) or `./RockDetector/build/bin/Release/RunTests ./RockDetector/test` (Windows).

The unit test suite verifies the current results match legacy results, which are saved in `ESP_046282_Rocks_north_bot.txt`.

If the results have intentionally changed then update `ESP_046282_Rocks_north_bot.txt` as follows:

```
cd RockDetector
./build/bin/RockDetector -i test/ESP_046282_north_botRst.pgm -p test/ESP_046282_params.txt -r test/ESP_046282_Rocks_north_bot.txt
```

# RockDetector Testing with Valgrind

The following command can be used to run the [valgrind](https://www.valgrind.org) MemCheck tool on Linux:

```
cd RockDetector
valgrind -v --track-origins=yes ./build/bin/RockDetector -i test/ESP_046282_north_botRst.pgm -p test/ESP_046282_params.txt -r test/ESP_046282_Rocks_north_bot_valgrind.txt
```
