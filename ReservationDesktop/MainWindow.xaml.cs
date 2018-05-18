using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using ReservationClass;

namespace ReservationDesktop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private class Service
        {
            public string Name { get; }
            public string Type { get; }
            public bool IsChecked { get; set; }

            public Service(string name, string type)
            {
                Name = name;
                Type = type;
                IsChecked = false;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private ObservableCollection<string> makeupArtists = new ObservableCollection<string>
        {
            "Парикмахер-колорист Ольга",
            "Парикмахер-колорист Марина",
            "Визажист Татьяна",
            "Мастер ногтевого сервиса Юлия",
            "Косметолог Кристина",
            "Бровист Вероника",
            "Массажист Маргарита",
            "Мастер ногтевого сервиса Мария"
        };

        private ObservableCollection<string> ReservationTimes = new ObservableCollection<string>
        {
            "10:00",
            "10:30",
            "11:00",
            "11:30",
            "12:00",
            "12:30",
            "13:00",
            "13:30",
            "14:00",
            "14:30",
            "15:00",
            "15:30",
            "16:00",
            "16:30",
            "17:00",
            "17:30",
            "18:00",
            "18:30",
            "19:00",
            "19:30",
            "20:00",
            "20:30"
        };

        private static readonly string cosmetologist = "Косметология";
        private static readonly string haircut = "Услуги парикмахера";
        private static readonly string manicure = "Маникюр";
        private static readonly string visage = "Услуги визажиста";
        private static readonly string massage = "Массаж";

        private ObservableCollection<string> serviceTypes = new ObservableCollection<string>
        {
            cosmetologist,
            haircut,
            manicure,
            visage,
            massage
        };

        private readonly List<Service> services = new List<Service>
        {
            new Service("Стрижка", haircut),
            new Service("Покраска", haircut),
            new Service("Маникюр обрезной", manicure),
            new Service("Покрытие", manicure),
            new Service("Чистка лица", cosmetologist),
            new Service("Уход за кожей лица", cosmetologist),
            new Service("Вечерний макияж", visage),
            new Service("Дневной макияж", visage),
            new Service("Оформление бровей", visage),
            new Service("Общий массаж всего тела 60 мин", massage),
            new Service("Спина + шея", massage)
        };

        private ObservableCollection<Service> availableServices;

        private ObservableCollection<Reservation> reservations
            = new ObservableCollection<Reservation>();

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(Reservation[]));

        public MainWindow()
        {
            InitializeComponent();
            ArtistsBox.ItemsSource = makeupArtists;
            TimeBox.ItemsSource = ReservationTimes;
            ServiceTypeBox.ItemsSource = serviceTypes;
            ReservationsList.ItemsSource = reservations;
        }

        private void ServiceTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Service service in services)
            {
                service.IsChecked = false;
            }

            availableServices = new ObservableCollection<Service>(
                services.Where(service =>
                    service.Type == (string)ServiceTypeBox.Items[ServiceTypeBox.SelectedIndex]));
            ServicesListBox.ItemsSource = availableServices;
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            reservations.Add(new Reservation(
                NameBox.Text,
                PhoneNumberBox.Text,
                GetDateTime(),
                new List<string>(
                    availableServices
                        .Where(service => service.IsChecked)
                        .Select(service => service.Name)),
                (string)ArtistsBox.Items[ArtistsBox.SelectedIndex]
            ));
        }

        private DateTime GetDateTime()
        {
            DateTime dateTime = (DateTime)DateBox.SelectedDate;
            string time = (string)TimeBox.Items[TimeBox.SelectedIndex];
            return dateTime.AddMinutes(
                Double.Parse(time.Substring(3)) +
                60 * Double.Parse(time.Substring(0, 2)));
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = "Файлы записей|*.xml",
                FileName = "Записи"
            };
            dlg.ShowDialog();

            using (var fileStream = dlg.OpenFile())
            {
                serializer.Serialize(fileStream, reservations.ToArray());
            }
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "Файлы записей|*.xml",
                FileName = "Записи"
            };
            dlg.ShowDialog();

            using (var fileStream = dlg.OpenFile())
            {
                Reservation[] reservationsInFile = (Reservation[])serializer.Deserialize(fileStream);
                foreach (var reservation in reservationsInFile)
                {
                    reservations.Add(reservation);
                }
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ReservationsList.SelectedIndex >= 0)
            {
                reservations.RemoveAt(ReservationsList.SelectedIndex);
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            Add.IsEnabled = false;
            var reserv = ReservationsList.SelectedItem as Reservation;

            //var serv = ReservationsList.SelectedItem as Service;
            if (reserv == null)
                return;
            else
            {
                NameBox.Text = reserv.ClientName;
                PhoneNumberBox.Text = reserv.PhoneNumber;
                DateBox.DisplayDate = reserv.ReservationDateTime;
                DateBox.Text = reserv.ReservationDate;
                TimeBox.SelectedItem = reserv.ReservationTime;
                //ServiceTypeBox
                //ServicesListBox
                ArtistsBox.SelectedItem = reserv.MakeupArtist;
                ServicesListBox.SelectedItem = reserv.Services;
            }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            var reservIndex = ReservationsList.SelectedIndex;
            reservations.RemoveAt(reservIndex);
            reservations.Insert(reservIndex,
                new Reservation(
                NameBox.Text,
                PhoneNumberBox.Text,
                GetDateTime(),
                new List<string>(
                    availableServices
                        .Where(service => service.IsChecked)
                        .Select(service => service.Name)),
                (string)ArtistsBox.Items[ArtistsBox.SelectedIndex]
            ));
            Add.IsEnabled = true;
        }
    }
}
