using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public struct TileStyle
{
    public Font Font = Raylib.GetFontDefault();
    public Color TextColor = Color.BLACK;
    public Color TextErrorColor = Color.RED;
    public float HoverEffectAlpha = 150f;
    public float ClickEffectAlpha = 255f;
    public float TextSize = 48f;

    public TileStyle()
    {
        Font = Raylib.GetFontDefault();
        TextColor = Color.BLACK;
        TextErrorColor = Color.RED;
        HoverEffectAlpha = 150f;
        ClickEffectAlpha = 255f;
        TextSize = 40.0f;
    }
}

public class Tile
{
    private Vector2 m_Pos;
    private Vector2 m_Size;
    private bool m_Focused = false;
    private bool m_TextLock = false;
    private string m_Text;
    private uint m_ID = 0;
    private bool m_bIsMistake = false;

    public Tile(Vector2 pos, string text = "", uint id = 0)
    {
        m_Pos = pos;
        m_Size = new Vector2(Globals.ResPack.Textures["cell_normal.png"].width, Globals.ResPack.Textures["cell_normal.png"].height);
        m_Text = text;
        m_ID = id;
    }

    public void Draw()
    {
        if (m_Text != Globals.SudokuGenerator.SolvedBoard1D[ID].ToString()) 
        {
            m_bIsMistake = true;
        }
        else
        {
            m_bIsMistake = false;
        }

        if (IsClicked) 
        {
            Focus = !Focus;
        }

        if (IsClickedOutsideRect && Focus)
        {
            Focus = false;
        }

        Texture2D texture = Focus ? Globals.ResPack.Textures["cell_focused.png"] : Globals.ResPack.Textures["cell_normal.png"];
        Color color = Text == "" ? Globals.TileStyle.TextColor : ( int.Parse(Text) == Globals.SudokuGenerator.SolvedBoard1D[ID] ? Globals.TileStyle.TextColor : Globals.TileStyle.TextErrorColor );
        TextObject textObject = new(m_Text, Globals.TileStyle.Font, (int)Globals.TileStyle.TextSize, 1f, color);

        Raylib.DrawTextureV(texture, m_Pos, Color.WHITE);
        if (IsHovered)
        {
            Raylib.DrawTextureV(Globals.ResPack.Textures["cell_focused.png"], m_Pos, new Color(255, 255, 255, (int)Globals.TileStyle.HoverEffectAlpha));
        }
        textObject.Draw(new Vector2(m_Pos.X + (m_Size.X - textObject.Measurements.X) / 2, m_Pos.Y + (m_Size.Y - textObject.Measurements.Y) / 2));
    }

    public void DrawSelectionMarker()
    {
        Raylib.DrawTextureV(Globals.ResPack.Textures["selection.png"], m_Pos, Color.WHITE);
    }

    public Vector2 Pos
    {
        get => m_Pos;
        set => m_Pos = value;
    }

    public Vector2 Size
    {
        get => m_Size;
        set => m_Size = value;
    }

    public bool Focus
    {
        get => m_Focused;
        set => m_Focused = value;
    }

    public string Text
    {
        get => m_Text;
        set => m_Text = value;
    }

    public uint ID
    {
        get => m_ID;
        set => m_ID = value;
    }

    public bool TextLock
    {
        get => m_TextLock;
        set => m_TextLock = value;
    }

    public bool Mistake
    {
        get => m_bIsMistake;
    }

    public bool IsHovered => Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(m_Pos.X, m_Pos.Y, m_Size.X, m_Size.Y));

    public bool IsClicked => IsHovered && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);

    private bool IsClickedOutsideRect => !IsHovered && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
}
