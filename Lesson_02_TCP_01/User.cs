using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_02_TCP_01
{
    [Serializable]
    internal class User
    {
        public string IPv4Address { get; set; } = string.Empty;
        public DateTime ConnectionTime { get; set; }
        public List<string> Quotes { get; set; } = new List<string>();
        public DateTime DisconnectionTime { get; set; }
    }
}
