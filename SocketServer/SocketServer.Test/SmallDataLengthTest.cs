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
    public class SmallDataLengthTest
    {
        private ActionRegister.Response resultForJson { get; set; } = new ActionRegister.Response() { action = "register" };
        private Register.Response resultForProtobuf { get; set; } = new Register.Response() { Action = "register" };

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void SerilizeJsonAndConvertToByteArray_SpandTime()
        {
            var str = JsonSerializer.Serialize(resultForJson, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            var buff = Encoding.UTF8.GetBytes(str);
            var data = new ArraySegment<byte>(buff, 0, buff.Length);

            Console.WriteLine($"Count:{data.Count}");
        }

        [TestMethod]
        public void SerilizeJsonAndCompressAndConvertToByteArray_SpandTime()
        {
            var str = JsonSerializer.Serialize(resultForJson, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            var buff = Encoding.UTF8.GetBytes(str);
            var compressedBuff = CompressTool.MicrosoftCompress(buff);
            var data = new ArraySegment<byte>(compressedBuff, 0, compressedBuff.Length);

            Console.WriteLine($"Count:{data.Count}");
        }

        [TestMethod]
        public void SerilizeProtobufAndByteArray_SpandTime()
        {
            using (var ms = new MemoryStream())
            {
                resultForProtobuf.WriteTo(ms);
                var buff = ms.ToArray();
                var data = new ArraySegment<byte>(buff, 0, buff.Length);

                Console.WriteLine($"Count:{data.Count}");
            }
        }
    }
}