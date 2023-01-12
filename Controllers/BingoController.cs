using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Bingo.Controllers;

[ApiController]
[Route("[controller]")]
public class BingoController : ControllerBase
{
    private readonly ILogger<BingoController> _logger;

    public BingoController(ILogger<BingoController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("Card")]
    public async Task<IActionResult> GenerateBingoCardsAsync([FromBody] CardRequest request)
    {
        var ptBR = new System.Globalization.CultureInfo("pt-BR");
        DateTime start = DateTime.Now;
        var cards = Card.GenerateCards(request.Quantity);
        
        string basePath = Path.Combine(Environment.CurrentDirectory, "cards", "entry-files");

        #region Clear base path
        if (Directory.Exists(basePath))
            Directory.Delete(basePath, true);

        Directory.CreateDirectory(basePath);
        #endregion

        cards.ToList().ForEach(card => 
        {
            var cardFilePath = Path.Combine(basePath, card.Code + ".pdf");
            var writer = new PdfWriter(cardFilePath);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            #region Header
                var header = new Div()
                    .SetPadding(10)
                    // .SetBackgroundColor(new DeviceRgb(234, 234, 234))
                    .SetBorder(new SolidBorder(1))
                    .SetBorderRadius(new BorderRadius(3))
                    .SetMarginBottom(6);

                var title = new Paragraph("AÇÃO ENTRE AMIGOS")
                    .SetFontSize(22)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(0)
                    .SetMultipliedLeading(1)
                    .SetBold();

                header.Add(title);

                var subTitle = new Paragraph($"ENTRE OS DIAS {request.Dates.Min(x => x.Date).ToString("dd")} E {request.Dates.Max(x => x.Date).ToString("dd")} DE {request.Dates.Min(x => x.Date).ToString("MMMM").ToUpper()} DE {request.Dates.Min(x => x.Date).ToString("yyyy")}")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(0)
                    .SetMultipliedLeading(1)
                    .SetBold();

                header.Add(subTitle);

                var promotedBy = new Paragraph(request.PromotedBy.ToUpper())
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(0)
                    .SetMultipliedLeading(1);

                header.Add(promotedBy);

                var place = new Paragraph($"LOCAL: {request.Address.ToUpper()}")
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(0)
                    .SetMultipliedLeading(1);

                header.Add(place);

                document.Add(header);
            #endregion

            #region Card info
                var codeCell = new Cell();
                codeCell.Add(new Paragraph($"CARTELA #{card.Code}")
                    .SetMargin(0)
                    .SetMultipliedLeading(1)
                    .SetFontSize(9))
                    .SetBold()
                    .SetBorder(Border.NO_BORDER);
                
                var priceCell = new Cell();
                priceCell.Add(new Paragraph($"VALOR: {request.Price.ToString("C")}")
                    .SetMargin(0)
                    .SetMultipliedLeading(1)
                    .SetFontSize(9))
                    .SetBold()
                    .SetBorder(Border.NO_BORDER);
                
                var timeCell = new Cell();
                timeCell.Add(new Paragraph($"HORÁRIO: {request.Dates.First().Date.ToString("HH:mm")}")
                    .SetMargin(0)
                    .SetMultipliedLeading(1)
                    .SetFontSize(9))
                    .SetBold()
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT);

                var cardInfo = new Table(UnitValue.CreatePercentArray(new float[] {60, 20, 20}))
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .AddCell(codeCell)
                    .AddCell(priceCell)
                    .AddCell(timeCell);

                document.Add(cardInfo);

                var separator = new LineSeparator(new SolidLine())
                    .SetMarginTop(2)
                    .SetMarginBottom(2);
                    
                document.Add(separator);
            #endregion

            #region Card content
            var table = new Table(UnitValue.CreatePercentArray(3))
                .SetMarginTop(3)
                .SetWidth(UnitValue.CreatePercentValue(100));
            foreach (var date in request.Dates)
            {
                var cell = new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .SetPaddingLeft(10)
                    .SetPaddingBottom(4)
                    .SetPaddingTop(4)
                    .SetPaddingRight(10);
                var tableDiv = new Div()
                    .SetMarginBottom(2);

                var dateText = new Paragraph($"DIA {date.Date.ToString("dd/MM/yyyy")} - {ptBR.DateTimeFormat.GetDayName(date.Date.DayOfWeek).ToUpper()}")
                    .SetBold()
                    .SetMargin(0)
                    .SetMultipliedLeading(1)
                    .SetFontSize(7);

                var awardText = new Paragraph($"{date.AwardDescription}")
                    .SetMargin(0)
                    .SetMultipliedLeading(1)
                    .SetFontSize(7);

                var cardTable = new Table(UnitValue.CreatePercentArray(5))
                    .SetWidth(UnitValue.CreatePercentValue(100));

                cardTable.AddHeaderCell(new Cell().Add(new Paragraph("B").SetMargin(0).SetFontSize(11).SetBold().SetTextAlignment(TextAlignment.CENTER)).SetPadding(0).SetSpacingRatio(0).SetBackgroundColor(new DeviceRgb(234, 234, 234)));
                cardTable.AddHeaderCell(new Cell().Add(new Paragraph("I").SetMargin(0).SetFontSize(11).SetBold().SetTextAlignment(TextAlignment.CENTER)).SetPadding(0).SetSpacingRatio(0).SetBackgroundColor(new DeviceRgb(234, 234, 234)));
                cardTable.AddHeaderCell(new Cell().Add(new Paragraph("N").SetMargin(0).SetFontSize(11).SetBold().SetTextAlignment(TextAlignment.CENTER)).SetPadding(0).SetSpacingRatio(0).SetBackgroundColor(new DeviceRgb(234, 234, 234)));
                cardTable.AddHeaderCell(new Cell().Add(new Paragraph("G").SetMargin(0).SetFontSize(11).SetBold().SetTextAlignment(TextAlignment.CENTER)).SetPadding(0).SetSpacingRatio(0).SetBackgroundColor(new DeviceRgb(234, 234, 234)));
                cardTable.AddHeaderCell(new Cell().Add(new Paragraph("O").SetMargin(0).SetFontSize(11).SetBold().SetTextAlignment(TextAlignment.CENTER)).SetPadding(0).SetSpacingRatio(0).SetBackgroundColor(new DeviceRgb(234, 234, 234)));

                cardTable.AddCell(new Cell().Add(new Paragraph(card.B[0].ToString().PadLeft(2, '0')).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.I[0].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.N[0].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.G[0].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.O[0].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));

                cardTable.AddCell(new Cell().Add(new Paragraph(card.B[1].ToString().PadLeft(2, '0')).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.I[1].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.N[1].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.G[1].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.O[1].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));

                cardTable.AddCell(new Cell().Add(new Paragraph(card.B[2].ToString().PadLeft(2, '0')).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.I[2].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph("FREE SPACE").SetMargin(0).SetMultipliedLeading(1).SetFontSize(5).SetFontColor(ColorConstants.WHITE).SetBold()).SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.BLACK).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.G[2].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.O[2].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));

                cardTable.AddCell(new Cell().Add(new Paragraph(card.B[3].ToString().PadLeft(2, '0')).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.I[3].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.N[2].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.G[3].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.O[3].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));

                cardTable.AddCell(new Cell().Add(new Paragraph(card.B[4].ToString().PadLeft(2, '0')).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));                
                cardTable.AddCell(new Cell().Add(new Paragraph(card.I[4].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.N[3].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.G[4].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));
                cardTable.AddCell(new Cell().Add(new Paragraph(card.O[4].ToString()).SetMargin(0)).SetTextAlignment(TextAlignment.CENTER));

                tableDiv.Add(dateText);
                tableDiv.Add(awardText);
                cell.Add(tableDiv);
                cell.Add(cardTable);
                table.AddCell(cell);
            }

            var divCardContent = new Div().SetMarginLeft(-10).SetMarginRight(-10);
            divCardContent.Add(table);
            document.Add(divCardContent);
            #endregion
            
            #region Conference line
                var conferenceLineDiv = new Div()
                    .SetMarginTop(3)
                    .SetWidth(UnitValue.CreatePercentValue(100));

                var conferenceLineText = new Paragraph("LINHA PARA CONFERÊNCIA")
                    .SetFontSize(7)
                    .SetMarginBottom(1)
                    .SetMultipliedLeading(1);

                var conferenceLineTable = new Table(UnitValue.CreatePercentArray(24))
                    .SetWidth(UnitValue.CreatePercentValue(100));
                card.All.ToList().ForEach(x => conferenceLineTable.AddCell(new Cell().Add(new Paragraph(x.ToString().PadLeft(2, '0')).SetMargin(0).SetMultipliedLeading(1)).SetTextAlignment(TextAlignment.CENTER)));

                conferenceLineDiv.Add(conferenceLineText);
                conferenceLineDiv.Add(conferenceLineTable);

                document.Add(conferenceLineDiv);
            #endregion

            #region Footer
            document.Add(new Paragraph(new Text("PÉ-FRIO: ").SetBold()).Add(new Text("DESTAQUE O CANHOTO DO PÉ-FRIO NA LINHA TRACEJADA E COLOQUE-O NA CAIXA DO EVENTO.")).SetFontSize(8).SetMultipliedLeading(1).SetMarginTop(16).SetMarginBottom(0));
            document.Add(new LineSeparator(new DashedLine(1)).SetMarginTop(2).SetMarginBottom(6));

            var footer = new Div()
                .SetPadding(10)
                .SetBorder(new SolidBorder(1))
                .SetBorderRadius(new BorderRadius(3));


            footer.Add(new Paragraph("NOME:").SetFontSize(8).SetMultipliedLeading(1).SetMargin(0).SetMarginBottom(2));
            footer.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(6));
            
            footer.Add(new Paragraph("TELEFONE:").SetFontSize(8).SetMultipliedLeading(1).SetMargin(0).SetMarginBottom(2));
            footer.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(6));
            
            footer.Add(new Paragraph("ENDEREÇO:").SetFontSize(8).SetMultipliedLeading(1).SetMargin(0).SetMarginBottom(2));
            footer.Add(new LineSeparator(new SolidLine(1)).SetMarginBottom(6));
            
            footer.Add(new Paragraph($"CARTELA: #{card.Code} - DATA: {request.Dates.Min(x => x.Date).ToString("dd/MM/yyyy")} à {request.Dates.Max(x => x.Date).ToString("dd/MM/yyyy")}").SetFontSize(6).SetMultipliedLeading(1).SetBold().SetMargin(0));
            footer.Add(new Paragraph($"{request.PromotedBy} - {request.Address}").SetFontSize(6).SetMultipliedLeading(1).SetBold().SetMargin(0));

            
            document.Add(footer);
            #endregion
            
            document.Close();
        });

        #region Merge files
            string mergedFileName = Guid.NewGuid().ToString() + ".pdf";
            string mergePath = Path.Combine(Environment.CurrentDirectory, "cards");
            var writer = new PdfWriter(Path.Combine(mergePath, mergedFileName));
            var pdf = new PdfDocument(writer);
            var merger = new PdfMerger(pdf);

            Directory.GetFiles(basePath).ToList().ForEach(file => 
            {
                var pdfFile = new PdfDocument(new PdfReader(file));
                merger.Merge(pdfFile, 1, pdfFile.GetNumberOfPages());
                pdfFile.Close();
            });

            pdf.Close();
        #endregion

        var provider = new PhysicalFileProvider(mergePath);
        var fileInfo = provider.GetFileInfo(mergedFileName);
        var readStream = fileInfo.CreateReadStream();

        TimeSpan time = (DateTime.Now - start);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($">> {cards.Count()} generated in {time.TotalMilliseconds}ms.");
        Console.ResetColor();
        return File(readStream, "application/pdf", mergedFileName);
    }
}
