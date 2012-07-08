// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0 

using System.Threading;
using MicroLiquidCrystal;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HelloWorld
{
    public class Program
    {
        public static void Main()
        {
            // create the transfer provider
/* 
            // Option 1: Use direct GPIO provider
            // Initialize the library with the numbers of the interface pins
            // Use wiring shown here http://arduino.cc/en/uploads/Tutorial/lcd_schem.png
            var lcdProvider = new GPIO_LCD_TransferProvider(Pins.GPIO_PIN_D12, Pins.GPIO_PIN_D11, //Pins.GPIO_PIN_D10, 
                //Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D6, 
                Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D2);
*/

            // Option 2: Use shift register provider
            var lcdProvider = new Shifter74Hc595LcdTransferProvider(SPI_Devices.SPI1, Pins.GPIO_PIN_D10, 
                Shifter74Hc595LcdTransferProvider.BitOrder.LSBFirst);

            // create the LCD interface
            var lcd = new Lcd(lcdProvider);

            // set up the LCD's number of columns and rows: 
            lcd.Begin(16, 2);

            // Print a message to the LCD.
            lcd.Write("hello, world!");

            while (true)
            {
                // set the cursor to column 0, line 1
                lcd.SetCursorPosition(0, 1);

                // print the number of seconds since reset:
                lcd.Write((Utility.GetMachineTime().Ticks / 10000).ToString());

                Thread.Sleep(100);
            }
        }
    }
}
