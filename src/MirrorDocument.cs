using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace Labelissimo.Templates;

public class MirrorDocument : IDocument
{
  public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
  public void Compose(IDocumentContainer containet)
  {

  }
}