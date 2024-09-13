using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ShareLove
{
    public partial class InputWindow : Window
    {
        private const int RequiredImageCount = 9;
        private List<string> selectedImages;

        public InputWindow()
        {
            InitializeComponent();
            selectedImages = new List<string>();
            ChooseButton.Focus();
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedImages = openFileDialog.FileNames.ToList();
                ImageCount.Text = $"({selectedImages.Count}) Images selected";
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Hidden;
            string orderNumber = OrderNumberTextBox.Text.Trim();

            if (string.IsNullOrEmpty(orderNumber) && selectedImages.Count != RequiredImageCount)
            { 
                ErrorMessage.Text = "Error: Type in the Order Number and select 9 Images!";
                ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrEmpty(orderNumber))
            {
                ErrorMessage.Text = "Error: Type in the Order Number!";
                ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            if (selectedImages.Count != RequiredImageCount)
            {
                ErrorMessage.Text = "Error: The amount of Images need to be correct = 9!";
                ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            orderNumber = "#" + orderNumber;

            ViewPdfWindow viewPdfWindow = new ViewPdfWindow(selectedImages, orderNumber);
            viewPdfWindow.Show();
            this.Close();
        }
    }
}

