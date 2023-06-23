global using Raylib_cs;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Sudoku.src
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Raylib.InitWindow(900, 900, "Sudoku");
            Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()));
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);

            Globals.ResourceLoader.Load("default");

            Globals.SudokuGenerator.Generate();

            var b = new Button("dsf", 10, 10, 100, 100);
            var tb = new TextureButton(Globals.ResPack.Textures["cell_normal.png"], 100, 100, 0, 0);

            Board board = new();

            Textbox textb = new(new Vector2(100, 100), new Vector2(250, 30));

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                //b.Draw();
                //tb.Draw();
                board.Draw();
                //textb.Update();
                //textb.Draw();

                Raylib.EndDrawing();
            }
        }
    }
}