using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;
using ExamDisplay.Annotations;

namespace ExamDisplay
{
    /// <summary>
    /// Interaction logic for Display.xaml
    /// </summary>
    /// 
    


    public partial class Display : Window
    {
        //Binding Data for UI
        DisplayViewModel _boundData = new DisplayViewModel();
        
        
        //internal vars
        private readonly Screens _displays;
        private TimeSpan _duration;
        private TimeSpan _startTime;

        //properties
        public TimeSpan StartTime { get { return _startTime; } set { _startTime = value;  SetTimeDisplay(); } }
        public TimeSpan Duration { get { return _duration; } set { _duration = value; SetTimeDisplay(); } }

        public Display(Screens displays, TimeSpan startTime, TimeSpan duration)
        {
            InitializeComponent();

            this.DataContext = _boundData;

            _displays = displays;
            _duration = duration;
            _startTime = startTime;
            
            ScreenListener();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            //switch to selected screen after window has loaded
            SwitchScreen();
        }

        

        private void ScreenListener()
        {
            _displays.ScreenChange += _displays_ScreenChange;
        }

        private void _displays_ScreenChange(object sender, ScreenEventArgs e)
        {
            SwitchScreen();
        }

        

        private void ResizeFonts(object sender, SizeChangedEventArgs e)
        {
            //get the width and height of the window
            double width = DisplayGrid.ActualWidth;
            double height = DisplayGrid.ActualHeight;

            //CentreDisplay
            CentreDisplay.FontSize = CalcFontSize(48, width, height);
            //ExamDisplay
            ExamDisplay.FontSize = CalcFontSize(48, width, height);
            //TimeDisplay
            TimeDisplay.FontSize = CalcFontSize(148, width, height);
            //DateDisplay
            DateDisplay.FontSize = CalcFontSize(48, width, height);
            //StartDisplay
            StartDisplay.FontSize = CalcFontSize(48, width, height);
            //FinishDisplay
            FinishDisplay.FontSize = CalcFontSize(48, width, height);
            //StartDisplay2
            StartDisplay2.FontSize = CalcFontSize(48, width, height);
            //FinishDisplay2
            FinishDisplay2.FontSize = CalcFontSize(48, width, height);
        }

        private static int CalcFontSize(int start, double width, double height)
        {
            //get ratio
            double ratio = ((width + height) / 2) / (((double)800 + 600) / 2);
            if (ratio < 1)
                ratio = 1;

            int returnFontSize = (int)(ratio * start);
            if (returnFontSize < 48)
                returnFontSize = 48;

            return returnFontSize;
        }

        private static string BuildExamDisplay(string board, string subject, string unit = null)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(board);
            builder.Append(" ");
            builder.Append(subject);

            if (!string.IsNullOrEmpty(unit))
            {
                builder.Append(" (");
                builder.Append(unit);
                builder.Append(")");
            }

            return builder.ToString();
        }

        private void SetTimeDisplay()
        {
            //set display values
            StartDisplay.Content = "Started: " + new DateTime(_startTime.Ticks).ToString("H:mm");
            FinishDisplay.Content = "Finish: " + new DateTime((_startTime.Ticks + _duration.Ticks)).ToString("H:mm");
            StartDisplay2.Content = StartDisplay.Content;
            FinishDisplay2.Content = FinishDisplay.Content;
        }

        private void SwitchScreen()
        {
            this.WindowState = System.Windows.WindowState.Normal;

            System.Drawing.Rectangle r = _displays.SelectedScreen.WorkingArea;
            this.Top = r.Top + 1;
            this.Left = r.Left + 1;

            this.Width = r.Width;
            this.Height = r.Height;

            this.WindowState = System.Windows.WindowState.Maximized;
        }

        public void SetCentreDisplay(string centre)
        {
            CentreDisplay.Content = "Centre: " + centre;
        }

        public void SetExamDisplay(string board, string subject, string unit = null)
        {
            ExamDisplay.Content = BuildExamDisplay(board, subject, unit);
        }
    }
}