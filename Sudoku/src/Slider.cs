using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

public class Slider
{

    public class Configuration
    {
        public Color DragColor;
        public Color SliderColor;
        public Color SliderRemainderColor;
        public uint LineWidth;
        public Vector2 DragSize;
        public float DragRoundness;


        public Configuration()
        {
            DragColor = Color.PURPLE;
            SliderColor = Color.LIGHTGRAY;
            SliderRemainderColor = Color.GRAY;
            LineWidth = 2;
            DragSize = new Vector2(4, 4);
            DragRoundness = 0f;
        }
    }

    internal class Drag
    {
        public BetterRenderTexture Parent { get; init; }
        public Configuration SliderProperties { get; init; }
        public int Pos { get; set; } = 0;
        private Vector2 Size => SliderProperties.DragSize;

        public Drag(BetterRenderTexture parent, ref Configuration sliderProperties)
        {
            Parent = parent;
            SliderProperties = sliderProperties;
        }


        public void Draw(int posX)
        {

        }
    }

    public BetterRenderTexture Parent { get; init; }
    public Vector2 Pos { get; private set; }
    public Vector2 Size { get; init;}
    public Configuration Properties { get; set; }

    private bool IsHovered => Raylib.CheckCollisionPointRec(Parent.MousePos, new Rectangle(Pos.X, Pos.Y, Size.X, Size.Y));


    public Slider(BetterRenderTexture parent, Vector2 pos, Vector2 size, Configuration? properties = null)
    {
        Parent = parent;
        Pos = pos;
        Size = size;
        Properties = properties is not null ? properties : new Configuration();
    }

    public Slider(BetterRenderTexture parent, int posX, int posY, int sizeX, int sizeY, Configuration? properties = null)
        : this(parent, new Vector2(posX, posY), new Vector2(sizeX, sizeY), properties)
    {

    }

    public void Draw(Vector2? pos = null)
    {
        if (pos is not null) 
        {
            Pos = (Vector2)pos;
        }



    }

    public void Draw(int? posX = null, int? posY = null)
    {
        Vector2 newPos = Pos;
        if (posX is not null)
        {
            newPos.X = (int)posX;
        }
        if(posY is not null)
        {
            newPos.Y = (int)posY;
        }
        Draw(newPos);
    }
}