using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using PhilLibX.IO;

namespace CoDLuaExporter
{
    public class Util
    {
        public static string GetHandler( Process process )
        {
            if( process == null )
            {
                return null;
            }

            string processDir = Path.GetDirectoryName( process.MainModule?.FileName );
            string handlerDir = Path.Combine( processDir, "Data" );
            string handler = Path.Combine( handlerDir, "CurrentHandler.csi" );

            if( !File.Exists( handler ) )
            {
                return null;
            }

            return handler;
        }
        public static object ReadStructByType( IntPtr processHandle, long address, Type type )
        {
            byte[] data = MemoryUtil.ReadBytes( processHandle, address, Marshal.SizeOf( type ) );
            GCHandle handle = GCHandle.Alloc( data, GCHandleType.Pinned );
            object result = Marshal.PtrToStructure( handle.AddrOfPinnedObject(), type );
            handle.Free();
            return result;
        }
        public static bool IsZlibCompressed( byte[] data )
        {
            return data.Length > 2 && data[0] == 0x78 && ( data[1] == 0xDA || data[1] == 0x9C );
        }
        public static byte[] ZlibDecompress( byte[] compressedData )
        {
            if( compressedData.Length < 2 )
            {
                return Array.Empty<byte>();
            }

            // Skip the first two bytes (78 DA or other zlib headers)
            byte[] rawDeflateData = new byte[compressedData.Length - 2];
            Array.Copy( compressedData, 2, rawDeflateData, 0, rawDeflateData.Length );

            using( MemoryStream input = new MemoryStream( rawDeflateData ) )
            using( MemoryStream output = new MemoryStream() )
            {
                using( DeflateStream decompressor = new DeflateStream( input, CompressionMode.Decompress ) )
                {
                    decompressor.CopyTo( output );
                }

                return output.ToArray();
            }
        }
        public static byte[] GetFileDataDecompressed( byte[] fileData )
        {
            if( fileData == null || fileData.Length == 0 )
            {
                return Array.Empty<byte>();
            }

            if( !IsZlibCompressed( fileData ) )
            {
                return fileData;
            }

            // Decompress file
            try
            {
                byte[] decompressed = ZlibDecompress( fileData );
                return decompressed;
            }
            catch( Exception )
            {
                return Array.Empty<byte>();
            }
        }
        public static string GetFontExtension( byte[] fileData )
        {
            if( fileData == null || fileData.Length < 4 )
            {
                return ".unknown";
            }

            uint signature = (uint) ( ( fileData[0] << 24 ) | ( fileData[1] << 16 ) | ( fileData[2] << 8 ) | fileData[3] );

            switch( signature )
            {
                case 0x00010000:
                    return ".ttf";

                case 0x4F54544F:
                    return ".otf";

                case 0x774F4646:
                    return ".woff";

                case 0x774F4632:
                    return ".woff2";

                case 0x74746366:
                    return ".ttc";

                default:
                    return ".unknown";
            }
        }
    }
}
