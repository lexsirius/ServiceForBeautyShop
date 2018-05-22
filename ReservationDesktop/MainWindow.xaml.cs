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
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

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
            try
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
            catch (Exception)
            {
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ReservationsList.SelectedIndex >= 0)
            {
                reservations.RemoveAt(ReservationsList.SelectedIndex);
            }
        }

        private int editIndex = -1;
        private bool editMode = false;

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
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
            var index = ReservationsList.SelectedIndex;
            if (index >= 0)
            {
                //ErrorLabel.Visibility = Visibility.Hidden;
                AddButton.IsEnabled = DeleteButton.IsEnabled = false;
                FinishButton.IsEnabled = true;
                EditButton.Content = "Отмена";

                var reserv = reservations[index];
                NameBox.Text = reserv.ClientName;
                PhoneNumberBox.Text = reserv.PhoneNumber;
                //DateBox.DisplayDate = reserv.ReservationDateTime;
                DateBox.SelectedDate = reserv.ReservationDateTime;
                //DateBox.Text = reserv.ReservationDate;
                TimeBox.SelectedItem = reserv.ReservationTime;
                ArtistsBox.SelectedItem = reserv.MakeupArtist;
                ServicesListBox.SelectedItem = reserv.Services;
                ServiceTypeBox.SelectedItem = GetServiceType(reserv.ServiceList[0]);
                ServiceTypeBox_SelectionChanged(null, null);
                foreach (var service in reserv.ServiceList)
                {
                    foreach (var availableService in availableServices)
                    {
                        if (availableService.Name == service)
                        {
                            availableService.IsChecked = true;
                        }
                    }
                }

                editIndex = index;
                editMode = true;
            }
        }

        private object GetServiceType(string service)
        {
            foreach (var existingService in services)
            {
                if (service == existingService.Name)
                {
                    return existingService.Type;
                }
            }

            return null;
        }

        private void DisableEdit()
        {
            //ErrorLabel.Visibility = Visibility.Hidden;
            AddButton.IsEnabled = DeleteButton.IsEnabled = true;
            FinishButton.IsEnabled = false;
            EditButton.Content = "Изменить";
            editIndex = -1;
            editMode = false;
        }

        private void Button_Finish_Click(object sender, RoutedEventArgs e)
        {
            reservations[editIndex] = new Reservation(
                NameBox.Text,
                PhoneNumberBox.Text,
                GetDateTime(),
                new List<string>(
                    availableServices
                        .Where(service => service.IsChecked)
                        .Select(service => service.Name)),
                (string)ArtistsBox.Items[ArtistsBox.SelectedIndex]
            );
            DisableEdit();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().Show();
        }

        private void MenuItem_Load_Click(object sender, RoutedEventArgs e)
        {
            Button_Load_Click(sender, e);
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            Button_Save_Click(sender, e);
        }


        private Word.Application WordApp;
        private Word.Document Doc;
        private void MenuItem_SaveWord_Click(object sender, RoutedEventArgs e)
        {
            WordApp = new Word.Application();
            WordApp.Visible = true;
            WordApp.Documents.Add();
            Doc = WordApp.Documents[1];
            var rowsCount = reservations.Count + 1;
            var wordRange = Doc.Range(0, 0);
            var wordTable = Doc.Tables.Add(wordRange, rowsCount, 6);
            wordTable.Cell(1, 1).Range.Text = "Время";
            wordTable.Cell(1, 2).Range.Text = "Дата";
            wordTable.Cell(1, 3).Range.Text = "Мастер";
            wordTable.Cell(1, 4).Range.Text = "Услуга";
            wordTable.Cell(1, 5).Range.Text = "Клиент";
            wordTable.Cell(1, 6).Range.Text = "Контактный номер";
            for (var i = 0; i < rowsCount - 1; i++)
            {
                wordTable.Cell(i + 2, 1).Range.Text = reservations[i].ReservationTime;
                wordTable.Cell(i + 2, 2).Range.Text = reservations[i].ReservationDate;
                wordTable.Cell(i + 2, 3).Range.Text = reservations[i].MakeupArtist;
                wordTable.Cell(i + 2, 4).Range.Text = reservations[i].Services;
                wordTable.Cell(i + 2, 5).Range.Text = reservations[i].ClientName;
                wordTable.Cell(i + 2, 6).Range.Text = reservations[i].PhoneNumber;
            }

            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = ".docx",
                    Filter = "Файлы записей в формате Word|*.docx",
                    FileName = "Документ Word"
                };
                dlg.ShowDialog();

                using (var fileStream = dlg.OpenFile())
                {
                    Doc.SaveAs(fileStream);
                }
            }
            catch (Exception)
            {
            }

        }

        private Excel.Application ExcelApp;
        private Excel.Worksheet ExcelWorksheet;
        private void MenuItem_SaveExсel_Click(object sender, RoutedEventArgs e)
        {
            ExcelApp = new Excel.Application();
            ExcelApp.Visible = true;
            ExcelApp.SheetsInNewWorkbook = 1;
            ExcelApp.Workbooks.Add();
            ExcelWorksheet = ExcelApp.Workbooks[1].Worksheets.Item[1];

            ExcelWorksheet.Range["A1"].Value = "Время";
            ExcelWorksheet.Range["B1"].Value = "Дата";
            ExcelWorksheet.Range["C1"].Value = "Мастер";
            ExcelWorksheet.Range["D1"].Value = "Услуга";
            ExcelWorksheet.Range["E1"].Value = "Клиент";
            ExcelWorksheet.Range["F1"].Value = "Контактный номер";
            for (var i = 0; i < reservations.Count; i++)
            {
                ExcelWorksheet.Cells[i + 2, 1].Value = reservations[i].ReservationTime;
                ExcelWorksheet.Cells[i + 2, 2].Value = reservations[i].ReservationDate;
                ExcelWorksheet.Cells[i + 2, 3].Value = reservations[i].MakeupArtist;
                ExcelWorksheet.Cells[i + 2, 4].Value = reservations[i].Services;
                ExcelWorksheet.Cells[i + 2, 5].Value = reservations[i].ClientName;
                ExcelWorksheet.Cells[i + 2, 6].Value = reservations[i].PhoneNumber;
            }
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = ".xlsx",
                    Filter = "Файлы записей в формате Excel|*.xlsx",
                    FileName = "Таблица Excel"
                };
                dlg.ShowDialog();

                using (var fileStream = dlg.OpenFile())
                {
                    ExcelApp.Workbooks[1].SaveAs(fileStream);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
