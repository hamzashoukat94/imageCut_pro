using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using static PdfSharp.Pdf.PdfDictionary;

namespace ShareLove
{
    public partial class ViewPdfWindow : Window
    {
        private string tempPdfPath;
        private string orderNumber;

        public ViewPdfWindow(List<string> selectedImages, string orderNumber)
        {
            InitializeComponent();
            this.orderNumber = orderNumber;

            var pdfGenerationService = new PdfGenerationService();
            // Save PDF stream to a temporary file
            tempPdfPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");

            pdfGenerationService.GeneratePdf(selectedImages, orderNumber, tempPdfPath);
            // Load PDF file into WebView2 control
            //LoadPdf(tempPdfPath);
            InitializeWebView(tempPdfPath);
        }


        private async void InitializeWebView(string pdfFilePath)
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);

                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

                webView.Source = new Uri(pdfFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // JavaScript to hide the default PDF viewer header
            string script = @"
                document.addEventListener('DOMContentLoaded', function() {
                    var interval = setInterval(function() {
                        var toolbar = document.querySelector('[role=""toolbar""]');
                        if (toolbar) {
                            toolbar.style.display = 'none';
                            clearInterval(interval);
                        }
                    }, 100);
                });
            ";

            await webView.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void LoadPdf(string pdfFilePath)
        {
            try
            {
                // Navigate WebView2 to the PDF file path

                webView.Source = new Uri(pdfFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading PDF: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Clean up the temporary PDF file
            if (!string.IsNullOrEmpty(tempPdfPath) && File.Exists(tempPdfPath))
            {
                try
                {
                    File.Delete(tempPdfPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error cleaning up temporary files: {ex.Message}");
                }
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            // Implement printing logic
            webView.ExecuteScriptAsync("window.print();");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Implement saving logic
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"{orderNumber}.pdf"; // Default filename based on order number
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.Copy(tempPdfPath, saveFileDialog.FileName, true);
                    MessageBox.Show($"PDF saved successfully to {saveFileDialog.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving PDF: {ex.Message}");
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            InputWindow inputWindow = new InputWindow();
            inputWindow.Show();
            this.Close();
        }
    }
}
