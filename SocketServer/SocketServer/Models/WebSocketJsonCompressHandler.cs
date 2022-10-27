using Bogus;
using SocketServer.Action;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SocketServer.Models
{
    public class WebSocketJsonCompressHandler
    {
        public WebSocketJsonCompressHandler(ILogger<WebSocketJsonHandler> logger)
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
                    var sliceByte = buffer.Take(res.Count).ToArray();

                    var decompressed = CompressTool.MicrosoftDecompress(sliceByte);

                    var cmd = Encoding.UTF8.GetString(decompressed);

                    var obj = JsonSerializer.Deserialize<ActionBase>(cmd);

                    if (obj?.action != null)
                    {
                        logger.LogInformation(cmd);

                        if (obj.action == "register")
                        {
                            var temp = JsonSerializer.Deserialize<ActionRegister.Request>(cmd);
                            RegisterResponseHandler(temp?.id ?? "?");
                        }
                        else if (obj.action == "first")
                        {
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
            var result = new ActionRegister.Response()
            {
                action = "register",
                id = userId
            };

            var str = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            var buff = Encoding.UTF8.GetBytes(str);
            var compressedBuff = CompressTool.MicrosoftCompress(buff);
            var data = new ArraySegment<byte>(compressedBuff, 0, compressedBuff.Length);
            Parallel.ForEach(WebSockets.Values, async (webSocket) =>
            {
                if (webSocket.State == WebSocketState.Open)
                    await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
            });
        }

        public void FirstResponseHandler()
        {
            var res = GenerateFirstResponse();

            var buff = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(res));
            var compressedBuff = CompressTool.MicrosoftCompress(buff);
            var data = new ArraySegment<byte>(compressedBuff, 0, compressedBuff.Length);
            Parallel.ForEach(WebSockets.Values, async (webSocket) =>
            {
                if (webSocket.State == WebSocketState.Open)
                    await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
            });
        }


        public static ActionFirst.Response GenerateFirstResponse()
        {
            var data = new Faker<ActionFirst.Data>()
            .RuleFor(a => a.schId, (f, u) => 1)
            .RuleFor(a => a.mode, (f, u) => 1)
            .RuleFor(a => a.pt, (f, u) => 1)
            .RuleFor(a => a.people, (f, u) => 1)
            .RuleFor(a => a.schT, (f, u) =>
            {
                return new Faker<ActionFirst.Scht>()
                       .RuleFor(a1 => a1.tnA, (f, u) => "team A")
                       .RuleFor(a1 => a1.tnB, (f, u) => "team B")
                       .RuleFor(a1 => a1.tcA, (f, u) => "team country A")
                       .RuleFor(a1 => a1.tcA, (f, u) => "team country B");
            }
            )
            .RuleFor(a => a.liveId, (f, u) => 1)
            .RuleFor(a => a.liveName, (f, u) => "liveName")
            .RuleFor(a => a.liveLang, (f, u) => "LiveLang")
            .RuleFor(a => a.alName, (f, u) => "AlName")
            .Generate(1);

            var aldata = new Faker<ActionFirst.Aldata>()
                .Generate();

            aldata.pt = new[] { "admin", "manager" };

            var first = new Faker<ActionFirst.Response>()
                .RuleFor(a => a.action, (f, u) => "first")
                .RuleFor(a => a.dc, (f, u) => 1)
                .Generate();

            first.menu = new int[] { 1, 2, 3 };
            first.index = new int[] { 1, 2, 3 };
            first.data = data.ToArray();
            first.alMenu = new int[] { 1, 2, 3 };
            first.alData = aldata;

            return first;
        }
    }
}