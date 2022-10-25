using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tesseract;

namespace Parking_System
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        Car live_car = new Car("spot", "test", 0, 0);
        
        public MainWindow()
        {
            
            InitializeComponent();

            
        }
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Label[] labels = new Label[] { P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18, P19, P20, P21, P22, P23, P24, P25, P26, P27, P28, P29, P30, P31, P32 };
            string tmp_spot;
            live_car.ReadData();
            for (int i = 0; i < live_car.Get_Count(); i++)
            {
                tmp_spot = live_car.Get_default_Sport(i);
                labels[int.Parse(tmp_spot.Substring(1))].BorderBrush = new SolidColorBrush(Colors.Blue);
                labels[int.Parse(tmp_spot.Substring(1))].Background = new SolidColorBrush(Colors.White);
                labels[int.Parse(tmp_spot.Substring(1))].Foreground = new SolidColorBrush(Colors.Blue);
                labels[int.Parse(tmp_spot.Substring(1))].Content = live_car.Get_default_License_plate(i);
            }
        }
        private void into_parking_btn_Click(object sender, RoutedEventArgs e)
        {
            Label[] labels = new Label[] { P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18, P19, P20, P21, P22, P23, P24, P25, P26, P27, P28, P29, P30, P31, P32 }; 
            OpenFileDialog open = new OpenFileDialog();
            string dir;
            string plate="", spot;
            string str_data = "０１２３４５６７８９";
            string str_data1 = "0123456789";
            open.Filter = "PNG파일 (*.PNG, *.png) | *.PNG;*.png;";
            
            list_Status.Items.Clear();
            list_Status.IsEnabled = false;
            leave_parking_btn.IsEnabled = false;
            lbl_title.Content = "";
      
            if(open.ShowDialog() == true)
            {
                dir = open.FileName.ToString();
            }else  return;
            
            Bitmap bitmap = new Bitmap(open.FileName);
            //Pix pix = PixConverter.ToPix(bitmap);
            var ocr = new TesseractEngine("./tessdata", "kor", EngineMode.Default);
            var texts = ocr.Process(bitmap);
            string outText = texts.GetText();
            ocr.Dispose();
            Debug.WriteLine(outText);
            for (int i=0;i<outText.Length;i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if (outText[i] == str_data[j]) plate += str_data1[j];
                    else if (outText[i] == str_data1[j]) plate += str_data1[j];
                   
                }
                if (0xAC00 <= outText[i] && outText[i] <= 0xD7A3) plate += outText[i];
            }
            Input_Text.Text = plate;

            for (int i = 0; i < live_car.Get_Count(); i++)
            {
                if (plate == live_car.Get_default_License_plate(i))
                {
                    MessageBox.Show("이미 입차된 차량입니다.", "오류");
                    return;

                }
            }
            ////////////////////////////////////////////////////////////////////////////////
            live_car.Set_car(plate);
            Random random = new Random();
            List<int> list = new List<int>();
            for(int i=0; i<33;i++)
            {
                if (!(labels[i].Content.ToString()==""))
                {
                    list.Add(i);
                }
            }

            int randomspot;

            randomspot = random.Next(33);
            while (list.Contains(randomspot))
                randomspot = random.Next(33);

            list.Add(randomspot);
            spot = labels[list[list.Count - 1]].Name.ToString();
            live_car.Set_spot(spot);
            Debug.WriteLine(list[list.Count - 1]);
            labels[list[list.Count - 1]].BorderBrush = new SolidColorBrush(Colors.Blue);
            labels[list[list.Count - 1]].Background = new SolidColorBrush(Colors.White);
            labels[list[list.Count - 1]].Foreground = new SolidColorBrush(Colors.Blue);
            labels[list[list.Count - 1]].Content = plate;
            live_car.WriteData();

        }

        private void search_btn_Click(object sender, RoutedEventArgs e)
        {

            string tmp = Input_Text.Text;
            int r_data;
            if (string.IsNullOrEmpty(tmp))
            {
                MessageBox.Show("차량번호를 입력해 주세요", "오류");

            }
            else
            {

                r_data = live_car.ReadData(tmp);
                if (r_data == 1)
                {
                    imagebox.Source = null;
                    list_Status.Items.Clear();
                    list_Status.IsEnabled = true;
                    leave_parking_btn.IsEnabled = false;
                    rate_check_btn.IsEnabled = true;

                    lbl_title.Content = "차량번호\t\t\t입차시간";
                    list_Status.Items.Add(live_car.GetLicense_plate() + "\t\t\t\t" + live_car.Getinto_timeH() + "시" + live_car.Getinto_timeM() + "분");
                }
                else
                {
                    MessageBox.Show("주차된 차량이 없습니다.", "오류");
                }
            }
        }

        private void rate_check_btn_Click(object sender, RoutedEventArgs e)
        {
            imagebox.Source = null;
            list_Status.Items.Clear();
            list_Status.IsEnabled = true;
            leave_parking_btn.IsEnabled = true;
            rate_check_btn.IsEnabled = false;
            lbl_title.Content = "차량번호\t\t\t이용시간\t\t\t이용요금";
            list_Status.Items.Add(live_car.GetLicense_plate() + "\t\t\t\t" + live_car.Getparking_time() + "\t\t\t\t" + live_car.Rate_check());
        }

        private void leave_parking_btn_Click(object sender, RoutedEventArgs e)
        {
            Label[] labels = new Label[] { P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18, P19, P20, P21, P22, P23, P24, P25, P26, P27, P28, P29, P30, P31, P32 };
            string tmp = Input_Text.Text;

            labels[int.Parse(live_car.Get_spot().Substring(1))].BorderBrush = new SolidColorBrush(Colors.White);
            labels[int.Parse(live_car.Get_spot().Substring(1))].Background = null;
            labels[int.Parse(live_car.Get_spot().Substring(1))].Foreground = new SolidColorBrush(Colors.Black);
            labels[int.Parse(live_car.Get_spot().Substring(1))].Content = "";

            list_Status.Items.Clear();
            leave_parking_btn.IsEnabled = false;
            list_Status.IsEnabled = false;
            rate_check_btn.IsEnabled = false;
            live_car.DeleteData(tmp);
        }

        private void end_btn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
