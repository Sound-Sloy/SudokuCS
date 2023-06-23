using Raylib_cs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


public delegate void ButtonCallback();

public class Button : IDisposable
{
    public enum Alignment
    {
        TOPLEFT,
        TOPRIGHT,
        TOPCENTER,
        MIDLEFT,
        MIDRIGHT,
        MIDCENTER,
        BOTTOMLEFT,
        BOTTOMRIGHT,
        BOTTOMCENTER
    }

    public class Configuration
    {
        public Color Color_Background;
        public Color Color_Foreground;
        public Color Color_Outline;
        public Color Color_Hovered_Layer;
        public float Outline_Roundness;
        public int Outline_Width;
        public Font Font;
        public float Font_Size;
        public float Font_Spacing;
        public Alignment Alignment;

        public Configuration()
        {
            Color_Background = Color.LIGHTGRAY;
            Color_Foreground = Color.DARKGRAY;
            Color_Outline = Color.BLACK;
            Color_Hovered_Layer = new Color(130, 130, 130, 150);
            Outline_Roundness = 0f;
            Outline_Width = 2;
            Font = Raylib.GetFontDefault();
            Font_Size = 20f;
            Font_Spacing = 1f;
            Alignment = Alignment.MIDCENTER;
        }
    }

    public Configuration Properties = new();
    private string m_Text;
    private Vector2 m_Pos;
    private Vector2 m_Size;
    private RenderTexture2D m_Canvas;
    public event ButtonCallback? OnClick;

    public Button(string text, Vector2 pos, Vector2 size)
    {
        m_Text = text;
        m_Pos = pos;
        m_Size = size;
        m_Canvas = Raylib.LoadRenderTexture((int)size.X, (int)size.Y);
    }

    public Button(string text, int posX, int posY, int sizeX, int sizeY) : this(text, new Vector2(posX, posY), new Vector2(sizeX, sizeY))
    {
    }

    public Button() : this("Button", new Vector2(100,100), new Vector2(100, 50))
    {
    }

    ~Button()
    {
        //Dispose(true);
    }


    public void Draw()
    {
        Raylib.BeginTextureMode(m_Canvas);
        //Raylib.ClearBackground(Color.WHITE);

        TextObject text = new(m_Text, Properties.Font, (int)Properties.Font_Size, Properties.Font_Spacing, Properties.Color_Foreground);
        Vector2 text_pos = new();

        switch (Properties.Alignment)
        {
            case Alignment.TOPRIGHT or Alignment.TOPLEFT or Alignment.TOPCENTER:
                text_pos.Y = Properties.Outline_Width;
                break;
            case Alignment.MIDRIGHT or Alignment.MIDCENTER or Alignment.MIDLEFT:
                text_pos.Y = (m_Size.Y - text.Measurements.Y) / 2;
                break;
            case Alignment.BOTTOMRIGHT or Alignment.BOTTOMCENTER or Alignment.BOTTOMLEFT:
                text_pos.Y = m_Size.Y - Properties.Outline_Width - Properties.Outline_Width;
                break;
        }
        switch (Properties.Alignment)
        {
            case Alignment.BOTTOMRIGHT or Alignment.MIDRIGHT or Alignment.TOPRIGHT:
                text_pos.X = m_Size.X - text.Measurements.X - Properties.Outline_Width;
                break;
            case Alignment.BOTTOMCENTER or Alignment.MIDCENTER or Alignment.TOPCENTER:
                text_pos.X = m_Size.X / 2 - text.Measurements.X / 2;
                break;
            case Alignment.BOTTOMLEFT or Alignment.MIDLEFT or Alignment.TOPLEFT:
                text_pos.X = Properties.Outline_Width;
                break;
        }

        Raylib.DrawRectangleRounded(new Rectangle(0, 0, m_Size.X, m_Size.Y), Properties.Outline_Roundness, 0, Properties.Color_Background);
        Raylib.DrawRectangleRoundedLines(new Rectangle(Properties.Outline_Width, Properties.Outline_Width, m_Size.X - 2 * Properties.Outline_Width, m_Size.Y - 2 * Properties.Outline_Width), Properties.Outline_Roundness, 10, Properties.Outline_Width, Properties.Color_Outline);

        if (IsHovered)
        {
            Raylib.DrawRectangleRounded(new Rectangle(0, 0, m_Size.X, m_Size.Y), Properties.Outline_Roundness, 0, Properties.Color_Hovered_Layer);
        }
        if (IsClicked)
        {
            OnClick?.Invoke();
        }

        text.Draw(text_pos);

        Raylib.EndTextureMode();

        Raylib.DrawTexturePro(m_Canvas.texture, new Rectangle(0, 0, m_Size.X, -m_Size.Y), new Rectangle(m_Pos.X, m_Pos.Y, m_Size.X, m_Size.Y), new Vector2(0, 0), 0f, Color.WHITE);
    }


    bool IsHovered => Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(m_Pos.X, m_Pos.Y, m_Size.X, m_Size.Y));
    bool IsClicked => IsHovered && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);



    private bool m_bDisposed = false;

    public void Dispose()
    {
        Dispose(disposing: true);

        GC.Collect();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!m_bDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources.

            }

            Raylib.UnloadRenderTexture(m_Canvas);

            // Note disposing has been done.
            m_bDisposed = true;
        }
    }
}

public class TextureButton : IDisposable
{
    public enum Alignment
    {
        TOPLEFT,
        TOPRIGHT,
        TOPCENTER,
        MIDLEFT,
        MIDRIGHT,
        MIDCENTER,
        BOTTOMLEFT,
        BOTTOMRIGHT,
        BOTTOMCENTER
    }

    public class Configuration
    {
        public Color HoverTint;
        public Alignment Alignment;

        public Configuration()
        {
            HoverTint = new Color(200, 200, 200, 255);
            Alignment = Alignment.MIDCENTER;
        }
    }

    public Configuration Properties = new();
    private Texture2D m_Texture;
    private Vector2 m_Pos;
    private Vector2 m_Size;
    private RenderTexture2D m_Canvas;
    private float m_ScaleFactor = 1f;
    public event ButtonCallback? OnClick;

    public TextureButton(Texture2D texture, Vector2 pos, Vector2? size = null)
    {
        m_Texture = texture;
        m_Pos = pos;

        Vector2 sizeDeNulled = new(0, 0);

        if(size == null)
        {
            sizeDeNulled = new Vector2(0, 0);
        }
        else
        {
            sizeDeNulled = (Vector2)size;
        }

        if(sizeDeNulled.X == 0 && sizeDeNulled.Y != 0)
        {
            float scaleFactor = sizeDeNulled.Y / texture.height;
            sizeDeNulled.X = texture.width * scaleFactor;
        }
        else if(sizeDeNulled.X != 0 && sizeDeNulled.Y == 0)
        {
            float scaleFactor = sizeDeNulled.X / texture.width;
            sizeDeNulled.Y = texture.height * scaleFactor;
        }
        else if(sizeDeNulled.X == 0 && sizeDeNulled.Y == 0)
        {
            sizeDeNulled.X = texture.width;
            sizeDeNulled.Y = texture.height;
        }

        m_Size = sizeDeNulled;
        m_ScaleFactor = texture.width < texture.height ? sizeDeNulled.X / texture.width : sizeDeNulled.Y / texture.height;
        m_Canvas = Raylib.LoadRenderTexture((int)sizeDeNulled.X, (int)sizeDeNulled.Y);
    }

    public TextureButton(Texture2D texture, int posX, int posY, int sizeX = 0, int sizeY = 0) : this(texture, new Vector2(posX, posY), new Vector2(sizeX, sizeY))
    {
    }

    ~TextureButton()
    {
        //Dispose(true);
    }

    public void Draw()
    {
        Raylib.BeginTextureMode(m_Canvas);

        Vector2 texturePos = new();

        switch (Properties.Alignment)
        {
            case Alignment.TOPRIGHT or Alignment.TOPLEFT or Alignment.TOPCENTER:
                texturePos.Y = 0;
                break;
            case Alignment.MIDRIGHT or Alignment.MIDCENTER or Alignment.MIDLEFT:
                texturePos.Y = (m_Size.Y - m_Texture.height * m_ScaleFactor) / 2;
                break;
            case Alignment.BOTTOMRIGHT or Alignment.BOTTOMCENTER or Alignment.BOTTOMLEFT:
                texturePos.Y = m_Size.Y;
                break;
        }
        switch (Properties.Alignment)
        {
            case Alignment.BOTTOMRIGHT or Alignment.MIDRIGHT or Alignment.TOPRIGHT:
                texturePos.X = m_Size.X - m_Texture.width * m_ScaleFactor;
                break;
            case Alignment.BOTTOMCENTER or Alignment.MIDCENTER or Alignment.TOPCENTER:
                texturePos.X = m_Size.X / 2 - m_Texture.width * m_ScaleFactor / 2;
                break;
            case Alignment.BOTTOMLEFT or Alignment.MIDLEFT or Alignment.TOPLEFT:
                texturePos.X = 0;
                break;
        }

        if (IsClicked)
        {
            OnClick?.Invoke();
        }

        Raylib.DrawTextureEx(m_Texture, texturePos, 0f, m_ScaleFactor, IsHovered ? Properties.HoverTint : Color.WHITE);

        Raylib.EndTextureMode();

        Raylib.DrawTexturePro(m_Canvas.texture, new Rectangle(0, 0, m_Size.X, -m_Size.Y), new Rectangle(m_Pos.X, m_Pos.Y, m_Size.X, m_Size.Y), new Vector2(0, 0), 0f, Color.WHITE);
    }


    bool IsHovered => Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(m_Pos.X, m_Pos.Y, m_Size.X, m_Size.Y));
    bool IsClicked => IsHovered && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);


    private bool m_bDisposed = false;

    public void Dispose()
    {
        Dispose(disposing: true);

        GC.Collect();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!m_bDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources.

            }

            Raylib.UnloadRenderTexture(m_Canvas);

            // Note disposing has been done.
            m_bDisposed = true;
        }
    }
}
