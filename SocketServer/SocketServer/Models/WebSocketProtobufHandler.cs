using Bogus;
using Google.Protobuf;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SocketServer.Models
{
    public class WebSocketProtobufHandler
    {
        public WebSocketProtobufHandler(ILogger<WebSocketJsonHandler> logger)
        {
            this.logger = logger;
        }

        //REF: https://radu-matei.com/blog/aspnet-core-websockets-middleware/
        ConcurrentDictionary<int, WebSocket> WebSockets = new ConcurrentDictionary<int, WebSocket>();
        private readonly ILogger<WebSocketJsonHandler> logger;

        public async Task ProcessWebSocket(WebSocket webSocket)
        {
            try
            {
                WebSockets.TryAdd(webSocket.GetHashCode(), webSocket);
                var buffer = new byte[1024 * 64]; // limit max received length
                var res = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!res.CloseStatus.HasValue)
                {
                    var request = buffer.Take(res.Count).ToArray();
                    var action = Base.Base.Parser.ParseFrom(request).Action;

                    if (!string.IsNullOrEmpty(action))
                    {
                        logger.LogInformation(action);

                        if (action == "register")
                        {
                            var temp = Register.Request.Parser.ParseFrom(request);
                            RegisterResponseHandler(temp.Id);
                        }
                        else if (action == "first")
                        {
                            var temp = First.Request.Parser.ParseFrom(request);
                            FirstResponseHandler();
                        }
                    }
                    res = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync(res.CloseStatus.Value, res.CloseStatusDescription, CancellationToken.None);
                WebSockets.TryRemove(webSocket.GetHashCode(), out var removed);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void RegisterResponseHandler(string userId)
        {
            var result = new Register.Response()
            {
                Action = "register",
                Id = userId
            };

            using (var ms = new MemoryStream())
            {
                result.WriteTo(ms);
                var buff = ms.ToArray();

                var data = new ArraySegment<byte>(buff, 0, buff.Length);
                Parallel.ForEach(WebSockets.Values, async (webSocket) =>
                {
                    if (webSocket.State == WebSocketState.Open)
                        await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
                });
            }
        }

        public void FirstResponseHandler()
        {
            var result = GenerateFirstResponse();

            using (var ms = new MemoryStream())
            {
                result.WriteTo(ms);
                var buff = ms.ToArray();

                var data = new ArraySegment<byte>(buff, 0, buff.Length);
                Parallel.ForEach(WebSockets.Values, async (webSocket) =>
                {
                    if (webSocket.State == WebSocketState.Open)
                        await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
                });
            }
        }

        static First.Response GenerateFirstResponse()
        {
            var data = new Faker<First.Response.Types.Data>()
                .RuleFor(a => a.SchId, (f, u) => (uint)1)
                .RuleFor(a => a.Mode, (f, u) => (uint)1)
                .RuleFor(a => a.Pt, (f, u) => (uint)1)
                .RuleFor(a => a.People, (f, u) => (uint)1)
                .RuleFor(a => a.SchT, (f, u) =>
                    {
                        return new Faker<First.Response.Types.Scht>()
                           .RuleFor(a1 => a1.TnA, (f, u) => "team A")
                           .RuleFor(a1 => a1.TnB, (f, u) => "team B")
                           .RuleFor(a1 => a1.TcA, (f, u) => "team country A")
                           .RuleFor(a1 => a1.TcA, (f, u) => "team country B");
                    }
                )
                .RuleFor(a => a.LiveId, (f, u) => (uint)1)
                .RuleFor(a => a.LiveName, (f, u) => "liveName")
                .RuleFor(a => a.LiveLang, (f, u) => "LiveLang")
                .RuleFor(a => a.AlName, (f, u) => "AlName")
                .Generate(1);

            var aldata = new Faker<First.Response.Types.Aldata>()
                .Generate();

            aldata.Pt.Add(new[] { "admin", "manager" });

            var first = new Faker<First.Response>()
                .RuleFor(a => a.Action, (f, u) => "first")
                .RuleFor(a => a.Dc, (f, u) => (uint)1)
                .Generate();

            first.Menu.Add(new uint[] { 1, 2, 3 });
            first.Index.Add(new uint[] { 1, 2, 3 });
            first.Data.AddRange(data);
            first.AlMenu.Add(new uint[] { 1, 2, 3 });
            first.AlData= aldata;

            return first;
        }
    }
}