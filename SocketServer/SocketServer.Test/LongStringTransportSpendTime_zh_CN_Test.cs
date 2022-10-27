using SocketServer.Action;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
using SocketServer.Models;
using Google.Protobuf;
using Bogus;
using System;

namespace SocketServer.Test
{
    [TestClass]
    public class LongStringTransportSpendTime_zh_CN_Test
    {
        private static int loopTimes { get; set; } = 10000;
        private static int textLength { get; set; } = 65535; // base on 1024 * 64 >> setting memory size,and need reduce property action length
        private static ActionRegister.Response resultForJson { get; set; } = new ActionRegister.Response() { action = "register" };
        private static Register.Response resultForProtobuf { get; set; } = new Register.Response() { Action = "register" };
        private Stopwatch stopWatch { get; set; } = new Stopwatch();

        [ClassInitialize()]
        public static void Init(TestContext context)
        {
            string target = new Faker("zh_CN").Lorem.Paragraph(2000);

            string idString = target.Substring(0,textLength);
            resultForJson.id = idString;
            resultForProtobuf.Id = idString;
        }

        [TestMethod]
        public void SerilizeJsonAndConvertToByteArray_SpandTime()
        {
            Console.WriteLine($"data length: {resultForJson.id.Length}");

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

            Console.WriteLine($"spend time: {stopWatch.ElapsedMilliseconds}");
        }


        [TestMethod]
        public void SerilizeJsonAndConvertToByteArray_Count()
        {
            var str = JsonSerializer.Serialize(resultForJson, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            var buff = Encoding.UTF8.GetBytes(str);
            var data = new ArraySegment<byte>(buff, 0, buff.Length);

            Console.WriteLine($"Count: {data.Count}");
        }

        [TestMethod]
        public void SerilizeJsonAndCompressAndConvertToByteArray_SpandTime()
        {
            Console.WriteLine($"data length: {resultForJson.id.Length}");

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

            Console.WriteLine($"spend time: {stopWatch.ElapsedMilliseconds}");
        }

        [TestMethod]
        public void SerilizeJsonAndCompressAndConvertToByteArray_Count()
        {
            var str = JsonSerializer.Serialize(resultForJson, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });

            var buff = Encoding.UTF8.GetBytes(str);
            var compressedBuff = CompressTool.MicrosoftCompress(buff);
            var data = new ArraySegment<byte>(compressedBuff, 0, compressedBuff.Length);

            Console.WriteLine($"Count: {data.Count}");
        }

        [TestMethod]
        public void SerilizeProtobufAndByteArray_SpandTime()
        {
            Console.WriteLine($"data length: {resultForProtobuf.Id.Length}");

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

            Console.WriteLine($"spend time: {stopWatch.ElapsedMilliseconds}");
        }

        [TestMethod]
        public void SerilizeProtobufAndByteArray_Count()
        {
            using (var ms = new MemoryStream())
            {
                resultForProtobuf.WriteTo(ms);
                var buff = ms.ToArray();
                var data = new ArraySegment<byte>(buff, 0, buff.Length);

                Console.WriteLine($"Count: {data.Count}");
            }
        }
    }
}