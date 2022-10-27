namespace SocketServer.Action
{
    public class ActionRegister
    {
        public class Request
        {
            public string action { get; set; } = string.Empty;
            public string id { get; set; } = string.Empty;
        }

        public class Response
        {
            public string action { get; set; } = string.Empty;
            public string id { get; set; } = string.Empty;
        }
    }
}