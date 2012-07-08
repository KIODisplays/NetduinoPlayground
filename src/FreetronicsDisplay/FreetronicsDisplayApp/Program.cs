using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MicroLiquidCrystal;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace FreetronicsDisplayApp
{
    public class Program
    {
        public static void Main()
        {
            // initialise the LCD display
            var ledPort = new OutputPort(Pins.ONBOARD_LED, false);
            var backlightPort = new OutputPort(Pins.GPIO_PIN_D3, false);
            var lcdProvider = new MicroLiquidCrystal.GpioLcdTransferProvider(
                Pins.GPIO_PIN_D8,  // RS
                Pins.GPIO_PIN_D9,  // ENABLE
                Pins.GPIO_PIN_D4,  // D4
                Pins.GPIO_PIN_D5,  // D5
                Pins.GPIO_PIN_D6,  // D6
                Pins.GPIO_PIN_D7); // D7
            var lcd = new Lcd(lcdProvider);
            lcd.Begin(16, 2);

            lcd.Clear();
            lcd.SetCursorPosition(2, 0);

            backlightPort.Write(true);

            lcd.Write("Ready ... ");

            const int interval = 10;
            const int reset = 1000;
            int duration = 0;
            while (true)
            {
                // set the cursor to column 0, line 1
                lcd.SetCursorPosition(0, 1);

                // print the number of seconds since reset:
                var time = Utility.GetMachineTime();
                lcd.Write(time.ToString());

                ledPort.Write(duration > reset/2);

                Thread.Sleep(interval);
                duration += interval;
                if (duration >= reset)
                    duration = 0;
            }
        }

    }
}
