using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Lab15Main
{
    public class PrioritizedThread
    {
        public Thread? Thread = null;
        int _priority = 1;

        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                if (value >= 3)
                {
                    _priority = 3;
                }
                if (value <= 1)
                {
                    _priority = 1;
                }
                else
                {
                    _priority = value;
                }
            }
        }
    }
}