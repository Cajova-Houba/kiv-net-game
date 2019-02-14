using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Map.Serializer.Binary
{
    /// <summary>
    /// Utility class with common methods for binary (de)serializetion.
    /// </summary>
    public class BinarySerializerUtils
    {
        /// <summary>
        /// Converts integer value to byte array of length 4 using little endian format:
        ///            0     1   2   3
        /// int 4 =   [4]   [0] [0] [0]
        /// int 256 = [0]   [1] [0] [0]
        /// int 511 = [255] [1] [0] [0]
        /// 
        /// </summary>
        /// <param name="intVal"></param>
        /// <returns></returns>
        public static byte[] IntToBytes(int intVal)
        {
            return new byte[]
            {
                (byte)(intVal & 255),
                (byte)((intVal >> 8) & 255),
                (byte)((intVal >> 16) & 255),
                (byte)((intVal >> 24) & 255)
            };
        }

        /// <summary>
        /// Converts name to list of bytes. If the name is longer than maxNameLen, it is trimmed to correct size.
        /// If the name is null, returns 4 bytes with value 0.
        /// </summary>
        /// <param name="str">Name to be converted.</param>
        /// <param name="maxBytes">Max number of allowed bytes.</param>
        /// <returns></returns>
        public static List<byte> StrToBytes(string str, int maxNameLen)
        {
            List<byte> res = new List<byte>();
            if (str == null || str.Length == 0)
            {
                res.AddRange(IntToBytes(0));
                return res;
            }
            else if (str.Length > maxNameLen)
            {
                str = str.Substring(0, maxNameLen);
            }

            byte[] encodedName = Encoding.UTF8.GetBytes(str);
            res.AddRange(IntToBytes(encodedName.Length));
            res.AddRange(encodedName);

            return res;
        }
    }
}
