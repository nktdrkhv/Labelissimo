using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Labelissimo.Templates;

public class MirrorDocumentOptions
{
    public string[] Labels { get; set; } = null!;
    public PageSize PageSize { get; set; } = PageSizes.A4;
    public float TranslateX { get; set; }
    public float TranslateY { get; set; }
    public string? CustomFontPath { get; set; }
    public string FontFamily { get; set; } = Fonts.Arial;
    public float FontSize { get; set; } = 20f;
    public FontWeight FontWeight { get; set; } = FontWeight.Normal;
    public float LetterSpacing { get; set; } = 0f;
    public float LineHeight { get; set; } = 1f;
    public string? BackgroundImagePath { get; set; }
}

public class MirrorDocument : IDocument
{
    private readonly MirrorDocumentOptions _options;
    private int _currentPage;

    public MirrorDocument(MirrorDocumentOptions options)
    {
        _options = options;
        if (_options.CustomFontPath is string source)
        {
            _options.FontFamily = Guid.NewGuid().ToString();
            FontManager.RegisterFontWithCustomName(_options.FontFamily, File.OpenRead(source));
        }
    }
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public byte[] GeneratePdf()
    {
        _currentPage = 0;
        return ((IDocument)this).GeneratePdf();
    }

    public void GeneratePdf(string path)
    {
        _currentPage = 0;
        ((IDocument)this).GeneratePdf(path);
    }

    public byte[] ImagePreview(int number = 0)
    {
        _currentPage = 0;
        var temp = _options.Labels;
        _options.Labels = new[] { _options.Labels.ElementAt(number) };
        var image = ((IDocument)this).GenerateImages();
        _options.Labels = temp;
        return image.First();
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(_options.PageSize);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(textStyle => textStyle
                .FontFamily(_options.FontFamily)
                .FontSize(_options.FontSize)
                .Weight(_options.FontWeight)
                .LetterSpacing(_options.LetterSpacing)
                .LineHeight(_options.LineHeight));

            page.Content()
                .Layers(layers =>
                {
                    layers.PrimaryLayer()
                        .Column(column =>
                        {
                            var labelComponent = new Label(_options);
                            foreach (var label in _options.Labels)
                            {
                                if (_currentPage != 0)
                                    column.Item().PageBreak();
                                labelComponent.Data = label;
                                column.Item().FlipOver().Component(labelComponent);
                                column.Item().Component(labelComponent);
                                _currentPage++;
                            }
                        });
                    if (_options.BackgroundImagePath is string path)
                        layers.Layer()
                            .Image(path, ImageScaling.FitArea);
                });
        });
    }

    private class Label : IComponent
    {
        private MirrorDocumentOptions _options;
        public string Data { get; set; } = null!;
        public Label(MirrorDocumentOptions options) => _options = options;
        public void Compose(IContainer container)
        {
            container
                .MinHeight(_options.PageSize.Height / 2)
                .AlignMiddle()
                .AlignCenter()
                .TranslateX(_options.TranslateX)
                .TranslateY(_options.TranslateY)
                .Text(text =>
                {
                    // todo: special text style
                    text.AlignCenter();
                    text.Span(Data);
                });
        }
    }
}