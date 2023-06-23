using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class SaveFrame
{
    public string Name = "";
    public double TimeStamp = Raylib.GetTime();
    public List<int> BoardValues = new();
    public int Lives = 0;
}

public class SaveFile
{
    public class FileContents
    {
        public long CreationTime;
        public int MaxLives;
        public int[] CorrectValues = { };
        public string UID = Guid.NewGuid().ToString();
        public List<SaveFrame> Frames = new();
    }

    public FileContents Contents = new();

    public SaveFile()
    {
        Contents.CreationTime = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        Contents.CorrectValues = Globals.SudokuGenerator.SolvedBoard1D;
    }

    public void AddFrame(SaveFrame frame)
    {
        Contents.Frames.Add(frame);
    }

    public void Save()
    {
        if (!Directory.Exists("./Saves"))
        {
            Directory.CreateDirectory("./Saves");
        }

        string content = JsonConvert.SerializeObject(Contents, Formatting.Indented);

        File.WriteAllText($"./Saves/{Contents.UID}.json", content);
    }
}