using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Client
    {
        public string Name { set; get; }
        public Socket s { set; get; }
        public Thread th { set; get; }
        
    }
}
