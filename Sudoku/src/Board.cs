using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


public class Board : IDisposable
{
    private Vector2 m_Offset = new();
    private Vector2 m_OffsetTile = new(2, 2);
    private Vector2 m_OffsetTileGroup = new(8, 8);
    private List<Tile> m_Tiles = new();
    private string m_SelectedTileValue = "";
    private int m_ClickedTileID = -1;
    private bool m_bIsComplete = false;
    private SaveFile m_SaveFile;
    private bool m_bHasBeenSaved = false;
    private int m_MaxLives = 5;
    private int m_Lives;

    private BetterRenderTexture m_Canvas;

    private TextureButton m_SaveButton;
    private SaveChildWindow m_SaveWindow;

    private bool m_bScreenshotLoaded = false;
    private Image m_Screenshot;
    private Texture2D m_ScreenshotTexture;


    public Board(int maxLives = 5)
    {
        m_Canvas = new BetterRenderTexture(Raylib.GetRenderWidth(), Raylib.GetRenderHeight(), null, Content);

        m_MaxLives = maxLives;
        for(uint i = 0; i < 81; i++)
        {
            string tileText = Globals.SudokuGenerator.Board1D[i] == 0 ? "" : Globals.SudokuGenerator.Board1D[i].ToString();


            m_Tiles.Add(new Tile(new Vector2(), ref m_Canvas, tileText, i));
            var tilePos = new Vector2(m_Tiles.Last().ID % 9 * m_Tiles.Last().Size.X + m_Tiles.Last().ID % 9 * m_OffsetTile.X + m_Tiles.Last().ID % 9 / 3 * m_OffsetTileGroup.X + m_Offset.X,
                    m_Tiles.Last().ID / 9 * m_Tiles.Last().Size.Y + m_Tiles.Last().ID / 9 * m_OffsetTile.Y + m_Tiles.Last().ID / 9 / 3 * m_OffsetTileGroup.Y + m_Offset.Y);
            m_Tiles.Last().Pos = tilePos;
            m_Tiles.Last().TextLock = m_Tiles.Last().Text != "";
        }


        m_SaveButton = new TextureButton(ref m_Canvas, Globals.ResPack.Textures["save_icon.png"], new Vector2(m_Tiles.Last().Pos.X + m_Tiles.Last().Size.X + m_OffsetTileGroup.X, m_Tiles.Last().Pos.Y + (m_Tiles.Last().Size.Y - Globals.ResPack.Textures["save_icon.png"].height) / 2));
        m_SaveButton.OnClick += SpawnSaveWindow;


        m_Lives = m_MaxLives;

        m_SaveFile = new SaveFile();
        m_SaveFile.Contents.MaxLives = m_MaxLives;

        m_SaveFile.AddFrame(new SaveFrame() { BoardValues = GetValues(), Lives = m_MaxLives });

        m_SaveWindow = new SaveChildWindow(ref m_Canvas, ref m_SaveFile, "Save Board");
        m_SaveWindow.Show = false;
    }

    ~Board()
    {
        Dispose(true);
    }

    public Vector2 Offset
    {
        get => m_Offset;
        set => m_Offset = value;
    }

    private List<int> GetValues()
    {
        List<int> vals = new();
        foreach(Tile tile in m_Tiles)
        {
            vals.Add(tile.Text != "" ? int.Parse(tile.Text) : 0);
        }
        return vals;
    }

    private void DrawHearts()
    {
        int posX = (int)(m_Tiles[Globals.SudokuGenerator.Size - 1].Pos.X + (int)m_Tiles[Globals.SudokuGenerator.Size - 1].Size.X + (int)m_OffsetTileGroup.X);
        int posY = (int)(m_Tiles[Globals.SudokuGenerator.Size - 1].Pos.Y + (int)m_OffsetTileGroup.Y);

        for (int i = 0; i < m_MaxLives; i++) 
        {
            Raylib.DrawTexture(i < m_Lives ? Globals.ResPack.Textures["heart_full.png"] : Globals.ResPack.Textures["heart_empty.png"], posX, posY + i * (Globals.ResPack.Textures["heart_full.png"].height + (int)m_OffsetTileGroup.Y), Color.WHITE);
        }
    }

    private void DrawTiles()
    {
        bool boardComplete = true;

        foreach (var tile in m_Tiles)
        {
            if(tile.Text != Globals.SudokuGenerator.SolvedBoard1D[tile.ID].ToString())
            {
                boardComplete = false;
            }

            if(tile.IsClicked)
            {
                m_ClickedTileID = (int)tile.ID;
                m_SelectedTileValue = tile.Text;
            }
        }

        if (boardComplete)
        {
            m_bIsComplete = true;
        }

        foreach (var tile in m_Tiles)
        {
            if(tile.Text == m_SelectedTileValue && m_SelectedTileValue != "")
            {
                tile.Focus = true;
            }
            else
            {
                tile.Focus = false;
            }
            if(m_bIsComplete)
            {
                tile.Focus = false;
            }
            tile.Draw();
            if(tile.ID == m_ClickedTileID && !m_bIsComplete && !tile.TextLock)
            {
                tile.DrawSelectionMarker();
            }
        }

        if (!m_bIsComplete) 
        { 
            char pressedChar = (char)Raylib.GetCharPressed();
            if (new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }.Contains(pressedChar) && m_ClickedTileID != -1 && !@m_Tiles[m_ClickedTileID].TextLock)
            {
                bool valueChanged = m_Tiles[m_ClickedTileID].Text != pressedChar.ToString();
                m_Tiles[m_ClickedTileID].Text = pressedChar.ToString();
                m_SelectedTileValue = pressedChar.ToString();

                if (m_Tiles[m_ClickedTileID].Mistake && valueChanged)
                {
                    m_Lives = m_Lives > 0 ? m_Lives - 1 : 0;
                }

                m_SaveFile.AddFrame(new SaveFrame()
                {
                    BoardValues = GetValues(),
                    Lives = m_Lives
                });

                m_ClickedTileID = -1;

                DbgShowLives();
                DbgShowSolvedBoard();
                DbgShowInitialBoard();
                DbgShowMTilesVals();
            }
        }
        if (m_bIsComplete && !m_bHasBeenSaved)
        {
            m_SaveFile.Save();
            m_bHasBeenSaved = true;
        }
    }

    private void Content()
    {
        if (m_SaveWindow.Show)
        {
            if (!m_bScreenshotLoaded)
            {
                m_Screenshot = Raylib.LoadImageFromScreen();
                m_ScreenshotTexture = Raylib.LoadTextureFromImage(m_Screenshot);

                m_bScreenshotLoaded = true;
            }

            //Raylib.DrawTexture(m_ScreenshotTexture, 0, 0, Color.WHITE);
            
            Raylib.DrawText("as", 0, 0, 20, Color.RED);
            m_SaveWindow.Draw();

            
        }
        else
        {
            if (m_ScreenshotTexture.width != 0 && m_bScreenshotLoaded)
            {
                Raylib.UnloadImage(m_Screenshot);
                Raylib.UnloadTexture(m_ScreenshotTexture);
                m_bScreenshotLoaded = false;
            }

            DrawHearts();
            DrawTiles();
            m_SaveButton.Draw();
        }

    }

    public void Draw()
    {
        m_Canvas.Draw(new Vector2(0,0));
    }


    private void DbgShowLives()
    {
        Console.WriteLine("DbgShowLives");
        Console.WriteLine($"{m_Lives}/{m_MaxLives}");
    }

    private void DbgShowSolvedBoard()
    {
        Console.WriteLine("DbgShowSolvedBoard");
        for (int i = 0; i < Globals.SudokuGenerator.SolvedBoard1D.Count(); i++) 
        {
            Console.Write($"[{i}] {Globals.SudokuGenerator.SolvedBoard1D[i]}   ");
        }
        Console.WriteLine();
    }

    private void DbgShowMTilesVals()
    {
        Console.WriteLine("DbgShowMTilesVals");
        foreach (var tile in m_Tiles)
        {
            Console.Write($"[{tile.ID}] {tile.Text}   ");
        }
        Console.WriteLine();
    }

    private void DbgShowInitialBoard()
    {
        Console.WriteLine("DbgShowInitialBoard");
        for (int i = 0; i < Globals.SudokuGenerator.Board1D.Count(); i++)
        {
            Console.Write($"[{i}] {Globals.SudokuGenerator.Board1D[i]}   ");
        }
        Console.WriteLine();
    }

    private void SpawnSaveWindow()
    {
        m_SaveWindow.Dispose();
        m_SaveWindow = new SaveChildWindow(ref m_Canvas, ref m_SaveFile, "Save Board");
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

            //Raylib.UnloadRenderTexture(m_Canvas);

            // Note disposing has been done.
            m_bDisposed = true;
        }
    }


    private class SaveChildWindow : IDisposable
    {
        public string Name { get; init; }
        public BetterRenderTexture Parent { get; init; }
        private BetterRenderTexture m_Canvas;
        public SaveFile SaveFile { get; init; }

        private TextObject WindowTitle { get; set; }
        private Textbox FileNameTextbox { get; set; }

        public bool IsHovered => Raylib.CheckCollisionPointRec(m_Canvas.MousePos, new Rectangle(0, 0, m_Canvas.Size.X, m_Canvas.Size.Y));
        public bool IsClicked => IsHovered && Mouse.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
        public bool IsClickedOutside => !IsHovered && Mouse.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);

        public bool Show { get; set; } = true;

        public SaveChildWindow(ref BetterRenderTexture parent, ref SaveFile saveFile, string name)
        {
            Parent = parent;
            Name = name;
            SaveFile = saveFile;
            m_Canvas = new BetterRenderTexture(300, 300, parent.GUID, Content);

            WindowTitle = new TextObject("Save Board", Globals.MainMenuFont, 20, 1f, Color.RED);
            FileNameTextbox = new Textbox(ref m_Canvas, 200, 30, new Textbox.Configuration { Font = Globals.MainMenuFont, OutlineRoundness = 1f });
        }

        private void Content()
        {
            Raylib.ClearBackground(Color.BLUE);
            WindowTitle.Draw(5, 5);
            FileNameTextbox.Draw(new Vector2(60,30));
            //if (IsClickedOutside) { Show = false; }
        }

        public void Draw()
        {
            //m_Canvas.Draw((int)(Raylib.GetRenderWidth() - m_Canvas.Size.X) / 2, (int)(Raylib.GetRenderHeight() - m_Canvas.Size.Y) / 2);
            m_Canvas.Draw(100,100);

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

                //Raylib.UnloadRenderTexture(m_Canvas);

                // Note disposing has been done.
                m_bDisposed = true;
            }
        }
    }
}
