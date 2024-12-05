
using System.Diagnostics;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

Console.WriteLine("Welcome to the F1 bingo generator!");
Console.WriteLine("Reading bingo lines");
string[]? entries = null;
string path = "bingo.txt";
if (File.Exists(path))
{
    string content = File.ReadAllText(path);
    entries = content.Split('\n');
}

if (entries == null || entries.Length == 0)
{
    Console.WriteLine(Directory.GetCurrentDirectory());
    Console.WriteLine("Please put some entries in bingo.txt next to this executable!");
    return; 
}

Console.WriteLine("How many bingo cards should be generated?");
int count = -1;
do
{
    try
    {
        count = Convert.ToInt32(Console.ReadLine());
    }
    catch (Exception ex)
    {
        Console.WriteLine("Invalid input.");
        count = -1;
    }
} while (count < 0);

Random rnd = new();
XFont titleFont = new("Verdana", 50, XFontStyleEx.BoldItalic);
XFont font = new("Verdana", 16, XFontStyleEx.Regular);
XPen linePen = new(XColors.Black);

for (int i = 0; i < count; i++)
{
    PdfDocument doc = new();
    doc.Info.Title = "F1 Bingo!";
    PdfPage page = doc.AddPage();
    XGraphics gfx = XGraphics.FromPdfPage(page);
    
    // Drawing
    gfx.DrawString("Formula 1 Bingo!", titleFont, XBrushes.Black, new XRect(0, 0, page.Width, 150), XStringFormats.Center);
    
    const int size = 5;
    const double cellSize = 110;
    const int lineSize = 20;
    const int charLen = 8;
    
    var xCenter = page.Width / 2;
    var xStart = xCenter - size * cellSize * 0.5;
    var xEnd = xCenter + size * cellSize * 0.5;
    var yCenter = page.Height / 2;
    var yStart = yCenter - size * cellSize * 0.5;
    var yEnd = yCenter + size * cellSize * 0.5;
    
    for (int x = 0; x <= size; x++)
        gfx.DrawLine(linePen, xStart + x * cellSize, yStart, xStart + x * cellSize, yEnd);
    
    for (int y = 0; y <= size; y++)
        gfx.DrawLine(linePen, xStart, yStart + y * cellSize, xEnd, yStart + y * cellSize);

    rnd.Shuffle(entries);
    List<string> rand = entries.ToList();
    int c = 0;
    while (c < size * size && rand.Count > 0)
    {
        string str = rand[0].Trim();
        rand.RemoveAt(0);
        if (str == "")
            continue;

        List<string> lines = [""];
        string[] split = str.Split(' ');
        foreach (var s in split)
        {
            if (lines.Last().Length > 0 && s.Length + lines.Last().Length > charLen)
                lines.Add(s);
            else
                lines[^1] = lines[^1] + " " + s;
        }

        int x = c % size;
        int y = c / size;
        var dX = xStart + (x + 0.5) * cellSize;
        var dY = yStart + (y + 0.5) * cellSize;

        
        for (int l = 0; l < lines.Count; l++)
        {
            double lY = ((l + 0.5) - lines.Count / 2.0) * lineSize;
            gfx.DrawString(lines[l], font, XBrushes.Black, new XRect(dX, dY + lY, size, size), XStringFormats.Center);
        }
        
        c++;
    }
    
    string outPath = "bingo_" + i + ".pdf";
    doc.Save(outPath);
    
    using Process fileopener = new Process();
    fileopener.StartInfo.FileName = "explorer";
    fileopener.StartInfo.Arguments = "\"" + outPath + "\"";
    fileopener.Start();
}