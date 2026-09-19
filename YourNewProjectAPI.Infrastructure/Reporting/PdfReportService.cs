using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.Infrastructure.Reporting;

internal sealed class PdfReportService : IPdfReportService
{
    public byte[] GenerateDashboardSummaryPdf(IEnumerable<string> patrakRecords)
    {
        // Sets the open-source community evaluation license type parameters
        QuestPDF.Settings.License = LicenseType.Community;

        // Construct the visual page grid blocks
        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                // 1. TOP TITLE HEADER PANEL
                page.Header().Text("Revenue Patrak System - Summary Report")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                // 2. MIDDLE RECORD TABLE GRID
                page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);   // Id column width
                        columns.RelativeColumn();     // Patrak Name column width
                        columns.ConstantColumn(100);  // Status column width
                    });

                    // Render table header labels
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Id").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Patrak Name").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten1).Padding(5).Text("Status").SemiBold();
                    });

                    // Render row fields dynamically
                    int index = 1;
                    foreach (var record in patrakRecords)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(index.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(record);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text("Active").FontColor(Colors.Green.Medium);
                        index++;
                    }
                });

                // 3. BOTTOM PAGE NUMBER FOOTER PANEL
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        });

        return document.GeneratePdf();
    }
}
