using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using PhilLibX;
using PhilLibX.IO;

namespace CoDLuaExporter
{
    public class LuaExporter
    {
        public static void ReadLuaFiles( Process process, string handler, string currentGame, string gameName, long assetPoolsAddress )
        {
            // Get pool index
            int poolIndex = GameDefinition.Games[currentGame].LuaPoolIndex;

            // Get addresses
            long luaPoolAddress = assetPoolsAddress + ( Marshal.SizeOf<CordycepXAssetPool64>() * poolIndex );
            long firstXAsset = MemoryUtil.ReadStruct<CordycepXAssetPool64>( process.Handle, luaPoolAddress ).FirstXAsset;

#if DEBUG
            // Print debug info
            long luaStructAddress = MemoryUtil.ReadInt64( process.Handle, MemoryUtil.ReadInt64( process.Handle, firstXAsset + 0x10 ) );
            Printer.WriteLine( "INIT", $"LuaStructAddress: {luaStructAddress:X}" );
            Printer.WriteLine( "INIT", "" );
#endif

            // Loop lua structs
            for( var current = MemoryUtil.ReadStruct<CordycepXAsset64>( process.Handle, firstXAsset ); ; current = MemoryUtil.ReadStruct<CordycepXAsset64>( process.Handle, current.Next ) )
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

                // Read the lua file struct
                Type luaStruct = GameDefinition.Games[currentGame].LuaFileStruct;
                dynamic luaFile = Util.ReadStructByType( process.Handle, current.Header, luaStruct );
                string Name = MemoryUtil.ReadNullTerminatedString( process.Handle, luaFile.NamePointer );

                // Skip other raw files for Black Ops 3
                if( gameName == "Black Ops 3" || gameName == "Black Ops 2" && Path.GetExtension( Name ) != ".lua")
                {
                    continue;
                }

                // Read the file data
                byte[] luaFileData = Util.GetFileDataDecompressed( MemoryUtil.ReadBytes( process.Handle, luaFile.RawDataPtr, luaFile.AssetSize ) );

                // Make sure the file isn't empty
                if( luaFileData.Length <= 1 )
                {
                    continue;
                }

                // If it's hashed, set the name as the hash
                if( String.IsNullOrEmpty( Name ) )
                {
                    Name = $"0x{luaFile.NamePointer:x}.lua";
                }

                // Use compiled extension
                Name = Path.ChangeExtension( Name, ".luac" );

                // Set export path
                string appDir       =   AppDomain.CurrentDomain.BaseDirectory;
                string exportRoot   =   Path.Combine( appDir, "exported_files" );
                string gameFolder   =   Path.Combine( exportRoot, gameName );
                string assetFolder  =   Path.Combine( gameFolder, "Lua" );
                string outputPath   =   Path.Combine( assetFolder, Name );
                string outputDir    =   Path.GetDirectoryName( outputPath );

                // Print lua file name
                Printer.WriteLine( "EXPORT", Name );

#if DEBUG
                // Print lua file struct info
                Printer.WriteLine( "EXPORT", $"NamePointer: \t\t\t{luaFile.NamePointer:X}" );
                Printer.WriteLine( "EXPORT", $"AssetSize: \t\t\t{luaFile.AssetSize:X}" );
                Printer.WriteLine( "EXPORT", $"RawDataPtr: \t\t\t{luaFile.RawDataPtr:X}" );
                Printer.WriteLine( "EXPORT", $"Output path: \t\t\t{outputPath}" );
                Printer.WriteLine( "EXPORT", $"" );
#endif

                // Write data to file
                Directory.CreateDirectory( outputDir );
                File.WriteAllBytes( outputPath, luaFileData );

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
