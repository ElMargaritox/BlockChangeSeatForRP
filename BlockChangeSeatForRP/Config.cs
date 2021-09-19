using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;

namespace BlockChangeSeatForRP
{
    public class Config : IRocketPluginConfiguration
    {
        public int cooldown { get; set; }
        public bool Lock_to_change_seats { get; set; }
        public void LoadDefaults()
        {
            cooldown = 10;
            Lock_to_change_seats = false;
        }
    }
}
