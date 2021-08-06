using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ServiceRequest_App
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Request> Requests;
        public bool PhoneWindowOpened = false;
        bool allowSelection;

        public MainWindow()
        {
            InitializeComponent();
            if (!HelperMethods.IsAdministrator())
            {
                MessageBox.Show("Для правильной работы приложения запустите программу от имени администратора.");
            }
            //Загрузка списка заявок
            RefreshRequestList();
            //Информационное сообщение
            HelloText.Text = "Создайте новую заявку и мы решим вашу проблему.\nВо вкладке списка номеров, вы можете найти номер требуемого человека.";
            //Окно просмотра информации о заявке
            BorderForCheck.Visibility = Visibility.Hidden;
            //Окно для сообщения
            BorderForMessage.Visibility = Visibility.Hidden;
            //Закрыть
            ButBorder_Delete.Visibility = Visibility.Hidden;
            //Отображение Таблицы с днями неделями
            WorkGraphic.Visibility = Visibility.Hidden;
            //Поле для ввода пароля
            PasswordPanel.Visibility = Visibility.Hidden;
            //Дни недели
            Monday.IsChecked = true;
            Tuesday.IsChecked = true;
            Wednesday.IsChecked = true;
            Thursday.IsChecked = true;
            Friday.IsChecked = true;
            //Время
            TimeFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            TimeTo.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0);
            //Рычаг для обновления данных
            allowSelection = true;
            //Получение информации о пк
            try
            {
                ipv4.Content = "IP: " + HelperMethods.GetIPv4();
                hostName.Content = "Host: " + Dns.GetHostName();
            }
            catch
            {
                MessageBox.Show("Невозможно получить IPv4 или MAC");
            }
        }

        //Просмотр заявки
        private void phonesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allowSelection)
            {
                CloseAndClearTextBox();
                ButBorder_Delete.Visibility = Visibility.Visible;
                Request r = (Request)RequestsList.SelectedItem;
                if (BorderForCheck.Visibility == Visibility.Hidden)
                {
                    BorderForCheck.Visibility = Visibility.Visible;
                }
                RequestNumberOnForm.Content = r.Number;
                StatusOnForm.Content = r.Status;
                UserNameOnForm.Content = r.UserName;
                MobileOnForm.Content = r.Mobile;
                EmailOnForm.Content = r.Email;
                AreaOnForm.Content = r.Location;
                if (r.ProblemWithMyPc)
                { ProblemWithMyPcOnForm.Content = "Да"; }
                else
                { ProblemWithMyPcOnForm.Content = "Нет"; }
                if (r.WithoutMe == "1")
                { WithMePcOnForm.Content = "Нет"; }
                else if (r.WithoutMe == "0")
                { WithMePcOnForm.Content = "Да"; }
                else
                {
                    WithMePcOnForm.Content = "Неизвестно";
                }
                if (r.WorkTime.Length > 1)
                {
                    CreateMessageOnForn.Text = r.TextOfRequest + "\n\nВремя работы:\n" + r.WorkTime;
                }
                else
                {
                    CreateMessageOnForn.Text = r.TextOfRequest;
                }
            }
        }

        private void ExitButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void MinButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        //Обновить
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            allowSelection = false;
            Requests.Clear();
            RefreshRequestList();
            allowSelection = true;
        }

        private void CloseAndClearTextBox()
        {
            BorderForMessage.Visibility = Visibility.Hidden;
            ButBorder_Create.Visibility = Visibility.Visible;
            ButBorder_Delete.Visibility = Visibility.Hidden;
            userName.Clear();
            userPhone.Clear();
            Area.Clear();
            Email.Clear();
            TimeFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
            TimeTo.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0);
            solutionOne.IsChecked = false;
            solutionTwo.IsChecked = false;
            withMyPc.IsChecked = false;
            Password.Clear();
            CreateMessage.Clear();
            Monday.IsChecked = true;
            Tuesday.IsChecked = true;
            Wednesday.IsChecked = true;
            Thursday.IsChecked = true;
            Friday.IsChecked = true;
            Saturday.IsChecked = false;
            Sunday.IsChecked = false;
            PasswordPanel.Visibility = Visibility.Hidden;
            WorkGraphic.Visibility = Visibility.Hidden;
        }

        //Отправка сообщения
        private void ButtonSend_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Area.Text.Length != 0 && CreateMessage.Text.Length != 0)
            {
                Request requestData = new Request();
                requestData.Date = DateTime.Now.ToString("MM/dd/yy");
                requestData.UserName = userName.Text;
                requestData.Location = Area.Text;
                requestData.Mobile = userPhone.Text;
                requestData.Email = Email.Text;
                if (solutionTwo.IsChecked.Value)
                {
                    requestData.WithoutMe = "1";
                }
                else if (solutionOne.IsChecked.Value)
                {
                    requestData.WithoutMe = "0";
                }
                else
                {
                    requestData.WithoutMe = null;
                }
                if (withMyPc.IsChecked.Value)
                {
                    requestData.ProblemWithMyPc = withMyPc.IsChecked.Value;
                }
                if (solutionOne.IsChecked.Value || solutionTwo.IsChecked.Value)
                {
                    requestData.WorkTime = HelperMethods.GetWorkTime(this);
                }
                requestData.TextOfRequest = CreateMessage.Text;
                requestData.Status = "В работе";
                bool check = DataBaseMethods.SendRequest(this, requestData);
                if (check)
                {
                    CloseAndClearTextBox();
                    MessageBox.Show("Заявка успешно отправлена.");
                    allowSelection = false;
                    Requests.Clear();
                    RefreshRequestList();
                    allowSelection = true;
                }
            }
            else
            {
                MessageBox.Show("Заполните обязательные поля для отправки. Достаточно указать местоположение и описание проблемы.", "Обязательные поля не заполнены");
            }
        }

        //Очистка сообщения или удаление заявки.
        private void ButtonDelete_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CreateMessage.Visibility == Visibility.Visible)
            {
                CloseAndClearTextBox();
            }
            if (BorderForCheck.Visibility == Visibility.Visible)
            {
                ButBorder_Delete.Visibility = Visibility.Hidden;
                BorderForCheck.Visibility = Visibility.Hidden;
                allowSelection = false;
                RequestsList.SelectedItem = null;
                allowSelection = true;
            }
        }

        private void solutionOne_Click(object sender, RoutedEventArgs e)
        {
            if (solutionTwo.IsChecked.Value)
            {
                solutionTwo.IsChecked = false;
            }
            if (solutionOne.IsChecked.Value)
            {
                WorkGraphic.Visibility = Visibility.Visible;
            }
            if (!solutionOne.IsChecked.Value && !solutionTwo.IsChecked.Value)
            {
                WorkGraphic.Visibility = Visibility.Hidden;
            }

        }

        private void solutionTwo_Click(object sender, RoutedEventArgs e)
        {
            if (solutionOne.IsChecked.Value)
            {
                solutionOne.IsChecked = false;
            }
            if (solutionTwo.IsChecked.Value)
            {
                WorkGraphic.Visibility = Visibility.Visible;
            }
            if (!solutionOne.IsChecked.Value && !solutionTwo.IsChecked.Value)
            {
                WorkGraphic.Visibility = Visibility.Hidden;
            }
        }

        //Создать новую заявку
        private void ButtonCreate_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            allowSelection = false;
            RequestsList.SelectedItem = null;
            allowSelection = true;
            BorderForCheck.Visibility = Visibility.Hidden;
            BorderForMessage.Visibility = Visibility.Visible;
            ButBorder_Create.Visibility = Visibility.Hidden;
            ButBorder_Delete.Visibility = Visibility.Visible;
        }

        //Номера Телефонов
        private void ButtonPhone_Click(object sender, RoutedEventArgs e)
        {

            if (!PhoneWindowOpened)
            {
                PhoneWindow phoneWindow = new PhoneWindow(this);
                phoneWindow.Owner = this;
                phoneWindow.Show();
                PhoneWindowOpened = true;
            }
        }

        private void withMyPc_Click(object sender, RoutedEventArgs e)
        {
            if (withMyPc.IsChecked.Value)
            {
                PasswordPanel.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordPanel.Visibility = Visibility.Hidden;
            }
        }

        public void RefreshRequestList()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            try
            {
                conn.Open();
                string sql = @"SELECT mainrequestinfo.RequestNumber, DateCreate, STATUS, UserName, Location, Mobile, Email, WithoutMe, ProblemWithMyPc, WorkTime, Text, Comments   FROM mainrequestinfo JOIN pcinfo ON mainrequestinfo.RequestNumber = pcinfo.RequestNumber WHERE pcinfo.MacAddress = """ + HelperMethods.GetBeautiMacAddress() + @""";";
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int count = 1;
                        Requests = new ObservableCollection<Request>();
                        while (reader.Read())
                        {
                            string RequestNumber = reader.GetString(0);
                            string Date = reader.GetString(1);
                            string Status = reader.GetString(2);
                            string UserName = "";
                            if (!reader.IsDBNull(reader.GetOrdinal("UserName")))
                            {
                                UserName = reader.GetString(3);
                            }
                            string Location = reader.GetString(4);
                            string Mobile = "";
                            if (!reader.IsDBNull(reader.GetOrdinal("Mobile")))
                            {
                                Mobile = reader.GetString(5);
                            }
                            string Email = "";
                            if (!reader.IsDBNull(reader.GetOrdinal("Email")))
                            {
                                Email = reader.GetString(6);
                            }
                            string WithoutMe = "";
                            if (!reader.IsDBNull(reader.GetOrdinal("WithoutMe")))
                            {
                                WithoutMe = reader.GetString(7);
                            }
                            bool ProblemWithMyPc = reader.GetBoolean(8);
                            string WorkTime = "";
                            if (!reader.IsDBNull(reader.GetOrdinal("WorkTime")))
                            {
                                WorkTime = reader.GetString(9);
                            }
                            string Text = reader.GetString(10);
                            string Comments = "";
                            if (!reader.IsDBNull(reader.GetOrdinal("Comments")))
                            {
                                Comments = reader.GetString(11);
                            }
                            string imagePath = "";
                            if (Status == "В работе")
                            {
                                imagePath = "source/inwork.png";
                            }
                            else if (Status == "Завершено")
                            {
                                imagePath = "source/complete.png";
                            }
                            else
                            {
                                imagePath = "source/cansel.png";
                            }
                            Requests.Add(new Request
                            {
                                Id = count,
                                ImagePath = imagePath,
                                Number = RequestNumber,
                                Date = Date,
                                Status = Status,
                                UserName = UserName,
                                Location = Location,
                                Mobile = Mobile,
                                Email = Email,
                                WithoutMe = WithoutMe,
                                ProblemWithMyPc = ProblemWithMyPc,
                                WorkTime = WorkTime,
                                TextOfRequest = Text,
                                //Coments = Comments
                            });
                            count++;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Невозможно подключиться к базе данных: " + e.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                RequestsList.ItemsSource = Requests;
            }
        }


    }
}
