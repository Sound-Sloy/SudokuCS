using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


public class Keyboard
{
    public static KeyboardKey GetKeyPressed()
    {
        foreach(KeyboardKey key in Enum.GetValues(typeof(KeyboardKey)))
        {
            if (Raylib.IsKeyPressed(key))
            {
                return key;
            }
        }
        return KeyboardKey.KEY_NULL;
    }

    public static KeyboardKey GetKeyDown()
    {
        foreach (KeyboardKey key in Enum.GetValues(typeof(KeyboardKey)))
        {
            if (Raylib.IsKeyDown(key))
            {
                return key;
            }
        }
        return KeyboardKey.KEY_NULL;
    }

    public static KeyboardKey GetKeyUp()
    {
        foreach (KeyboardKey key in Enum.GetValues(typeof(KeyboardKey)))
        {
            if (Raylib.IsKeyUp(key))
            {
                return key;
            }
        }
        return KeyboardKey.KEY_NULL;
    }
}

public class Mouse
{
    public static Vector2 GetMousePosition() => Raylib.GetMousePosition();

    public static int GetMouseX() => (int)GetMousePosition().X;

    public static int GetMouseY() => (int)GetMousePosition().Y;

    public static bool IsMouseButtonPressed(MouseButton button) => Raylib.IsMouseButtonPressed(button);
    public static bool IsMouseButtonReleased(MouseButton button) => Raylib.IsMouseButtonReleased(button);
    public static bool IsMouseButtonDown(MouseButton button) => Raylib.IsMouseButtonDown(button);
    public static bool IsMouseButtonUp(MouseButton button) => Raylib.IsMouseButtonUp(button);

    public static MouseButton? GetMouseButtonPressed()
    {
        foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
        {
            if (IsMouseButtonPressed(button))
            {
                return button;
            }
        }

        return null;
    }

    public static MouseButton? GetMouseButtonDown()
    {
        foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
        {
            if (IsMouseButtonDown(button))
            {
                return button;
            }
        }

        return null;
    }

    public static MouseButton? GetMouseButtonUp()
    {
        foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
        {
            if (IsMouseButtonUp(button))
            {
                return button;
            }
        }

        return null;
    }

}
