using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

public delegate void BetterRenderTextureContent();

file static class BRTTree
{
    public static Dictionary<Guid, Vector2> RealPositions = new();
}

public class BetterRenderTexture : IDisposable
{
    private RenderTexture2D m_RenderTexture;

    public event BetterRenderTextureContent? Content;

    public Texture2D Texture => m_RenderTexture.texture;
    public Texture2D Depth => m_RenderTexture.depth;
    public uint ID => m_RenderTexture.id;
    public Vector2 Pos { get; set; } = Vector2.Zero;
    public Vector2 ParentPos { get => BRTTree.RealPositions[ParentGuid];}
    public Vector2 Size { get;set; }
    public Guid GUID { get; init; }
    public Guid ParentGuid { get; init; }
    public Vector2 RealPos => BRTTree.RealPositions[GUID];


    public BetterRenderTexture(Vector2 size, Guid? parentGuid = null, BetterRenderTextureContent? content = null)
    {
        GUID = Guid.NewGuid();
        if(parentGuid is not null)
        {
            ParentGuid = (Guid)parentGuid;
            BRTTree.RealPositions.Add(GUID, BRTTree.RealPositions[ParentGuid] + Pos);
        }
        else
        {
            BRTTree.RealPositions.Add(GUID, new Vector2(0,0));
            ParentGuid = GUID;
        }
        Size = size;
        Content = content;
        m_RenderTexture = Raylib.LoadRenderTexture((int)size.X, (int)size.Y);
    }

    public BetterRenderTexture(int sizeX, int sizeY, Guid? parentGuid = null, BetterRenderTextureContent? content = null) : this(new Vector2(sizeX, sizeY), parentGuid, content)
    {

    }

    ~BetterRenderTexture()
    {
        BRTTree.RealPositions.Remove(GUID);
    }


    public Vector2 MousePos => Mouse.GetMousePosition() - BRTTree.RealPositions[GUID];


    public void Draw(Vector2 pos)
    {
        if(pos != Pos)
        {
            Console.WriteLine($"{pos.X} {pos.Y}");
            Pos = pos;
            Console.WriteLine($"MY GUID: [{GUID}] >>> PARENT GUID: [{ParentGuid}]");
            if (ParentGuid == GUID)
            {
                BRTTree.RealPositions[GUID] = Pos;
            }
            else
            {
                BRTTree.RealPositions[GUID] = BRTTree.RealPositions[ParentGuid] + Pos;
                Console.WriteLine($"MY REAL POS: [{BRTTree.RealPositions[GUID].X};{BRTTree.RealPositions[GUID].Y}] >> MY POS: [{Pos.X};{Pos.Y}]");
            }
        }

        UpdateSize();
        Raylib.BeginTextureMode(m_RenderTexture);

        Raylib.ClearBackground(Color.BLANK);

        if (Content is not null)
        {
            Content();
        }

        Raylib.EndTextureMode();
        Raylib.DrawTexturePro(Texture, new Rectangle(0, 0, Size.X, -Size.Y), new Rectangle(Pos.X, Pos.Y, Size.X, Size.Y), new Vector2(0, 0), 0f, Color.WHITE);
    }

    public void Draw(int posX, int posY)
    {
        Draw(new Vector2(posX, posY));
    }

    private void UpdateSize()
    {
        if(Size.X != Texture.width || Size.Y != Texture.height)
        {
            Raylib.UnloadRenderTexture(m_RenderTexture);
            m_RenderTexture = Raylib.LoadRenderTexture((int)Size.X, (int)Size.Y);
        }
    }


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

            Raylib.UnloadRenderTexture(m_RenderTexture);

            // Note disposing has been done.
            m_bDisposed = true;
        }
    }
}
