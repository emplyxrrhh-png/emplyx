using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PushCompressor
{
    // Wrapper class for the highly performant LZO compression library
    public class LZOCompressor
    {
        #region Dll-Imports

        [DllImport("lzo.dll")]
        private static extern int __lzo_init3();

        [DllImport("lzo.dll")]
        private static extern string lzo_version_string();
        
        [DllImport("lzo.dll")]
        private static extern string lzo_version_date();

		[DllImport("lzo.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int lzo1x_1_compress(
			byte[] src,
			int src_len,
			byte[] dst,
			ref int dst_len,
			byte[] wrkmem
			);

        //[DllImport("lzo.dll")] // .NET 2.0
        [DllImport("lzo.dll", CallingConvention = CallingConvention.Cdecl)] // .NET 4.0: Need to use CallingConvection
        private static extern int lzo1x_decompress(byte[] src, int src_len, byte[] dst, ref int dst_len, byte[] wrkmem);

        private byte[] _workMemory = new byte[16384L * 4]; // LZO1X_MEM_COMPRESS

        #endregion

        static LZOCompressor()
        {
            int init = __lzo_init3();

            if (init != 0)
                throw new Exception("Initialization of LZO-Compressor failed !");
        }

        // Constructor
        public LZOCompressor()
        {
        }

        // Version string of the compression library
        public string Version
        {
            get
            {
                return lzo_version_string();
            }
        }

        // Version date of the compression library
        public string VersionDate
        {
            get
            {
                return lzo_version_date();
            }
        }

        public byte[] Decrypt(byte[] data, byte[] key)
        {
            LZOCompressor comp = new LZOCompressor();
            byte[] xorDecoded2 = XOREncode2(data);
            byte[] decodedBytes = comp.Decompress(xorDecoded2);
            byte[] xorDecoded = XOREncode(decodedBytes, key);

            return xorDecoded;
        }

        public byte[] Encrypt(byte[] data, byte[] key)
        {
            LZOCompressor comp = new LZOCompressor();
            //byte[] xoredContent = this.Encoding.GetBytes(EncryptOrDecrypt(this.Encoding.GetString(data), this.Encoding.GetString(key)));

            byte[] xoredContent = XOREncode(data, key);
            byte[] contentEncodedBytes = comp.Compress(xoredContent);
            byte[] xored2Content = XOREncode2(contentEncodedBytes);
            return xored2Content;
        }

        public string Decrypt(string data, string key)
        {
            string contentDecoded = Encoding.GetEncoding("iso-8859-1").GetString(Decrypt(Encoding.GetEncoding("iso-8859-1").GetBytes(data), Encoding.GetEncoding("iso-8859-1").GetBytes(key)));

            return contentDecoded;
        }

        public string Encrypt(string data, string key)
        {
            string contentEncoded = Encoding.GetEncoding("iso-8859-1").GetString(Encrypt(Encoding.GetEncoding("iso-8859-1").GetBytes(data), Encoding.GetEncoding("iso-8859-1").GetBytes(key)));
            return contentEncoded;
        }

        public byte[] Decompress(byte[] src)
        {
            int origlen = 50000;
            byte[] dst = new byte[origlen];
            int outlen = origlen;
            lzo1x_decompress(src, src.Length, dst, ref outlen, _workMemory);

            return dst;
        }

        public byte[] Compress(byte[] src)
        {
           
            byte[] dst = new byte[src.Length + src.Length / 64 + 16 + 3];
            int outlen = 0;
            lzo1x_1_compress(src, src.Length, dst, ref outlen, _workMemory);
          
            byte[] ret = new byte[outlen];
            Array.Copy(dst, 0, ret, 0, outlen);
            return ret;
        }

        private byte[] XOREncode(byte[] data, byte[] key)
        {
            int dataLen = data.Length;
            int keyLen = key.Length;
            byte[] output = new byte[dataLen];
            int j = 0;

            for (int i = 0; i < dataLen; i++)
            {
                output[i] = (byte)(data[i] ^ key[j]);
                j = j + 1;

                if (j >= keyLen)
                    j = 0;

            }

            return output;
        }

        private byte[] XOREncode2(byte[] data)
        {
            int size = data.Length;
            byte[] output = new byte[size];

            for (int i = 0; i < size; i++)
                output[i] = (byte)(sbyte)(data[i] ^ (i % 128));

            return output;
        }
    }
}