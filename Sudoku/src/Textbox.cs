using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;


public class Textbox
{
    private class Glyph
    {
        public string Symbol { get; set; }
        public float FontSize { get; set; }
        public bool Selected { get; set; }
        public Font Font { get; set; } = Raylib.GetFontDefault();

        public Vector2 Size => Raylib.MeasureTextEx(Font, Symbol, FontSize, 1f);

        public Glyph(string symbol, Font font, float size, bool isSelected = false)
        {
            Symbol = symbol;
            FontSize = size;
            Selected = isSelected;
            Font = font;
        }
    }

    public class Configuration
    {
        public string PlaceholderText;
        public Font Font;
        public float FontSize;
        public float FontSpacing;
        public Color BackgroundColor;
        public Color TextColor;
        public Color OutlineColor;
        public Color CursorColor;
        public Color PlaceholderColor;
        public int OutlineWidth;
        public float OutlineRoundness;
        public float CursorBlinkSpeed;

        public Configuration()
        {
            PlaceholderText = "";
            Font = Raylib.GetFontDefault();
            FontSize = 20f;
            FontSpacing = 1f;
            BackgroundColor = Color.LIGHTGRAY;
            TextColor = Color.DARKGRAY;
            OutlineColor = Color.BLACK;
            CursorColor = Color.RED;
            PlaceholderColor = Color.GRAY;
            OutlineWidth = 2;
            OutlineRoundness = 0f;
            CursorBlinkSpeed = 4f;
        }
    }

    public Vector2 Pos { get; private set; }
    private int m_TextPos = 0;
    public Vector2 Size { get; private set; }
    private List<Glyph> m_Glyphs = new();
    private int m_CursorPos = 0;
    private BetterRenderTexture m_Canvas;
    private BetterRenderTexture Parent { get; }
    public Configuration Properties { get; set; }
    private Vector2 m_Offset = new Vector2(2, 2);

    private bool m_KeyShouldRepeat = false;
    private float m_KeyRepeatDelay = 0.05f; // Delay in seconds between repeated actions
    private float m_KeyRepeatTimer = 0f;

    private bool m_bShouldDrawCursor = false;
    private float m_CursorBlinkTimer = 0f;

    public bool Focus { get; private set; } = false;

    //private Vector2 MousePos => Parent.MousePos;

    private bool IsHovered => Raylib.CheckCollisionPointRec(m_Canvas.MousePos, new Rectangle(0, 0, Size.X, Size.Y));
    private bool IsClicked => IsHovered && Mouse.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
    private bool IsClickedOutside => !IsHovered && Mouse.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);

    public string Text{
        get {
            string text = "";
            foreach (var glyph in m_Glyphs)
            {
                text += glyph.Symbol;
            }
            return text;
        }
    }

    /*
     private string GetText()
    {
        string text = "";
        foreach (var glyph in m_Glyphs)
        {
            text+=glyph.Symbol;
        }
        return text;
    }
     */

    public Textbox(ref BetterRenderTexture parent, Vector2 size, Configuration? properties = null)
    {
        m_Canvas = new BetterRenderTexture((int)size.X, (int)size.Y, parent.GUID, Content);
        Parent = parent;
        Size = size;
        Properties = properties != null ? properties : new Configuration();

        m_TextPos = Properties.OutlineWidth + (int)m_Offset.X;
    }

    public Textbox(ref BetterRenderTexture parent, int sizeX, int sizeY, Configuration? properties = null) 
        : this(ref parent, new Vector2(sizeX, sizeY), properties)
    {

    }

    private void CheckCursorPos()
    {
        if (m_CursorPos < 0)
        {
            m_CursorPos = 0;
        }
    }

    private void HandleBackspace()
    {
        if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL))
        {

        }
        else
        {
            if (m_Glyphs.Any() && m_CursorPos > 0)
            {
                m_CursorPos--;
                m_Glyphs.RemoveAt(m_CursorPos);
            }
        }
    }

    private void HandleDelete()
    {
        if (m_CursorPos < m_Glyphs.Count())
        {
            m_Glyphs.RemoveAt(m_CursorPos);
        }
    }

    private void HandleEnter()
    {
        Focus = false;
    }

    private void HandleLeft()
    {
        if(m_CursorPos > 0)
        {
            m_CursorPos--;
        }
    }

    private void HandleRight()
    {
        if (m_CursorPos < m_Glyphs.Count())
        {
            m_CursorPos++;
        }
    }

    public void Update()
    {
        if (IsClicked)
        {
            Focus = !Focus;
        }
        if (Focus)
        {
            if (Keyboard.GetKeyPressed() == KeyboardKey.KEY_ESCAPE || IsClickedOutside)
            {
                Focus = false;
            }
        }

        for(int i = 0; i < m_Glyphs.Count(); i++)
        {
            m_Glyphs[i].Font = Properties.Font;
            m_Glyphs[i].FontSize = Properties.FontSize;
        }

        if(!Focus)
        {
            return;
        }


        int pressedChar = Raylib.GetCharPressed();
        if (pressedChar != 0)
        {
            m_Glyphs.Insert(m_CursorPos, new Glyph(Convert.ToChar(pressedChar).ToString(), Properties.Font, Properties.FontSize));
            m_CursorPos++;
            CheckCursorPos();
        }


        //                                                              Handle Special Keys 


        KeyboardKey pressedKey = Keyboard.GetKeyPressed();

        switch (pressedKey)
        {
            case KeyboardKey.KEY_ENTER or KeyboardKey.KEY_KP_ENTER:
                HandleEnter();
                m_KeyRepeatTimer = -0.5f;
                break;

            case KeyboardKey.KEY_BACKSPACE:
                HandleBackspace();
                m_KeyRepeatTimer = -0.5f;
                break;

            case KeyboardKey.KEY_LEFT:
                HandleLeft();
                m_KeyRepeatTimer = -0.5f;
                break;

            case KeyboardKey.KEY_RIGHT:
                HandleRight();
                m_KeyRepeatTimer = -0.5f;
                break;

            case KeyboardKey.KEY_DELETE:
                HandleDelete();
                m_KeyRepeatTimer = -0.5f;
                break;

        }

        m_KeyRepeatTimer += Raylib.GetFrameTime();
        if (m_KeyRepeatTimer >= m_KeyRepeatDelay)
        {
            KeyboardKey heldKey = Keyboard.GetKeyDown();

            switch (heldKey)
            {
                case KeyboardKey.KEY_ENTER or KeyboardKey.KEY_KP_ENTER:
                    HandleEnter();
                    break;

                case KeyboardKey.KEY_BACKSPACE:
                    HandleBackspace();
                    break;

                case KeyboardKey.KEY_LEFT:
                    HandleLeft();
                    break;

                case KeyboardKey.KEY_RIGHT:
                    HandleRight();
                    break;

                case KeyboardKey.KEY_DELETE:
                    HandleDelete();
                    break;

            }

            m_KeyRepeatTimer = 0f;

        }

        if (m_Glyphs.Any())
        {
            if (m_TextPos + MeasureText(Text.Substring(0, m_CursorPos)).X < Properties.OutlineWidth + m_Offset.X + m_Glyphs[m_CursorPos > 0 ? m_CursorPos - 1 : 0].Size.X)
            {
                int a = (int)MeasureText(Text.Substring(0, m_CursorPos > 0 ? m_CursorPos : 0)).X + m_TextPos;
                m_TextPos += (int)m_Glyphs[m_CursorPos > 0 ? m_CursorPos - 1 : 0].Size.X - a;
            }
            else if (m_TextPos + MeasureText(Text.Substring(0, m_CursorPos)).X > Size.X - Properties.OutlineWidth - m_Offset.X - m_Glyphs[m_CursorPos > 0 ? m_CursorPos - 1 : 0].Size.X) 
            {
                int a = (int)MeasureText(Text.Substring(0, m_CursorPos > 0 ? m_CursorPos : 0)).X - Math.Abs(m_TextPos) - (int)Size.X;
                m_TextPos -= a + Properties.OutlineWidth + (int)m_Offset.X;
            }
        }
        if (m_TextPos > Properties.OutlineWidth + m_Offset.X)
        {
            m_TextPos = Properties.OutlineWidth + (int)m_Offset.X;
        }

    }


    private Vector2 TextMeasurements => Raylib.MeasureTextEx(Properties.Font, Text, Properties.FontSize, Properties.FontSpacing);

    private Vector2 MeasureText(string text)
    {
        return Raylib.MeasureTextEx(Properties.Font, text, Properties.FontSize, Properties.FontSpacing);
    }

    private void DrawCursor()
    {
        if (!Focus)
        {
            return;
        }
        m_CursorBlinkTimer += Raylib.GetFrameTime();
        if(m_CursorBlinkTimer >= 1 / Properties.CursorBlinkSpeed)
        {
            m_CursorBlinkTimer = 0;
            m_bShouldDrawCursor = !m_bShouldDrawCursor;
        }

        if (m_bShouldDrawCursor)
        {
            Raylib.DrawRectangleV(new Vector2(m_TextPos + (int)MeasureText(Text.Substring(0, m_CursorPos > 0 ? m_CursorPos : 0)).X, (Size.Y - Properties.FontSize) / 2), new Vector2(2, Properties.FontSize), Properties.CursorColor);
        }
    }

    private void Content()
    {
        Raylib.ClearBackground(Color.DARKPURPLE);
        Raylib.DrawRectangleRounded(new Rectangle(0, 0, Size.X, Size.Y), Properties.OutlineRoundness, 0, Properties.BackgroundColor);
        Raylib.DrawRectangleRoundedLines(new Rectangle(Properties.OutlineWidth, Properties.OutlineWidth, Size.X - 2 * Properties.OutlineWidth, Size.Y - 2 * Properties.OutlineWidth), Properties.OutlineRoundness, 10, Properties.OutlineWidth, Properties.OutlineColor);

        Raylib.DrawTextEx(Properties.Font, Text, new Vector2(m_TextPos, (Size.Y - Properties.FontSize) / 2), Properties.FontSize, Properties.FontSpacing, Properties.TextColor);

        DrawCursor();
        TextObject textObject = new("helo", Raylib.GetFontDefault(), 20, 1, Color.SKYBLUE);
        textObject.Draw(0, 0);
    }

    public void Draw(Vector2 pos)
    {
        if(pos != Pos)
        {
            Pos = pos;
        }
        m_Canvas.Draw(pos);
        if (IsHovered)
        {
            Console.WriteLine($"hovered >> {Raylib.GetTime()}");
        }

    }

    public void UpdateAndDraw(Vector2 pos)
    {
        if (pos != Pos)
        {
            Pos = pos;
        }
        Update();
        Draw(pos);
    }

    
}
