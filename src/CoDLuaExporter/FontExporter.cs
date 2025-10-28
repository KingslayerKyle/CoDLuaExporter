using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using PhilLibX;
using PhilLibX.IO;

namespace CoDLuaExporter
{
    public class FontExporter
    {
        public static void ReadFontFiles( Process process, string handler, string currentGame, string gameName, long assetPoolsAddress )
        {
            // Get pool index
            int poolIndex = GameDefinition.Games[currentGame].FontPoolIndex;

            // Don't run font exporter on games that don't have traditional font formats
            if( poolIndex == -1 )
            {
                Printer.WriteLine( "INFO", "" );
                Printer.WriteLine( "INFO", "This game doesn't use traditional font formats, skipping font export." );
                return;
            }

            // Get addresses
            long fontPoolAddress = assetPoolsAddress + ( Marshal.SizeOf<CordycepXAssetPool64>() * poolIndex );
            long firstXAssetFont = MemoryUtil.ReadStruct<CordycepXAssetPool64>( process.Handle, fontPoolAddress ).FirstXAsset;

#if DEBUG
            // Print debug 
            long fontStructAddress = MemoryUtil.ReadInt64( process.Handle, MemoryUtil.ReadInt64( process.Handle, firstXAssetFont + 0x10 ) );
            Printer.WriteLine( "INIT", $"FontStructAddress: {fontStructAddress:X}" );
            Printer.WriteLine( "INIT", "" );
#endif

            // Loop font structs
            for( var current = MemoryUtil.ReadStruct<CordycepXAsset64>( process.Handle, firstXAssetFont ); ; current = MemoryUtil.ReadStruct<CordycepXAsset64>( process.Handle, current.Next ) )
            {
                // Skip invalid header
                if( current.Header == 0 )
                {
                    if( current.Next == 0 )
                    {
                        break;
                    }

                    continue;
                }

                // Read the font file struct
                Type fontStruct = GameDefinition.Games[currentGame].FontFileStruct;
                dynamic fontFile = Util.ReadStructByType( process.Handle, current.Header, fontStruct );
                string Name = MemoryUtil.ReadNullTerminatedString( process.Handle, fontFile.NamePointer );

                // Read the file data
                byte[] fontFileData = Util.GetFileDataDecompressed( MemoryUtil.ReadBytes( process.Handle, fontFile.RawDataPtr, fontFile.AssetSize ) );

                // Make sure the file isn't empty
                if( fontFileData.Length <= 1 )
                {
                    continue;
                }

                // If it's hashed, set the name as the hash
                if( String.IsNullOrEmpty( Name ) )
                {
                    Name = $"0x{fontFile.NamePointer:x}{Util.GetFontExtension( fontFileData )}";
                }

                // Set export path
                string appDir       =   AppDomain.CurrentDomain.BaseDirectory;
                string exportRoot   =   Path.Combine( appDir, "exported_files" );
                string gameFolder   =   Path.Combine( exportRoot, gameName );
                string assetFolder  =   Path.Combine( gameFolder, "Font" );
                string outputPath   =   Path.Combine( assetFolder, Name );
                string outputDir    =   Path.GetDirectoryName( outputPath );

                // Print font file name
                Printer.WriteLine( "EXPORT", Name );

#if DEBUG
                // Print font file struct info
                Printer.WriteLine( "EXPORT", $"NamePointer: \t\t\t{fontFile.NamePointer:X}" );
                Printer.WriteLine( "EXPORT", $"AssetSize: \t\t\t{fontFile.AssetSize:X}" );
                Printer.WriteLine( "EXPORT", $"RawDataPtr: \t\t\t{fontFile.RawDataPtr:X}" );
                Printer.WriteLine( "EXPORT", $"Output path: \t\t\t{outputPath}" );
                Printer.WriteLine( "EXPORT", $"" );
#endif

                // Write data to file
                Directory.CreateDirectory( outputDir );
                File.WriteAllBytes( outputPath, fontFileData );

                // Stop if this is the last file
                if( current.Next == 0 )
                {
                    break;
                }

                // Testing
                //Console.ReadKey();
            }
        }
    }
}
