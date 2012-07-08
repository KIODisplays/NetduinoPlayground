// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0 

using System.Threading;
using MicroLiquidCrystal;
using SecretLabs.NETMF.Hardware.Netduino;

namespace PavelBanskyDemo
{
    public class Program
    {
        // Demo from http://bansky.net/blog/2008/10/interfacing-lcd-with-3-wires-from-net-micro-framework

        public static void Main()
        {
            // create the transfer provider
            var lcdProvider = new Shifter74Hc595LcdTransferProvider(SPI_Devices.SPI1, Pins.GPIO_PIN_D10,
                Shifter74Hc595LcdTransferProvider.BitOrder.LSBFirst);

            // create the LCD interface
            var lcd = new Lcd(lcdProvider);

            // set up the LCD's number of columns and rows: 
            lcd.Begin(16, 2);

            // Creating custom characters (Smiley face and gimp)
            byte[] buffer = new byte[] {    0x07, 0x08, 0x10, 0x10, 0x13, 0x13, 0x10, 0x10,
                                            0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04,
                                            0x1C, 0x02, 0x01, 0x01, 0x19, 0x19, 0x01, 0x01,
                                            0x10, 0x10, 0x12, 0x11, 0x10, 0x10, 0x08, 0x07,
                                            0x00, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00, 0x1F,
                                            0x01, 0x01, 0x09, 0x11, 0x01, 0x01, 0x02, 0x1C,

                                            0x15, 0x15, 0x0E, 0x04, 0x04, 0x0A, 0x11, 0x11,
                                            0x04, 0x04, 0x0E, 0x15, 0x04, 0x0A, 0x11, 0x11
                                       };

            // Load custom characters to display CGRAM
            for (int i = 0; i < 8; i++)
            {
                lcd.CreateChar(i, buffer, i * 8);
            }

            // Turn displat on, turn back light on, hide small cursor, show big blinking cursor
            lcd.BlinkCursor = true;

            lcd.Clear();
            lcd.Write("Start me up!");
            Thread.Sleep(3000);

            lcd.Clear();
            lcd.BlinkCursor = false;

            // Print the special characters with the face
            lcd.Write(new byte[] { 0x00, 0x01, 0x02 }, 0, 3);
            lcd.Write(" .NET Micro");

            // Move to second line
            lcd.SetCursorPosition(0, 1);

            // Print the special characters with the face
            lcd.Write(new byte[] { 0x03, 0x04, 0x05 }, 0, 3);
            lcd.Write("  Framework");
            Thread.Sleep(2000);

            // Blink with back light
            for (int i = 0; i < 4; i++)
            {
                lcd.Backlight = (i % 2) != 0;
                Thread.Sleep(400);
            }

            lcd.Clear();
            const string message = "* Hello World! *";
            // Let gimp write the message
            for (int i = 0; i < message.Length; i++)
            {
                lcd.SetCursorPosition(i, 1);
                lcd.WriteByte((byte)(((i % 2) == 0) ? 0x06 : 0x07));

                lcd.SetCursorPosition(i, 0);
                lcd.Write(message[i].ToString());

                Thread.Sleep(200);

                lcd.SetCursorPosition(i, 1);
                lcd.Write(" ");
            }
            Thread.Sleep(1500);

            lcd.Clear();
            lcd.SetCursorPosition(16, 0);

            lcd.Write("http://bansky.net/blog");

            // Scroll the page url
            while (true)
            {
                lcd.ScrollDisplayLeft();
                Thread.Sleep(400);
            }
        }

    }
}
