using System;
using System.Text;
using System.Threading;
using FusionWare.SPOT.Hardware;
using MFToolkit.Net.Ntp;
using MicroLiquidCrystal;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;

namespace NetworkClock
{
    public class Program
    {
        private static Lcd _lcd;
        private static I2CBus _bus;
        private static DS1307RealTimeClock _clock;
        private static Timer _lcdTimer;
        private static Timer _ntpTimer;

        public const int ClockUpdateMinutes = 15;

        public static void Main()
        {
            // initialize i2c bus (only one instance is allowed)
            _bus = new I2CBus();

            // initialize provider (multiple devices can be attached to same bus)
            var lcdProvider = new MCP23008LcdTransferProvider(_bus);

            // create the LCD interface
            _lcd = new Lcd(lcdProvider);

            // set up the LCD's number of columns and rows: 
            _lcd.Begin(16, 2);

            // Print a message to the LCD.
            _lcd.Write("Netduino clock");

            // initialize RTC clock
            _clock = new DS1307RealTimeClock(_bus);

            // TODO: Do this only once to set your clock
            // clock.SetClock(new DateTime(2010,11,25,8,17,32));

            _clock.SetLocalTimeFromRTC();
            Debug.Print("The RTC time is: " + DateTime.Now);

            // set timer to update display
            _lcdTimer = new Timer(UpdateDisplay, null, 500, 500);

            // update time now and then every 15 minutes
            _ntpTimer = new Timer(UpdateTime, null, TimeSpan.Zero, new TimeSpan(0, ClockUpdateMinutes, 0));
            
            // subscribe to network change events
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;

            // end of main 
            Thread.Sleep(Timeout.Infinite);
        }

        static void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                // update timer immediately when network is available
                _ntpTimer.Change(TimeSpan.Zero, new TimeSpan(0, ClockUpdateMinutes, 0));
            }
        }

        private static void UpdateTime(object state)
        {
            try
            {
                _lcd.Clear();
                _lcd.Write("Updating time...");
                _lcd.SetCursorPosition(0,1);

                // get time from default server
                var dt = NtpClient.GetNetworkTime();
                Debug.Print("The network time is: " + dt);

                // set RTC clock
                _clock.SetClock(dt);

                _lcd.Write("Done");
            }
            catch(Exception ex)
            {
                _lcd.Write("Network error!");
                // ignore errors
                Debug.Print("Error updating time");
            }
        }

        // Use single line character buffer to avoid flickering
        // caused by calling lcd.Clear. 
        // This way new text will overwrite existing characters. 
        static readonly byte[] _lineBuffer = new byte[16];

        private static byte[] FillLine(string text)
        {
            // fill empty space
            for (int i = 0; i < _lineBuffer.Length; i++) 
                _lineBuffer[i] = (byte)' ';

            // write new text
            var bytes = Encoding.UTF8.GetBytes(text);
            bytes.CopyTo(_lineBuffer, 0);

            return _lineBuffer;
        }

        private static void UpdateDisplay(object state)
        {
            var dt = DateTime.Now;

            // write time 
            _lcd.Home();
            _lcd.Write(FillLine(dt.ToString("hh:mm:ss")), 0, 16);

            // write date
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write(FillLine(dt.ToString("dd/MM/yyyy")), 0, 16);
        }
    }
}
