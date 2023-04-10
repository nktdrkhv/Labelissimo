using Labelissimo.Helpers;
using Labelissimo.Templates;
using QuestPDF.Helpers;

namespace MirrorDocument;

public static class Program
{
    public static void Main()
    {
        var txt = new TxtHelper("Files/input.txt");
        var options = new MirrorDocumentOptions()
        {
            PageSize = PageSizes.A4.Landscape(),
            FontSize = 40f,
            CustomFontPath = Path.Combine("Files", "ALS_Sirius_Regular_0.95.otf"),
            Labels = txt.Lables,
        };
        var doc = new Labelissimo.Templates.MirrorDocument(options);
        doc.GeneratePdf("Files/output.pdf");
        options.FontSize = 60f;
        doc.GeneratePdf("Files/output1.pdf");
    }
}