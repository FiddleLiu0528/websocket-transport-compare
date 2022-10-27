using System.IO;
using System.IO.Compression;
using System.Text;

namespace SocketServer.Models
{
    public static class CompressTool
    {
        // 使用System.IO.Compression进行Deflate压缩
        public static byte[] MicrosoftCompress(byte[] data)
        {
            MemoryStream uncompressed = new MemoryStream(data); // 这里举例用的是内存中的数据；需要对文本进行压缩的话，使用 FileStream 即可
            MemoryStream compressed = new MemoryStream();
            DeflateStream deflateStream = new DeflateStream(compressed, CompressionMode.Compress); // 注意：这里第一个参数填写的是压缩后的数据应该被输出到的地方
            uncompressed.CopyTo(deflateStream); // 用 CopyTo 将需要压缩的数据一次性输入；也可以使用Write进行部分输入
            deflateStream.Close();  // 在Close中，会先后执行 Finish 和 Flush 操作。
            byte[] result = compressed.ToArray();
            return result;
        }

        // 使用System.IO.Compression进行Deflate解压
        public static byte[] MicrosoftDecompress(byte[] data)
        {
            MemoryStream compressed = new MemoryStream(data);
            MemoryStream decompressed = new MemoryStream();
            DeflateStream deflateStream = new DeflateStream(compressed, CompressionMode.Decompress); // 注意： 这里第一个参数同样是填写压缩的数据，但是这次是作为输入的数据
            deflateStream.CopyTo(decompressed);
            byte[] result = decompressed.ToArray();
            return result;
        }

        /// <summary>
        /// GZip the byte.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] GZipByte(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var compressor = new GZipStream(output, CompressionMode.Compress))
                using (var writer = new BinaryWriter(compressor, Encoding.UTF8))
                {
                    writer.Write(data, 0, data.Length);
                    writer.Close();
                }

                return output.ToArray();
            }
        }

        /// <summary>
        /// Deflate the byte.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static byte[] DeflateByte(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var compressor = new DeflateStream(output, CompressionMode.Compress))
                {
                    compressor.Write(data, 0, data.Length);
                    compressor.Close();
                }

                return output.ToArray();
            }
        }
    }
}
