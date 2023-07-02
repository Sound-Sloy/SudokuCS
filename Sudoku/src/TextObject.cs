using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;


public class TextObject
{
    private Font Font { get; set; }
    private int FontSize { get; set; }
    private float FontSpacing { get; set; }
    public string Text { get; set; }
    public Color Color { get; set; }
    public Vector2 Measurements => Raylib.MeasureTextEx(Font, Text, FontSize, FontSpacing);

    public TextObject(string text, Font font, int fontSize = 20, float fontSpacing = 1f, Color? color = null)
    {
        Text = text;
        FontSize = fontSize;
        FontSpacing = fontSpacing;
        if (color == null)
        {
            Color = Color.BLACK;
        }
        else
        {
            Color = (Color)color;
        }
        Font = font;
    }

    public TextObject(string text, ref Font font, int fontSize = 20, float fontSpacing = 1f, Color? color = null) : this(text, font, fontSize, fontSpacing, color)
    {

    }

    /*public TextObject(string text, ref Font font, int fontSize = 20, float fontSpacing = 1f, Color? color = null)
    {
        Text = text;
        m_FontSize = fontSize;
        m_FontSpacing = fontSpacing;
        if (color == null)
        {
            Color = Color.BLACK;
        }
        else
        {
            Color = (Color)color;
        }
        m_Font = font;
    }*/

    public TextObject(string text, ref Dictionary<int, Font> fontList, int fontSize = 20, float fontSpacing = 1f, Color? color = null) : this(text, fontList[fontSize], fontSize, fontSpacing, color)
    {

    }

    /*public TextObject(string text, ref Dictionary<int, Font> fontList, int fontSize = 20, float fontSpacing = 1f, Color? color = null)
    {
        Text = text;
        m_FontSize = fontSize;
        m_FontSpacing = fontSpacing;
        if (color == null)
        {
            Color = Color.BLACK;
        }
        else
        {
            Color = (Color)color;
        }

        m_Font = fontList[fontSize];
    }*/

    public void Draw(Vector2 pos)
    {
        //__TODO__
        //Raylib.DrawTextPro(m_Font, m_Text, pos, m_OriginPoint, m_Rotation, m_FontSize, m_FontSpacing, m_Color);

        Raylib.DrawTextEx(Font, Text, pos, FontSize, FontSpacing, Color);
    }

    public void Draw(int posX, int posY)
    {
        Raylib.DrawTextEx(Font, Text, new Vector2(posX, posY), FontSize, FontSpacing, Color);
    }

}
