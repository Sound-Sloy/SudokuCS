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
        private string m_Symbol;
        private float m_FontSize;
        private bool m_bIsSelected;
        private Font m_Font = Raylib.GetFontDefault();

        public Glyph(string symbol, Font font, float size, bool isSelected = false)
        {
            m_Symbol = symbol;
            m_FontSize = size;
            m_bIsSelected = isSelected;
            m_Font = font;
        }

        public Vector2 Size => Raylib.MeasureTextEx(m_Font, m_Symbol, m_FontSize, 1f);

        public string Symbol
        {
            get => m_Symbol;
            set => m_Symbol = value;
        }

        public float FontSize
        {
            get => m_FontSize;
            set => m_FontSize = value;
        }
        
        public bool Selected
        {
            get => m_bIsSelected;
            set => m_bIsSelected = value;
        }

        public Font Font
        {
            get => m_Font;
            set => m_Font = value;
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

    private Vector2 m_Pos;
    private int m_TextPos = 0;
    private Vector2 m_Size;
    private List<Glyph> m_Glyphs = new();
    private int m_CursorPos = 0;
    private RenderTexture2D m_Canvas;
    private bool m_IsFocused = false;
    private Configuration m_Properties;
    private Vector2 m_Offset = new Vector2(2, 2);

    private bool m_KeyShouldRepeat = false;
    private float m_KeyRepeatDelay = 0.05f; // Delay in seconds between repeated actions
    private float m_KeyRepeatTimer = 0f;

    private bool m_bShouldDrawCursor = false;
    private float m_CursorBlinkTimer = 0f;

    public Configuration Properties
    {
        get => m_Properties;
        set => m_Properties = value;
    }

    public Vector2 Pos
    {
        get => m_Pos;
        set => m_Pos = value;
    }

    public bool Focus => m_IsFocused;

    private bool IsHovered => Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(m_Pos.X, m_Pos.Y, m_Size.X, m_Size.Y));
    private bool IsClicked => IsHovered && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
    private bool IsClickedOutside => !IsHovered && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);

    public string Text => GetText();

    public Textbox(Vector2 pos, Vector2 size, Configuration? properties = null)
    {
        m_Pos = pos;
        m_Size = size;
        if(properties != null)
        {
            m_Properties = properties;
        }
        else
        {
            m_Properties = new Configuration();
        }

        m_TextPos = m_Properties.OutlineWidth + (int)m_Offset.X;
        m_Canvas = Raylib.LoadRenderTexture((int)size.X, (int)size.Y);
    }

    public Textbox(int posX, int posY, int sizeX, int sizeY, Configuration? properties = null) 
        : this(new Vector2(posX, posY), new Vector2(sizeX, sizeY), properties)
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
        m_IsFocused = false;
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
            m_IsFocused = !m_IsFocused;
        }
        if (m_IsFocused)
        {
            if (Keyboard.GetKeyPressed() == KeyboardKey.KEY_ESCAPE || IsClickedOutside)
            {
                m_IsFocused = false;
            }
        }

        for(int i = 0; i < m_Glyphs.Count(); i++)
        {
            m_Glyphs[i].Font = Properties.Font;
            m_Glyphs[i].FontSize = Properties.FontSize;
        }

        if(!m_IsFocused)
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


        int pressedKey = Raylib.GetKeyPressed();

        /*if (Raylib.IsKeyDown((KeyboardKey)pressedKey))
        {
            Console.WriteLine(Raylib.GetKeyPressed().ToString() + " " + Raylib.GetTime().ToString() + "LEFT");
        }*/


        switch ((KeyboardKey)pressedKey)
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
            if (m_TextPos + MeasureText(GetText().Substring(0, m_CursorPos)).X < Properties.OutlineWidth + m_Offset.X + m_Glyphs[m_CursorPos > 0 ? m_CursorPos - 1 : 0].Size.X)
            {
                int a = (int)MeasureText(GetText().Substring(0, m_CursorPos > 0 ? m_CursorPos : 0)).X + m_TextPos;
                m_TextPos += (int)m_Glyphs[m_CursorPos > 0 ? m_CursorPos - 1 : 0].Size.X - a;
            }
            else if (m_TextPos + MeasureText(GetText().Substring(0, m_CursorPos)).X > m_Size.X - Properties.OutlineWidth - m_Offset.X - m_Glyphs[m_CursorPos > 0 ? m_CursorPos - 1 : 0].Size.X) 
            {
                int a = (int)MeasureText(GetText().Substring(0, m_CursorPos > 0 ? m_CursorPos : 0)).X - Math.Abs(m_TextPos) - (int)m_Size.X;
                m_TextPos -= a + Properties.OutlineWidth + (int)m_Offset.X;
            }
        }
        if(m_TextPos > Properties.OutlineWidth + m_Offset.X)
        {
            m_TextPos = Properties.OutlineWidth + (int)m_Offset.X;
        }

    }

    private string GetText()
    {
        string text = "";
        foreach (var glyph in m_Glyphs)
        {
            text+=glyph.Symbol;
        }
        return text;
    }

    private Vector2 TextMeasurements => Raylib.MeasureTextEx(Properties.Font, GetText(), Properties.FontSize, Properties.FontSpacing);

    private Vector2 MeasureText(string text)
    {
        return Raylib.MeasureTextEx(Properties.Font, text, Properties.FontSize, Properties.FontSpacing);
    }

    private void DrawCursor()
    {
        if (!m_IsFocused)
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
            Raylib.DrawRectangleV(new Vector2(m_TextPos + (int)MeasureText(GetText().Substring(0, m_CursorPos > 0 ? m_CursorPos : 0)).X, (m_Size.Y - Properties.FontSize) / 2), new Vector2(2, Properties.FontSize), Properties.CursorColor);
        }
    }

    public void Draw()
    {
        Raylib.BeginTextureMode(m_Canvas);

        //Raylib.ClearBackground(Color.BLACK);
        //Console.WriteLine("here");
        Raylib.DrawRectangleRounded(new Rectangle(0, 0, m_Size.X, m_Size.Y), Properties.OutlineRoundness, 0, Properties.BackgroundColor);
        Raylib.DrawRectangleRoundedLines(new Rectangle(Properties.OutlineWidth, Properties.OutlineWidth, m_Size.X - 2 * Properties.OutlineWidth, m_Size.Y - 2 * Properties.OutlineWidth), Properties.OutlineRoundness, 10, Properties.OutlineWidth, Properties.OutlineColor);

        Raylib.DrawTextEx(Properties.Font, GetText(), new Vector2(m_TextPos, (m_Size.Y - Properties.FontSize) / 2), Properties.FontSize, Properties.FontSpacing, Properties.TextColor);

        DrawCursor();
        
        Raylib.EndTextureMode();

        
        Raylib.DrawTexturePro(m_Canvas.texture, new Rectangle(0, 0, m_Size.X, -m_Size.Y), new Rectangle(m_Pos.X, m_Pos.Y, m_Size.X, m_Size.Y), new Vector2(0, 0), 0f, Color.WHITE);

    }

    public void UpdateAndDraw()
    {
        Update();
        Draw();
    }

    
}
