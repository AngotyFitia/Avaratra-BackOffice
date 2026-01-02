using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Linq;
using Avaratra.BackOffice.Models;

namespace Avaratra.BackOffice.Utils
{
    public static class PdfGenerator
    {
        // Fonts réutilisables
        private static readonly Font TitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 51, 102));
        private static readonly Font SubTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(0, 0, 0));
        private static readonly Font NormalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, new BaseColor(0, 0, 0));
        private static readonly Font HeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
        private static readonly BaseColor HeaderBg = new BaseColor(0, 51, 102);

        /// Rapport détaillé pour une région ou un district.
        public static byte[] GenerateEntityReport(
            string entityType,              // "Région" ou "District"
            string entityName,
            int totalPopulation,
            List<(string Name, int Population)> subEntities, // districts ou communes
            string subEntityLabel           // "Nom du district" ou "Nom de la commune"
        )
        {
            using var stream = new MemoryStream();
            var doc = new Document(PageSize.A4, 50, 50, 80, 50);
            var writer = PdfWriter.GetInstance(doc, stream);
            writer.PageEvent = new PdfPageEvents();
            doc.Open();

            // En‑tête institutionnel
            var header = new Paragraph("MINISTÈRE DE L’EAU ET DU CLIMAT", TitleFont) { Alignment = Element.ALIGN_CENTER };
            doc.Add(header);

            doc.Add(new Paragraph($"Rapport officiel sur le {entityType}", SubTitleFont) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(" ", NormalFont));

            // Informations générales
            var info = new Paragraph($"{entityType} : {entityName}\nPopulation totale : {totalPopulation}", NormalFont)
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };
            doc.Add(info);
            doc.Add(new Paragraph(" ", NormalFont));

            // Tableau des sous‑entités
            if (subEntities != null && subEntities.Count > 0)
            {
                // doc.Add(new Paragraph($"Liste des {subEntityLabel.ToLower()}s :", SubTitleFont));
                doc.Add(new Paragraph($"Liste des {subEntityLabel.ToLower()}s :", SubTitleFont));
                doc.Add(new Paragraph(" ", NormalFont));

                var table = new PdfPTable(2) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 3, 2 });

                table.AddCell(new PdfPCell(new Phrase(subEntityLabel.ToUpper(), HeaderFont)) { BackgroundColor = HeaderBg, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Population", HeaderFont)) { BackgroundColor = HeaderBg, HorizontalAlignment = Element.ALIGN_CENTER });

                bool alternate = false;
                foreach (var item in subEntities)
                {
                    var bg = alternate ? new BaseColor(240, 240, 240) : new BaseColor(255, 255, 255);
                    table.AddCell(new PdfPCell(new Phrase(item.Name, NormalFont)) { BackgroundColor = bg });
                    table.AddCell(new PdfPCell(new Phrase(item.Population.ToString(), NormalFont)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_CENTER });
                    alternate = !alternate;
                }

                doc.Add(table);
            }
            else
            {
                doc.Add(new Paragraph("Aucune donnée disponible.", NormalFont));
            }

            doc.Close();
            return stream.ToArray();
        }

        /// Rapport liste de régions ou de districts.
        public static byte[] GenerateEntitiesListReport<T>(
            string entityType,              // "Région" ou "District"
            List<T> entities,
            Func<T, string> getName,
            Func<T, int> getPopulation,
            Func<T, int> getSubCount,
            string subEntityLabel           // "districts" ou "communes"
        )
        {
            using var stream = new MemoryStream();
            var doc = new Document(PageSize.A4, 50, 50, 80, 50);
            var writer = PdfWriter.GetInstance(doc, stream);
            writer.PageEvent = new PdfPageEvents();
            doc.Open();

            var header = new Paragraph("MINISTÈRE DE L’EAU ET DU CLIMAT", TitleFont) { Alignment = Element.ALIGN_CENTER };
            doc.Add(header);
            doc.Add(new Paragraph($"Rapport officiel des {entityType}s filtrés", SubTitleFont) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(" ", NormalFont));

            var table = new PdfPTable(3) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3, 2, 2 });

            table.AddCell(new PdfPCell(new Phrase($"Nom du {entityType}", HeaderFont)) { BackgroundColor = HeaderBg, HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("Population totale", HeaderFont)) { BackgroundColor = HeaderBg, HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase($"Nombre de {subEntityLabel}", HeaderFont)) { BackgroundColor = HeaderBg, HorizontalAlignment = Element.ALIGN_CENTER });
            bool alternate = false;
            foreach (var e in entities)
            {
                var bg = alternate ? new BaseColor(240, 240, 240) : new BaseColor(255, 255, 255);
                table.AddCell(new PdfPCell(new Phrase(getName(e), NormalFont)) { BackgroundColor = bg });
                table.AddCell(new PdfPCell(new Phrase(getPopulation(e).ToString(), NormalFont)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase(getSubCount(e).ToString(), NormalFont)) { BackgroundColor = bg, HorizontalAlignment = Element.ALIGN_CENTER });
                alternate = !alternate;
            }

            doc.Add(table);
            doc.Close();
            return stream.ToArray();
        }
    }

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
