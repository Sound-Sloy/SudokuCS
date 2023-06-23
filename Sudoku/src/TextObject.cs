using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;


public class TextObject
{

    private string m_Text = "";
    private Font m_Font;
    private int m_FontSize;
    private float m_FontSpacing;
    private Color m_Color;

    public TextObject(string text, Font font, int fontSize = 20, float fontSpacing = 1f, Color? color = null)
    {
        m_Text = text;
        m_FontSize = fontSize;
        m_FontSpacing = fontSpacing;
        if (color == null)
        {
            m_Color = Color.BLACK;
        }
        else
        {
            m_Color = (Color)color;
        }
        m_Font = font;
    }

    public TextObject(string text, ref Font font, int fontSize = 20, float fontSpacing = 1f, Color? color = null)
    {
        m_Text = text;
        m_FontSize = fontSize;
        m_FontSpacing = fontSpacing;
        if (color == null)
        {
            m_Color = Color.BLACK;
        }
        else
        {
            m_Color = (Color)color;
        }
        m_Font = font;
    }

    public TextObject(string text, ref Dictionary<int, Font> fontList, int fontSize = 20, float fontSpacing = 1f, Color? color = null)
    {
        m_Text = text;
        m_FontSize = fontSize;
        m_FontSpacing = fontSpacing;
        if (color == null)
        {
            m_Color = Color.BLACK;
        }

        m_Font = fontList[fontSize];
    }

    public Vector2 Measurements => Raylib.MeasureTextEx(m_Font, m_Text, m_FontSize, m_FontSpacing);

    public void Draw(Vector2 pos)
    {
        //__TODO__
        //Raylib.DrawTextPro(m_Font, m_Text, pos, m_OriginPoint, m_Rotation, m_FontSize, m_FontSpacing, m_Color);

        Raylib.DrawTextEx(m_Font, m_Text, pos, m_FontSize, m_FontSpacing, m_Color);
    }

    public void Draw(int posX, int posY)
    {
        Raylib.DrawTextEx(m_Font, m_Text, new Vector2(posX, posY), m_FontSize, m_FontSpacing, m_Color);
    }

}
