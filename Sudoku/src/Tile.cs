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
    private TextObject m_TextObject;
    
    public Vector2 Pos { get; set; }
    public Vector2 Size { get; set; }
    public bool Focus { get; set; } = false;
    public string Text { get; set; } = "";
    public uint ID { get; private set; } = 0;
    public bool TextLock { get; set; } = false;
    public BetterRenderTexture Parent { get; init; }
    //public Guid GUID { get; init; }

    public Vector2 MousePos => Parent.MousePos;
    public bool Mistake => Text != Globals.SudokuGenerator.SolvedBoard1D[ID].ToString() && Text != "";

    public Tile(Vector2 pos, ref BetterRenderTexture parent, string text = "", uint id = 0)
    {
        Pos = pos;
        Parent = parent;
        Size = new Vector2(Globals.ResPack.Textures["cell_normal.png"].width, Globals.ResPack.Textures["cell_normal.png"].height);
        Text = text;
        ID = id;
        m_TextObject = new TextObject(text, Globals.TileStyle.Font, (int)Globals.TileStyle.TextSize, 1f, Globals.TileStyle.TextColor);
    }

    public void Draw()
    {
        if(m_TextObject.Text != Text)
        {
            m_TextObject.Text = Text;
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
        Color color = Mistake ? Globals.TileStyle.TextErrorColor : Globals.TileStyle.TextColor;
        m_TextObject.Color = color;

        Raylib.DrawTextureV(texture, Pos, Color.WHITE);
        if (IsHovered)
        {
            Raylib.DrawTextureV(Globals.ResPack.Textures["cell_focused.png"], Pos, new Color(255, 255, 255, (int)Globals.TileStyle.HoverEffectAlpha));
        }
        m_TextObject.Draw(new Vector2(Pos.X + (Size.X - m_TextObject.Measurements.X) / 2, Pos.Y + (Size.Y - m_TextObject.Measurements.Y) / 2));
    }

    public void DrawSelectionMarker()
    {
        Raylib.DrawTextureV(Globals.ResPack.Textures["selection.png"], Pos, Color.WHITE);
    }

    public bool IsHovered => Raylib.CheckCollisionPointRec(MousePos, new Rectangle(Pos.X, Pos.Y, Size.X, Size.Y));

    public bool IsClicked => IsHovered && Mouse.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);

    private bool IsClickedOutsideRect => !IsHovered && Mouse.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
}
