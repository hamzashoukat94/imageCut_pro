using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;

namespace ShareLove
{
    internal class PdfGenerationService
    {
        private const double MM_TO_PT = 2.83465;
        private const double IMAGE_SIZE_MM = 53; // 55 mm
        private const double IMAGE_SIZE_PT = IMAGE_SIZE_MM * MM_TO_PT; // 55mm in points
        private const double CELL_PADDING = 10;
        private const double SCISSOR_SIZE_PT = 12; // Size of scissor icon in points
        private const double OFFSET_MM = 12; // 12mm
        private const double OFFSET_PT = OFFSET_MM * MM_TO_PT; // 12mm in points
        private const double TOP_MARGIN_MM = 30; // 30mm top margin
        private const double BOTTOM_MARGIN_MM = 20; // 20mm bottom margin
        private const double TOP_MARGIN_PT = TOP_MARGIN_MM * MM_TO_PT; // 30mm in points
        private const double BOTTOM_MARGIN_PT = BOTTOM_MARGIN_MM * MM_TO_PT; // 20mm in points

        // Octagon properties
        private const double MARGIN_TOP_BOTTOM = 25; // Margin above and below the image
        private const double MARGIN_LEFT_RIGHT = 20; // Margin to the left and right of the image
        private const double DIAGONAL_PADDING_PT = 2; // Padding between the image corner and the diagonal

        public void GeneratePdf(List<string> imagePaths, string orderNumber, string filePath)
        {
            try
            {
                // Create PDF document
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4; // Set page size to A4
                XGraphics gfx = XGraphics.FromPdfPage(page);
                double totalCellSpacing = 25; // Total spacing between cells in PT
                double adjustedCellWidth = (page.Width - totalCellSpacing) / 3;

                // Add left margin to the starting point for drawing elements
                double leftMarginPt = 10; // Specify the left margin in points
                double startX = leftMarginPt;
                // Calculate available height
                double availableHeight = page.Height - TOP_MARGIN_PT - BOTTOM_MARGIN_PT;
                double cellHeight = availableHeight / 3;
               

                // Load scissor icon (you need to provide a scissor image path)
                string scissorPath = "resources/scissor.png"; // Replace with your scissor icon path
                XImage scissorIcon = XImage.FromFile(scissorPath);

                // Define a dotted line pen
                XPen dottedPen = new XPen(XColors.Black, 0.5);
                dottedPen.DashStyle = XDashStyle.Dash;

                // Define a blue pen for the octagon
                XPen bluePen = new XPen(XColors.Blue, 0.5);

                // Draw images and order numbers
                for (int i = 0; i < imagePaths.Count; i++)
                {
                    // Calculate position for current cell
                    int row = i / 3;
                    int col = i % 3;

                    double leftX = startX + col * adjustedCellWidth;
                    double scissorX = col * adjustedCellWidth;
                    double y = TOP_MARGIN_PT + row * cellHeight;

                    // Load and draw image
                    XImage image = XImage.FromFile(imagePaths[i]);
                    double imageX = leftX + (adjustedCellWidth - IMAGE_SIZE_PT) / 2;
                    double imageY = y + CELL_PADDING;
                    gfx.DrawImage(image, imageX, imageY, IMAGE_SIZE_PT, IMAGE_SIZE_PT);

                    // Vertices of the octagon surrounding the image
                    XPoint[] octagonPoints = new XPoint[8];
                    double imageLeft = imageX;
                    double imageRight = imageX + IMAGE_SIZE_PT;
                    double imageTop = imageY;
                    double imageBottom = imageY + IMAGE_SIZE_PT;

                    octagonPoints[0] = new XPoint(imageLeft + MARGIN_LEFT_RIGHT - DIAGONAL_PADDING_PT, imageTop - MARGIN_TOP_BOTTOM); // Top-left
                    octagonPoints[1] = new XPoint(imageRight - MARGIN_LEFT_RIGHT + DIAGONAL_PADDING_PT, imageTop - MARGIN_TOP_BOTTOM); // Top-right
                    octagonPoints[2] = new XPoint(imageRight + MARGIN_LEFT_RIGHT, imageTop + MARGIN_TOP_BOTTOM - DIAGONAL_PADDING_PT); // Right-top
                    octagonPoints[3] = new XPoint(imageRight + MARGIN_LEFT_RIGHT, imageBottom - MARGIN_TOP_BOTTOM + DIAGONAL_PADDING_PT); // Right-bottom
                    octagonPoints[4] = new XPoint(imageRight - MARGIN_LEFT_RIGHT + DIAGONAL_PADDING_PT, imageBottom + MARGIN_TOP_BOTTOM); // Bottom-right
                    octagonPoints[5] = new XPoint(imageLeft + MARGIN_LEFT_RIGHT - DIAGONAL_PADDING_PT, imageBottom + MARGIN_TOP_BOTTOM); // Bottom-left
                    octagonPoints[6] = new XPoint(imageLeft - MARGIN_LEFT_RIGHT + DIAGONAL_PADDING_PT, imageBottom - MARGIN_TOP_BOTTOM + DIAGONAL_PADDING_PT); // Left-bottom
                    octagonPoints[7] = new XPoint(imageLeft - MARGIN_LEFT_RIGHT + DIAGONAL_PADDING_PT, imageTop + MARGIN_TOP_BOTTOM - DIAGONAL_PADDING_PT); // Left-top

                    // Draw octagon
                    gfx.DrawPolygon(bluePen, octagonPoints);

                    // Draw order number
                    double textY = imageY + IMAGE_SIZE_PT - 5; // Just beneath the image
                    gfx.DrawString(orderNumber, new XFont("Arial", 6), XBrushes.Black,
                        new XRect(imageX, textY, IMAGE_SIZE_PT, 20), // Adjust height to fit text
                        XStringFormats.Center);

                    // Draw dotted line between rows with scissor icon on the left-most side
                    if (row < 2 && col == 0) // Only draw for the first two rows
                    {
                        double lineY = TOP_MARGIN_PT + (row + 1) * availableHeight / 3 - OFFSET_PT; // 12mm above the middle of the adjacent rows
                        gfx.DrawImage(scissorIcon, scissorX, lineY - SCISSOR_SIZE_PT / 2, SCISSOR_SIZE_PT, SCISSOR_SIZE_PT);
                        gfx.DrawLine(dottedPen, scissorX + SCISSOR_SIZE_PT, lineY, page.Width, lineY); // Dotted line
                    }
                }

                // Save PDF document
                document.Save(filePath);
                document.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                throw; // Rethrow exception for further handling
            }
        }
    }
}
