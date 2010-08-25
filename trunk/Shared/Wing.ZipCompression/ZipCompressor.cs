using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Wing.ZipCompression
{
    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    /// Helper class.
    /// </summary>
    public class ZipCompressor
    {
        /// <summary>
        /// Compresses string.
        /// </summary>
        /// <param name="str">String to compress.</param>
        /// <returns></returns>
        public static byte[] Compress(string str)
        {
            using (var ms = new MemoryStream())
            {
                using (var outStream = new ZipOutputStream(ms))
                {
                    outStream.PutNextEntry(new ZipEntry("ZipEntry"));

                    var writer = new StreamWriter(outStream);
                    writer.Write(str);
                    // flush any pending data
                    writer.Flush();

                    outStream.CloseEntry();

                    ms.Seek(0, SeekOrigin.Begin);

                    byte[] zipped = ms.ToArray();

                    outStream.Flush();
                    outStream.Finish();

                    return zipped;
                }
            }
        }

        /// <summary>
        /// Compresses string and convert result to base64 string.
        /// </summary>
        /// <param name="str">String to compress.</param>
        /// <returns></returns>
        public static string CompressAndConvertToBase64String(string str)
        {
            byte[] array_zipped_out = ZipCompressor.Compress(str);
            String zipped_str = Convert.ToBase64String(array_zipped_out);
            return zipped_str;
        }

        /// <summary>
        /// Decompress base64 string.
        /// </summary>
        /// <param name="str">Base64 string to decompress.</param>
        /// <returns></returns>
        public static string DecompressFromBase64String(string str)
        {
            byte[] array_zipped_in = Convert.FromBase64String(str);
            String unzipped_str = ZipCompressor.Decompress(array_zipped_in);
            return unzipped_str;
        }

        /// <summary>
        /// Decompresses stream.
        /// </summary>
        /// <param name="ms1">Stream to decompress.</param>
        /// <returns></returns>
        public static string Decompress(Stream ms1)
        {
            using (var inStream = new ZipInputStream(ms1))
            {
                int extractCount = 0;
                byte[] decompressedData = new byte[1024];

                byte[] unzipped = null;
                ZipEntry zipped_entry = inStream.GetNextEntry();
                using (var outputStream = new MemoryStream())
                {
                    if (zipped_entry != null)
                    {
                        while (true)
                        {
                            int numRead = inStream.Read(decompressedData, 0, decompressedData.Length);
                            if (numRead <= 0)
                            {
                                outputStream.Position = 0;
                                unzipped = outputStream.ToArray();
                                break;
                            }
                            extractCount += numRead;
                            outputStream.Write(decompressedData, 0, numRead);
                        }
                    }
                    else
                    {
                        throw new Exception("Archive is corrupted. No archive entry found.");
                    }
                    inStream.Close();

                    var reader = new StreamReader(outputStream);
                    String str = reader.ReadToEnd();
                    outputStream.Flush();
                    outputStream.Close();
                    return str;
                }
            }
        }

        /// <summary>
        /// Decompresses bytes array.
        /// </summary>
        /// <param name="data">Array to decompress.</param>
        /// <returns></returns>
        public static string Decompress(byte[] data)
        {
            using (var inStream = new MemoryStream(data))
            {
                inStream.Position = 0;
                return ZipCompressor.Decompress(inStream);
            }
        }


    }
}
