using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using Avaratra.BackOffice.Models;

namespace Avaratra.BackOffice.Services
{
    public class PdfReportGeneratorService
    {
        public static byte[] GenerateRegionReport(string regionName, int totalPopulation, List<(string Name, int Population)> districts)
        {
            using var stream = new MemoryStream();
            var doc = new Document(PageSize.A4, 50, 50, 80, 50); // marges
            var writer = PdfWriter.GetInstance(doc, stream);

            // Pied de page
            writer.PageEvent = new PdfPageEvents();

            doc.Open();

            // Fonts
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 51, 102));
            var subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(0, 0, 0));
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, new  BaseColor(0, 0, 0));

            // En‑tête institutionnel
            var header = new Paragraph("MINISTÈRE DE L’EAU ET DU CLIMAT", titleFont) { Alignment = Element.ALIGN_CENTER };
            doc.Add(header);

            doc.Add(new Paragraph("Rapport officiel sur la région", subTitleFont) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(" ", normalFont));

            // Informations générales
            var info = new Paragraph($"Région : {regionName}\nPopulation totale : {totalPopulation}", normalFont)
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };
            doc.Add(info);
            doc.Add(new Paragraph(" ", normalFont));

            // Tableau des districts
            if (districts != null && districts.Count > 0)
            {
                doc.Add(new Paragraph("Liste des districts :", subTitleFont));
                doc.Add(new Paragraph(" ", normalFont));

                var table = new PdfPTable(2) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 3, 2 });

                // En‑têtes
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
                var headerBg = new BaseColor(0, 51, 102);

                table.AddCell(new PdfPCell(new Phrase("Nom du district", headerFont)) { BackgroundColor = headerBg, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Population", headerFont)) { BackgroundColor = headerBg, HorizontalAlignment = Element.ALIGN_CENTER });

                // Lignes alternées
                bool alternate = false;
                foreach (var d in districts)
                {
                    var bg = alternate ? new BaseColor(240, 240, 240) : new BaseColor(255, 255, 255);
                    table.AddCell(new PdfPCell(new Phrase(d.Name, normalFont)) { BackgroundColor = bg });
                    table.AddCell(new PdfPCell(new Phrase(d.Population.ToString(), normalFont)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_CENTER });
                    alternate = !alternate;
                }

                doc.Add(table);
            }
            else
            {
                doc.Add(new Paragraph("Encore aucun district.", normalFont));
            }

            doc.Close();
            return stream.ToArray();
        }
        
        public static byte[] GenerateRegionsListReport(List<Region> regions)
        {
            using var stream = new MemoryStream();
            var doc = new Document(PageSize.A4, 50, 50, 80, 50);
            var writer = PdfWriter.GetInstance(doc, stream);
            writer.PageEvent = new PdfPageEvents();
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 51, 102));
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, new BaseColor(0, 0, 0));

            // Titre
            var header = new Paragraph("MINISTÈRE DE L’EAU ET DU CLIMAT", titleFont) { Alignment = Element.ALIGN_CENTER };
            doc.Add(header);
            doc.Add(new Paragraph("Rapport officiel des régions filtrées", titleFont) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(" ", normalFont));

            // Tableau
            var table = new PdfPTable(3) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3, 2, 2 });

            var headerBg = new BaseColor(0, 51, 102);
            table.AddCell(new PdfPCell(new Phrase("Nom de la région", headerFont)) { BackgroundColor = headerBg, HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Population totale", headerFont)) { BackgroundColor = headerBg, HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Nb de districts", headerFont)) { BackgroundColor = headerBg, HorizontalAlignment = Element.ALIGN_CENTER });

            bool alternate = false;
            foreach (var r in regions)
            {
                var bg = alternate ? new BaseColor(240, 240, 240) : new BaseColor(255, 255, 255);
                table.AddCell(new PdfPCell(new Phrase(r.intitule, normalFont)) { BackgroundColor = bg });
                table.AddCell(new PdfPCell(new Phrase(r.totalPopulationRegion.ToString(), normalFont)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase(r.Districts?.Count.ToString() ?? "0", normalFont)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_CENTER });
                alternate = !alternate;
            }

            doc.Add(table);
            doc.Close();
            return stream.ToArray();
        }
    }

    // Classe pour pied de page
    public class PdfPageEvents : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            var cb = writer.DirectContent;
            var footer = new Phrase($"Page {writer.PageNumber}",
                FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(128, 128, 128)));

            ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT,
                footer, document.Right, document.Bottom - 20, 0);
        }
    }

    

}
