using System;
using System.Collections.Generic;

namespace CoDLuaExporter
{
    public struct CordycepXAssetPool64
    {
        public long     FirstXAsset;
        public long     LastXAsset;
        public long     LookupTable;
        public long     HeaderMemory;
        public long     Padding;
    }
    public struct CordycepXAsset64
    {
        public long     Header;
        public long     Temp;
        public long     Next;
        public long     Previous;
    }
    public struct LuaFileOld
    {
        public long     NamePointer;
        public int      AssetSize;
        public int      Unk1;
        public long     RawDataPtr;
    }
    public struct LuaFileBO4
    {
        public long     NamePointer;
        public long     Padding1;
        public int      AssetSize;
        public int      Unk1;
        public long     RawDataPtr;
    }
    public struct LuaFileBO6
    {
        public long     NamePointer;
        public long     Padding1;
        public long     Padding2;
        public long     Padding3;
        public long     Padding4;
        public long     Padding5;
        public long     Padding6;
        public int      Unk1;
        public int      AssetSize;
        public long     RawDataPtr;
    }
    public struct FontFileOld
    {
        public long     NamePointer;
        public int      AssetSize;
        public int      Unk1;
        public long     RawDataPtr;
    }
    public struct FontFileBO4
    {
        public long     NamePointer;
        public long     Padding1;
        public long     Unk1;
        public long     Padding2;
        public int      AssetSize;
        public int      Unk2;
        public long     RawDataPtr;
    }
    public struct FontFileCW
    {
        public long     NamePointer;
        public long     Unk1;
        public int      Unk2;
        public int      AssetSize;
        public long     RawDataPtr;
    }
    public struct FontFileMW22
    {
        public long     NamePointer;
        public long     Unk1;
        public int      AssetSize;
        public int      Unk2;
        public long     RawDataPtr;
    }
    public struct FontFileBO6
    {
        public long     NamePointer;
        public long     Unk1;
        public long     Unk2;
        public long     Padding1;
        public long     Padding2;
        public long     Padding3;
        public long     RawDataPtr;
        public long     Padding4;
        public int      AssetSize;
    }
    public readonly struct GameDefinition
    {
        public string Name
        {
            get;
        }
        public int LuaPoolIndex
        {
            get;
        }
        public int FontPoolIndex
        {
            get;
        }
        public Type LuaFileStruct
        {
            get;
        }
        public Type FontFileStruct
        {
            get;
        }
        public GameDefinition( string name, int luaPoolIndex, int fontPoolIndex, Type luaFileStruct, Type fontFileStruct )
        {
            Name = name;
            LuaPoolIndex = luaPoolIndex;
            FontPoolIndex = fontPoolIndex;
            LuaFileStruct = luaFileStruct;
            FontFileStruct = fontFileStruct;
        }
        public static readonly Dictionary<string, GameDefinition> Games = new Dictionary<string, GameDefinition>()
        {
            ["GHOSTS00"] = new GameDefinition(
                "Ghosts",
                54,
                -1,
                typeof( LuaFileOld ),
                typeof( FontFileOld )
            ),
            ["ADVANWAR"] = new GameDefinition(
                "Advanced Warfare",
                59,
                -1,
                typeof( LuaFileOld ),
                typeof( FontFileOld )
            ),
            ["BLKOPS03"] = new GameDefinition(
                "Black Ops 3",
                47,
                80,
                typeof( LuaFileOld ),
                typeof( FontFileOld )
            ),
            ["INFIWARF"] = new GameDefinition(
                "Infinite Warfare",
                59,
                66,
                typeof( LuaFileOld ),
                typeof( FontFileOld )
            ),
            ["REMAST00"] = new GameDefinition(
                "Modern Warfare Remastered",
                61,
                70,
                typeof( LuaFileOld ),
                typeof( FontFileOld )
            ),
            ["WWWWWWW2"] = new GameDefinition(
                "World War 2",
                69,
                78,
                typeof( LuaFileOld ),
                typeof( FontFileOld )
            ),
            ["BLKOPS04"] = new GameDefinition(
                "Black Ops 4",
                103,
                76,
                typeof( LuaFileBO4 ),
                typeof( FontFileBO4 )
            ),
            ["MODWAR19"] = new GameDefinition(
                "Modern Warfare 2019",
                62,
                69,
                typeof( LuaFileOld ),
                typeof( FontFileOld )
            ),
            ["BLKOPSCW"] = new GameDefinition(
                "Black Ops Cold War",
                124,
                101,
                typeof( LuaFileOld ),
                typeof( FontFileCW )
            ),
            ["VANGUARD"] = new GameDefinition(
                "Vanguard",
                64,
                74,
                typeof( LuaFileOld ),
                typeof( FontFileOld )
            ),
            ["MODWAR22"] = new GameDefinition(
                "Modern Warfare 2022",
                82,
                174,
                typeof( LuaFileOld ),
                typeof( FontFileMW22 )
            ),
            ["YAMYAMOK"] = new GameDefinition(
                "Modern Warfare 2023",
                76,
                170,
                typeof( LuaFileOld ),
                typeof( FontFileMW22 )
            ),
            ["BLACKOP6"] = new GameDefinition(
                "Black Ops 6",
                73,
                168,
                typeof( LuaFileBO6 ),
                typeof( FontFileBO6 )
            )
        };
    }
}
