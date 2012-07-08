using System;
using FusionWare.SPOT.Hardware;
using Microsoft.SPOT.Hardware;

namespace NetworkClock
{
    /// <summary>
    /// This class interfaces with the DS1307 real time clock chip via the I2C bus.
    /// To wire the chip to a netduino board, wire as follows:
    /// SDA -> Analog Pin 4
    /// SCL -> Analog Pin 5
    /// GND -> GND
    /// 5V  -> 5V
    /// </summary>
    public class DS1307RealTimeClock : I2CDeviceDriver
    {
        private const int DS1307_Address = 0x68;
        private const int DS1307_ClockRateKhz = 100;

        public const int UserDataAddress = 8;
        public const int UserDataLength = 56;

        private static class Register
        {
            public const int Seconds    = 0x00;
            public const int Minutes    = 0x01;
            public const int Hours      = 0x02;
            public const int DayOfWeek  = 0x03;
            public const int Day        = 0x04;
            public const int Month      = 0x05;
            public const int Year       = 0x06;
            public const int Control    = 0x07;
        }

        public DS1307RealTimeClock(I2CBus bus)
            : base(bus, DS1307_Address, DS1307_ClockRateKhz)
        {
        }

        /// <summary>
        /// Set the local .NET time from the RTC board. You can do this on startup then call
        /// DateTime.Now during program execution.
        /// </summary>
        public void SetLocalTimeFromRTC()
        {
            var dt = Now();
            Utility.SetLocalTime(dt);
        }

        public void SetClock(DateTime dt)
        {
            SetClock(
                (byte) (dt.Year - 2000), 
                (byte) dt.Month, 
                (byte) dt.Day, 
                (byte) dt.Hour, 
                (byte) dt.Minute, 
                (byte) dt.Second, 
                dt.DayOfWeek);
        }


        /// <summary>
        /// This method sets the real time clock. The current implementation does not take into account control
        /// registers on the DS1307. They can be easily added if needed.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="dayofWeek"></param>
        /// <returns></returns>
        public void SetClock(byte year, byte month, byte day, byte hour, byte minute, byte second, DayOfWeek dayofWeek)
        {
            // Set the time
            var buffer = new byte[] 
                             {
                                 Register.Seconds, // Address of first register
                                 second.ToBCD(),
                                 minute.ToBCD(),
                                 hour.ToBCD(),
                                 ((byte)(dayofWeek + 1)).ToBCD(),
                                 day.ToBCD(),
                                 month.ToBCD(),
                                 year.ToBCD()
                             };

            Write(buffer);
        }

        /// <summary>
        /// Reads data from the DS1307 clock registers and returns it as a .NET DateTime.
        /// </summary>
        /// <returns></returns>
        public DateTime Now()
        {
            var data = new byte[7];
            WriteRead(new byte[] { Register.Seconds }, data);

            //TODO: Add exception handling if result == 0

            var dt = new DateTime(
                2000 + data[Register.Year].FromBCD(),               // Year
                data[Register.Month].FromBCD(),                     // Month
                data[Register.Day].FromBCD(),                      // Day
                ((byte)(data[Register.Hours] & 0x3f)).FromBCD(),    // Hour
                data[Register.Minutes].FromBCD(),                   // Minute
                ((byte)(data[Register.Seconds] & 0x7f)).FromBCD()   // Second
                );

            return dt;
        }


        /// <summary>
        /// Write data to the clock memory. Normally, this will be used for writing to the user data area.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        public void Write(byte address, byte[] data)
        {
            byte[] buffer = new byte[57];
            buffer[0] = address;
            data.CopyTo(buffer, 1);

            Write(buffer);
        }

        /// <summary>
        /// Read data from the clock memory. Normally this will be used for reading data from the user memory area.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Read(byte address, byte[] data)
        {
            WriteRead(new[] { address }, data);
        }

    }
}