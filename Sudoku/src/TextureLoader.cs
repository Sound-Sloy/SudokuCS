using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class TextureLoader
{
    public static Texture2D LoadTexture(string path)
    {

        Texture2D texture = Raylib.LoadTexture(path);
        if (texture.width != 0)
        {
            return texture;
        }

        TextObject text = new TextObject("Unable to locate '" + path + "'", Raylib.GetFontDefault(), 16, 1, Color.DARKGRAY);
        Raylib.InitWindow((int)text.Measurements.X + 20, 30, "ERROR");
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RAYWHITE);

            text.Draw(10, 5);

            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();

        Environment.Exit(1);
        return new Texture2D();
    }
}
