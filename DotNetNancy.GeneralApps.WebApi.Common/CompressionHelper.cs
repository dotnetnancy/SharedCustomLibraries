using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace DotNetNancy.GeneralApps.WebApi.Common
{
    public class CompressionHelper
    {

        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress, CompressionLevel.BestCompression))
                {
                    zipStream.Write(data, 0, data.Length);
                    zipStream.Close();
                    return compressedStream.ToArray();
                }
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            {
                using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress, CompressionLevel.BestCompression))
                {
                    using (var resultStream = new MemoryStream())
                    {

                        var buffer = new byte[4096];
                        int read;

                        while ((read = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            resultStream.Write(buffer, 0, read);
                        }
                        //zipStream.Close();
                        return resultStream.ToArray();
                    }
                }
            }
        }
    }
}