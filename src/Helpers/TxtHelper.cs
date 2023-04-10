namespace Labelissimo.Helpers;

public class TxtHelper
{
    public string[] Lables { get; set; }

    public TxtHelper(string filePath)
    {
        Lables = File.ReadAllLines(filePath);
        for (int i = 0; i < Lables.Length; i++)
            Lables[i] = Lables[i].Trim().Replace('~', '\n');
    }
}