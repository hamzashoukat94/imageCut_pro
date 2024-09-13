using System.Windows;

namespace ShareLove
{
    public partial class LoginWindow : Window
    {
        private const string HardcodedUsername = "admin";
        private const string HardcodedPassword = "sharelove-2024";

        public LoginWindow()
        {
            InitializeComponent();
            UsernameTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == HardcodedUsername && PasswordBox.Password == HardcodedPassword)
            {
                InputWindow inputWindow = new InputWindow();
                inputWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
