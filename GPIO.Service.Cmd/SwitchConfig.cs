using System;
using System.Collections.Generic;
using System.Text;

namespace GPIO.Service.Cmd
{
    public class SwitchConfig
    {
        public int Pin { get; set; }
        public int SwitchOnHour { get; set; }
        public int SwitchOffHour { get; set; }
    }
}
