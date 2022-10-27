using System;
using static First.Response.Types;

namespace SocketServer.Action
{
    public class ActionFirst
    {
        public class Request
        {
            public string action { get; set; } = string.Empty;
            public int mode { get; set; }
            public int pt { get; set; }
            public int sort { get; set; }
            public int keepL { get; set; }
            public int dc { get; set; }
        }

        public class Response
        {
            public string action { get; set; } = string.Empty;
            public int[] menu { get; set; } 
            public int[] index { get; set; }
            public Data[]? data { get; set; }
            public int[] alMenu { get; set; }
            public Aldata? alData { get; set; }
            public int dc { get; set; }
        }


        public class Data
        {
            public int schId { get; set; } 
            public int mode { get; set; }
            public int pt { get; set; } 
            public int people { get; set; }
            public Scht schT { get; set; } 
            public int liveId { get; set; }
            public string liveName { get; set; } = string.Empty;
            public string liveLang { get; set; } = string.Empty;
            public string alName { get; set; } = string.Empty;
        }

        public class Scht
        {
            public string tnA { get; set; } = string.Empty;
            public string tnB { get; set; } = string.Empty;
            public string tcA { get; set; } = string.Empty;
            public string tcB { get; set; } = string.Empty;
        }

        public class Aldata
        {
            public string[] pt { get; set; } 
        }
    }
}