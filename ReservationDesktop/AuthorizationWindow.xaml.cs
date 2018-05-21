using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ReservationDesktop
{
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private const string AdminLogin = "kek";
        private const string AdminPassword = "kek";
        private const string UsersFilePath = "users.xml";

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(User[]));
        private readonly User[] users;

        public AuthorizationWindow()
        {
            users = ReadUsers(UsersFilePath);
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == AdminLogin && PasswordTextBox.Password == AdminPassword)
            {
                new AdministrationWindow().Show();
                Close();
                return;
            }

            User attempt = new User(LoginTextBox.Text, PasswordTextBox.Password);
            foreach (var user in users)
            {
                if (attempt == user)
                {
                    new MainWindow().Show();
                    Close();
                    return;
                }
            }

            ErrorLabel.Visibility = Visibility.Visible;
        }

        private User[] ReadUsers(string usersFilePath)
        {
            try
            {
                using (var fileStream = new FileStream(usersFilePath, FileMode.Open))
                {
                    return (User[]) serializer.Deserialize(fileStream);
                }
            }
            catch (Exception e)
            {
                return new User[0];
            }
        }
    }
}
