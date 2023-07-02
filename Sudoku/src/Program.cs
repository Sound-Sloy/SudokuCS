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

            Globals.MainMenuFont = Raylib.LoadFont(Globals.ResPack.Fonts["ubuntu-regular.ttf"]);

            MainMenu mainMenu = new();

            Board board = new();

            bool gameInProgress = false;

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                if (!gameInProgress)
                {
                    mainMenu.Draw();
                }

                if (mainMenu.ShouldStartGame && !gameInProgress)
                {
                    gameInProgress = true;
                    board.Dispose();
                    Globals.SudokuGenerator.Generate((int)mainMenu.SelectedDifficulty);
                    board = new Board();
                }
                
                if(gameInProgress)
                {
                    board.Draw();
                }

                //board.Draw();
                //textb.Update();
                //textb.Draw();

                Raylib.EndDrawing();
            }
        }
    }
}