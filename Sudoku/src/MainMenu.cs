using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using static MainMenu;

public class MainMenu
{
    public enum Difficulty
    {
        Beginner,
        Easy,
        Medium,
        Hard,
        Extreme
    }
    
    private class DifficultyFrame
    {
        private BetterRenderTexture m_Canvas;
        private TextureButton m_BackButton;
        private TextureButton m_NextButton;
        private TextObject m_Text;
        private Difficulty m_Difficulty;

        public Difficulty Difficulty => m_Difficulty;

        public Vector2 Size => CanvasSize;

        public DifficultyFrame(ref BetterRenderTexture parent, int width, int height)
        {
            m_Canvas = new BetterRenderTexture(width, height, parent.GUID, Content);

            m_BackButton = new TextureButton(ref m_Canvas, Globals.ResPack.Textures["back_button.png"], 0, 0);
            m_NextButton = new TextureButton(ref m_Canvas, Globals.ResPack.Textures["next_button.png"], width - Globals.ResPack.Textures["next_button.png"].width, 0);
            m_BackButton.OnClick += DecreaseDifficulty;
            m_NextButton.OnClick += IncreaseDifficulty;
            m_Text = new TextObject("Beginner", Globals.MainMenuFont, 20, 1, Color.BLACK);
        }

        private void Content()
        {
            m_BackButton.Draw();
            m_NextButton.Draw();

            if (m_Text.Text != m_Difficulty.ToString())
            {
                m_Text.Text = m_Difficulty.ToString();
            }

            m_Text.Draw((int)(CanvasSize.X - m_Text.Measurements.X) / 2, (int)(CanvasSize.Y - m_Text.Measurements.Y) / 2);
        }

        public void Draw(Vector2 pos)
        {

            m_Canvas.Draw(pos);
            /*Console.WriteLine($"Pos: [{pos.X} : {pos.Y}]");
            Raylib.DrawTexturePro(m_Canvas.texture, new Rectangle(0, 0, CanvasSize.X, -CanvasSize.Y), new Rectangle(pos.X, pos.Y, CanvasSize.X, CanvasSize.Y), Vector2.Zero, 0f, Color.WHITE);*/

        }

        public void Draw(int posX, int posY)
        {
            Draw(new Vector2(posX, posY));
        }

        private Vector2 CanvasSize => m_Canvas.Size;

        private void IncreaseDifficulty()
        {
            int diffInt = (int)m_Difficulty;
            diffInt = (diffInt + 1) % Enum.GetNames(typeof(Difficulty)).Length;
            m_Difficulty = (Difficulty)Enum.ToObject(typeof(Difficulty), diffInt);
        }

        private void DecreaseDifficulty()
        {
            int diffInt = (int)m_Difficulty;
            diffInt = diffInt - 1 < 0 ? diffInt = Enum.GetNames(typeof(Difficulty)).Length - 1 : diffInt - 1;
            m_Difficulty = (Difficulty)Enum.ToObject(typeof(Difficulty), Math.Abs(diffInt));
        }

    }

    public enum Mode
    {
        Arcade,
        Timely,
        Something,
        WOWIDK
    }

    private class ModeFrame
    {
        private BetterRenderTexture m_Canvas;
        private TextureButton m_BackButton;
        private TextureButton m_NextButton;
        private TextObject m_Text;
        private Mode m_Mode;

        public Vector2 Size => CanvasSize;
        public Mode Mode => m_Mode;

        public ModeFrame(ref BetterRenderTexture parent, int width, int height)
        {
            m_Canvas = new BetterRenderTexture(width, height, parent.GUID, Content);

            m_BackButton = new TextureButton(ref m_Canvas, Globals.ResPack.Textures["back_button.png"], 0, 0);
            m_NextButton = new TextureButton(ref m_Canvas, Globals.ResPack.Textures["next_button.png"], width - Globals.ResPack.Textures["next_button.png"].width, 0);
            m_BackButton.OnClick += DecreaseMode;
            m_NextButton.OnClick += IncreaseMode;
            m_Text = new TextObject("Arcade", Globals.MainMenuFont, 20, 1, Color.BLACK);
        }

        private void Content()
        {
            m_BackButton.Draw();
            m_NextButton.Draw();

            if (m_Text.Text != m_Mode.ToString())
            {
                m_Text.Text = m_Mode.ToString();
            }

            m_Text.Draw((int)(CanvasSize.X - m_Text.Measurements.X) / 2, (int)(CanvasSize.Y - m_Text.Measurements.Y) / 2);
        }

        public void Draw(Vector2 pos)
        {
            m_Canvas.Draw(pos);
        }

        public void Draw(int posX, int posY)
        {
            Draw(new Vector2(posX, posY));
        }

        private Vector2 CanvasSize => m_Canvas.Size;

        private void IncreaseMode()
        {
            int diffInt = (int)m_Mode;
            diffInt = (diffInt + 1) % Enum.GetNames(typeof(Mode)).Length;
            m_Mode = (Mode)Enum.ToObject(typeof(Mode), diffInt);
        }

        private void DecreaseMode()
        {
            int diffInt = (int)m_Mode;
            diffInt = diffInt - 1 < 0 ? diffInt = Enum.GetNames(typeof(Mode)).Length - 1 : diffInt - 1;
            m_Mode = (Mode)Enum.ToObject(typeof(Mode), Math.Abs(diffInt));
        }

    }

    private BetterRenderTexture m_Canvas;
    private Texture2D m_Logo;

    private DifficultyFrame m_DifficultyFrame;
    private ModeFrame m_ModeFrame;

    private Vector2 CanvasSize => m_Canvas.Size;

    private Button m_PlayButton;

    public Difficulty SelectedDifficulty => m_DifficultyFrame.Difficulty;
    public Mode SelectedMode => m_ModeFrame.Mode;
    public bool ShouldStartGame { get; private set; } = false;

    public MainMenu()
    {
        m_Canvas = new BetterRenderTexture(Raylib.GetRenderWidth(), Raylib.GetRenderHeight(), null, Content);
        m_Logo = Globals.ResPack.Textures["logo.png"];
        m_DifficultyFrame = new DifficultyFrame(ref m_Canvas, 150, 32);
        m_ModeFrame = new ModeFrame(ref m_Canvas, 150, 32);
        m_PlayButton = new Button(ref m_Canvas, "Play", (int)(CanvasSize.X - 100) / 2, (int)(CanvasSize.Y - 40) / 2, 100, 40);
        m_PlayButton.OnClick += () => { ShouldStartGame = true; };
    }

    private void Content()
    {
        Raylib.DrawTextureV(m_Logo, new Vector2((CanvasSize.X - m_Logo.width) / 2, 50), Color.WHITE);
        m_DifficultyFrame.Draw((int)(Raylib.GetRenderWidth() - m_DifficultyFrame.Size.X) / 2, 50 + m_Logo.height + 50);
        m_ModeFrame.Draw((int)(Raylib.GetRenderWidth() - m_DifficultyFrame.Size.X) / 2, 50 + m_Logo.height + 50 + 32 + 50);

        m_PlayButton.Draw();
    }

    public void Draw()
    {
        UpdateCanvasSize();
        m_Canvas.Draw(Vector2.Zero);
    }

    private void UpdateCanvasSize()
    {
        if(CanvasSize.X != Raylib.GetRenderWidth() || CanvasSize.Y != Raylib.GetRenderHeight())
        {
            m_Canvas.Size = new Vector2(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
        }
    }

    ~MainMenu()
    {
        Raylib.UnloadTexture(m_Logo);
    }
}
