using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
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
