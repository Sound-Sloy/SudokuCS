using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using Resources;

public static class Globals
{
    public static Resources.Resources ResourceLoader = new();
    public static Resources.ResourcePack ResPack = ResourceLoader.Get;
    public static SudokuLib SudokuGenerator = new SudokuLib();
    public static TileStyle TileStyle = new TileStyle();
}
