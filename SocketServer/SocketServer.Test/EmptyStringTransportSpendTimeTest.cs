using SocketServer.Action;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
using SocketServer.Models;
using Google.Protobuf;

namespace SocketServer.Test
{
    [TestClass]
    public class EmptyStringTransportSpendTimeTest
    {
        private int loopTimes { get; set; } = 10000;
        private ActionRegister.Response resultForJson { get; set; } = new ActionRegister.Response() { action = "register" };
        private Register.Response resultForProtobuf { get; set; } = new Register.Response() { Action = "register" };
        private Stopwatch stopWatch { get; set; } = new Stopwatch();

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void SerilizeJsonAndConvertToByteArray_SpandTime()
        {
            stopWatch.Start();

            // Execute
            for (int i = 0; i < loopTimes; i++)
            {
                var str = JsonSerializer.Serialize(resultForJson, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                });

                var buff = Encoding.UTF8.GetBytes(str);
                var data = new ArraySegment<byte>(buff, 0, buff.Length);
            }

            stopWatch.Stop();

            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }

        [TestMethod]
        public void SerilizeJsonAndCompressAndConvertToByteArray_SpandTime()
        {
            stopWatch.Start();

            // Execute
            for (int i = 0; i < loopTimes; i++)
            {
                var str = JsonSerializer.Serialize(resultForJson, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                });

                var buff = Encoding.UTF8.GetBytes(str);
                var compressedBuff = CompressTool.MicrosoftCompress(buff);
                var data = new ArraySegment<byte>(compressedBuff, 0, compressedBuff.Length);
            }

            stopWatch.Stop();

            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }

        [TestMethod]
        public void SerilizeProtobufAndByteArray_SpandTime()
        {
            stopWatch.Start();

            // Execute
            for (int i = 0; i < loopTimes; i++)
            {
                using (var ms = new MemoryStream())
                {
                    resultForProtobuf.WriteTo(ms);
                    var buff = ms.ToArray();
                    var data = new ArraySegment<byte>(buff, 0, buff.Length);
                }
            }

            stopWatch.Stop();

            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }
    }
}