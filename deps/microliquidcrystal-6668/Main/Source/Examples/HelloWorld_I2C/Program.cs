// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0 

using System.Diagnostics;
using System.Threading;
using FusionWare.SPOT.Hardware;
using MicroLiquidCrystal;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HelloWorld_I2C
{
    public class Program
    {
        public static void Main()
        {
            // Option 1: Use I2C provider. 
            // Default configuration coresponds to Adafruit's LCD backpack

            // initialize i2c bus (only one instance is allowed)
            var bus = new I2CBus();

            // initialize provider (multiple devices can be attached to same bus)
            var lcdProvider = new MCP23008LcdTransferProvider(bus);

/*
            // Option 2: Adafruit's LCD backup can also work in SIP mode.
            // this setup enabled this pinout.
            var lcdProvider = new Shifter74Hc595LcdTransferProvider(SPI_Devices.SPI1, Pins.GPIO_PIN_D10,
                Shifter74Hc595LcdTransferProvider.BitOrder.MSBFirst,
                new Shifter74Hc595LcdTransferProvider.ShifterSetup
                {
                    RS = ShifterPin.GP0,
                    RW = ShifterPin.None,
                    Enable = ShifterPin.GP1,
                    D4 = ShifterPin.GP5,
                    D5 = ShifterPin.GP4,
                    D6 = ShifterPin.GP3,
                    D7 = ShifterPin.GP2,
                    BL = ShifterPin.GP6
                });
*/

            // create the LCD interface
            var lcd = new Lcd(lcdProvider);

            // set up the LCD's number of columns and rows: 
            lcd.Begin(16, 2);

            // Print a message to the LCD.
            lcd.Write("hello, world!");

            Stopwatch sw = Stopwatch.StartNew();

            while (true)
            {
                sw.Start();

                // set the cursor to column 0, line 1
                lcd.SetCursorPosition(0, 1);

                // print the number of seconds since reset:
                lcd.Write((Utility.GetMachineTime().Ticks / 10000).ToString());

                Debug.Print(sw.ElapsedMilliseconds.ToString());
                sw.Reset();

                Thread.Sleep(100);

            //    lcd.Backlight = !lcd.Backlight;
            }
        }

    }
}
