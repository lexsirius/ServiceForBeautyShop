using System;
using System.Collections.Generic;
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

namespace ReservationDesktop
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            AboutProgram.Text = "Программа предназначена для обслуживания" +
                " и администрирования салонов красоты. С её помощью можно упростить" +
                "процедуру записи в салоны." +
                "\n\n\tУчетная запись: Администратор." +
                "\nПод учетной записью администратора Вы можете добавлять, редактировать" +
                " и удалять учетные записи других пользователей (работников салона)." +
                "\n\n\tУчетная запись: Пользователь/Диспетчер." +
                "\nДиспетчер осуществляет непосредственную работу с функционалом программы," +
                "которая направлена на упрощение его рабочего места." +
                "Пользователь может добавлять, редактировать и удалять записи с данными клиентов.";
            AboutProgram.TextAlignment = TextAlignment.Justify;
        }
    }
}
