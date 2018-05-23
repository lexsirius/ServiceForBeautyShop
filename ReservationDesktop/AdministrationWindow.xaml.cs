using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for AdministrationWindow.xaml
    /// </summary>
    public partial class AdministrationWindow : Window
    {
        private const string UsersFilePath = "users.xml";

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(User[]));

        private readonly ObservableCollection<User> users;

        public AdministrationWindow()
        {
            users = new ObservableCollection<User>(ReadUsers(UsersFilePath));
            InitializeComponent();
            UsersList.ItemsSource = users;
        }

        private User[] ReadUsers(string usersFilePath)
        {
            try
            {
                using (var fileStream = new FileStream(usersFilePath, FileMode.Open))
                {
                    return (User[])serializer.Deserialize(fileStream);
                }
            }
            catch (Exception e)
            {
                return new User[0];
            }
        }

        private void WriteUsers(string usersFilePath)
        {
            try
            {
                using (var fileStream = new FileStream(usersFilePath, FileMode.Create))
                {
                    serializer.Serialize(fileStream, users.ToArray());
                }
            }
            catch (Exception e)
            {
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            WriteUsers(UsersFilePath);
            SavedLabel.Visibility = Visibility.Visible;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SavedLabel.Visibility = Visibility.Hidden;
            ErrorLabel.Visibility = Visibility.Hidden;
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            if (login != "" && password != "")
            {
                if (LoginInList(login))
                {
                    ErrorLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    users.Add(new User(login, password).EncryptPassword());

                    LoginTextBox.Text = "";
                    PasswordTextBox.Text = "";
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            SavedLabel.Visibility = Visibility.Hidden;
            var index = UsersList.SelectedIndex;
            if (index >= 0)
            {
                ErrorLabel.Visibility = Visibility.Hidden;
                users.RemoveAt(index);
            }
        }

        private int editedIndex = -1;
        private bool editMode = false;

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SavedLabel.Visibility = Visibility.Hidden;
            if (editMode)
            {
                DisableEdit();
            }
            else
            {
                EnableEdit();
            }
        }

        private void EnableEdit()
        {
            var index = UsersList.SelectedIndex;
            if (index >= 0)
            {
                ErrorLabel.Visibility = Visibility.Hidden;
                AddButton.IsEnabled = RemoveButton.IsEnabled = false;
                FinishButton.IsEnabled = true;
                EditButton.Content = "Отмена";
                LoginTextBox.Text = users[index].Login;
                PasswordTextBox.Text = users[index].Password;
                editedIndex = index;
                editMode = true;
            }
        }

        private void DisableEdit()
        {
            ErrorLabel.Visibility = Visibility.Hidden;
            AddButton.IsEnabled = RemoveButton.IsEnabled = true;
            FinishButton.IsEnabled = false;
            EditButton.Content = "Изменить";
            editedIndex = -1;
            editMode = false;
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            SavedLabel.Visibility = Visibility.Hidden;
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            if (login != "" && password != "")
            {
                users[editedIndex] = new User(login, password).EncryptPassword();
                DisableEdit();
            }
        }

        private bool LoginInList(string login)
        {
            foreach (var user in users)
            {
                if (user.Login == login)
                {
                    return true;
                }
            }

            return false;
        }


    }
}
