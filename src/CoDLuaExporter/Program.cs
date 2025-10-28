using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PhilLibX;

namespace CoDLuaExporter
{
    internal class Program
    {
        static void Main( string[] args )
        {
            // Init
            Printer.WriteLine( "INIT", $"──────────────────────────────────────────────" );
            Printer.WriteLine( "INIT", $"CoDLuaExporter {Assembly.GetExecutingAssembly().GetName().Version} by Kingslayer Kyle." );
            Printer.WriteLine( "INIT", $"\t- Exports lua and font files from CoD games." );
            Printer.WriteLine( "INIT", $"" );

            // Resources
            Printer.WriteLine( "INIT", $"Get Cordycep here:" );
            Printer.WriteLine( "INIT", $"\t- https://discord.gg/eY2Y5p2PEp" );
            Printer.WriteLine( "INIT", $"" );
            Printer.WriteLine( "INIT", $"Decompiler for old games:" );
            Printer.WriteLine( "INIT", $"\t- https://github.com/JariKCoding/CoDLuaDecompiler" );
            Printer.WriteLine( "INIT", $"" );
            Printer.WriteLine( "INIT", $"Decompiler for new games:" );
            Printer.WriteLine( "INIT", $"\t- https://github.com/marsinator358/luajit-decompiler-v2/tree/Call-of-Duty" );
            Printer.WriteLine( "INIT", $"" );

            // Notes
            Printer.WriteLine( "INIT", $"Important:" );
            Printer.WriteLine( "INIT", $"\t- Cordycep is required to export files." );
            Printer.WriteLine( "INIT", $"\t- Make sure the game is loaded with Cordycep." );
            Printer.WriteLine( "INIT", $"" );

            // Supported games
            Printer.WriteLine( "INIT", $"Supported games:" );
            Printer.WriteLine( "INIT", $"\t- GHO, AW, BO3, IW, MWR, WW2, BO4, MW19, CW, VG, MW22, MW23, BO6" );
            Printer.WriteLine( "INIT", $"──────────────────────────────────────────────" );
            Printer.WriteLine( "INIT", $"" );

            // Get Cordycep process and handler
            Process process = Process.GetProcessesByName( "Cordycep.CLI" ).FirstOrDefault();
            string handler = Util.GetHandler( process );

            // Make sure it exists
            if( process == null || handler == null )
            {
                Printer.WriteLine( "ERROR", "Cordycep is not running. Please start it and try again.", ConsoleColor.DarkRed );
                return;
            }

            // Current game we're working with
            string currentGame = Encoding.ASCII.GetString( File.ReadAllBytes( handler ), 0, 8 );
            string gameName = GameDefinition.Games[currentGame].Name;
            Printer.WriteLine( "INIT", $"Found handler: {gameName}" );

            // Get assetpools address
            long assetPoolsAddress = BitConverter.ToInt64( File.ReadAllBytes( handler ), 8 );

            // Wait for user input
            Printer.WriteLine( "INIT", "" );
            Printer.WriteLine( "INIT", "Press any key to export..." );
            Printer.WriteLine( "INIT", "" );
            Console.ReadKey();

            // Read files
            LuaExporter.ReadLuaFiles( process, handler, currentGame, gameName, assetPoolsAddress );
            FontExporter.ReadFontFiles( process, handler, currentGame, gameName, assetPoolsAddress );

            // Done
            Printer.WriteLine( "DONE", "" );
            Printer.WriteLine( "DONE", "Press enter to exit..." );
            Console.ReadLine();
        }
    }
}
