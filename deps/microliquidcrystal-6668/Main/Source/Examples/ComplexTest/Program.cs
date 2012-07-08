// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0 

using System;
using System.Threading;
using MicroLiquidCrystal;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace LCDTest
{
    public class Program
    {
        private static InterruptPort _modeSwitch;
        private static Lcd _lcd;
        private static int _mode;
        private static Thread _testThread;

        public static void Main()
        {
            // create shift register provider
            var lcdProvider = new Shifter74Hc595LcdTransferProvider(SPI_Devices.SPI1, Pins.GPIO_PIN_D10,
                Shifter74Hc595LcdTransferProvider.BitOrder.LSBFirst);

            // construct the LCD object
            _lcd = new Lcd(lcdProvider);

            // set up the LCD's number of columns and rows: 
            _lcd.Begin(16, 2);

            _modeSwitch = new InterruptPort(Pins.ONBOARD_SW1, false, ResistorModes.Disabled, Port.InterruptMode.InterruptEdgeLow);
            _modeSwitch.OnInterrupt += OnModeSwitchInterrupt;

            // run test
            ChangeMode();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void OnModeSwitchInterrupt(uint data1, uint data2, DateTime time)
        {
            _mode = (_mode + 1)%5;
            ChangeMode();
        }

        private static void ChangeMode()
        {
            if (_testThread != null)
                _testThread.Abort();

            _testThread = new Thread(StartTest);
            _testThread.Start();
        }

        private static void StartTest()
        {
            switch (_mode)
            {
                case 0:
                    HelloWorld();
                    break;
                case 1:
                    Blink();
                    break;
                case 2:
                    Cursor();
                    break;
                case 3:
                    SetCursor();
                    break;
                case 4:
                    CreateCharTest();
                    break;
/*
                case 5:
                    AutoScroll();
                    break;
*/
            }
            Thread.Sleep(Timeout.Infinite);
        }

        private static void HelloWorld()
        {
            _lcd.Clear();

            // Print a message to the LCD.
            _lcd.Write("hello, world!");

            while(true)
            {

                // set the cursor to column 0, line 1
                // (note: line 1 is the second row, since counting begins with 0):
                _lcd.SetCursorPosition(0, 1);
                // print the number of seconds since reset:
                _lcd.Write((Utility.GetMachineTime().Ticks / 10000).ToString());

                Thread.Sleep(100);
            }
        }

        private static void Blink()
        {
            _lcd.Clear();

            // Print a message to the LCD.
            _lcd.Write("Blink cursor");

            _lcd.SetCursorPosition(0, 1);

            while (true)
            {
                // Turn off the blinking cursor:
                _lcd.BlinkCursor = false;
                Thread.Sleep(3000);

                // Turn on the blinking cursor:
                _lcd.BlinkCursor = true;
                Thread.Sleep(3000);
            }
        }

        private static void Cursor()
        {
            _lcd.Clear();

            // Print a message to the LCD.
            _lcd.Write("Show cursor");

            _lcd.SetCursorPosition(0, 1);

            while (true)
            {
                // Turn off the cursor:
                _lcd.ShowCursor = false;
                Thread.Sleep(500);

                // Turn on the cursor:
                _lcd.ShowCursor = true;
                Thread.Sleep(500);
            }
        }

        private static void SetCursor()
        {
            const int numRows = 2;
            const int numCols = 16;

            while (true)
            {
                // loop from ASCII 'a' to ASCII 'z':
                for (char thisLetter = 'a'; thisLetter <= 'z'; thisLetter++)
                {
                    // loop over the columns:
                    for (int thisCol = 0; thisCol < numRows; thisCol++)
                    {
                        // loop over the rows:
                        for (int thisRow = 0; thisRow < numCols; thisRow++)
                        {
                            // set the cursor position:
                            _lcd.SetCursorPosition(thisRow, thisCol);
                            // print the letter:
                            _lcd.Write(thisLetter.ToString());
                            Thread.Sleep(200);
                        }
                    }
                }
            }
        }

        private static void CreateCharTest()
        {
            _lcd.Clear();

            byte[] smiley = new byte[]
                                {
                                    0x00,
                                    0x11,
                                    0x00,
                                    0x00,
                                    0x11,
                                    0x0E,
                                    0x00,
                                    0x00
                                };

            _lcd.CreateChar(0, smiley);
            _lcd.WriteByte(0);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
