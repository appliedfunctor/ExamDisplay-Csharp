using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExamDisplay
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {


        //set up event handler
        public event EventHandler<SettingsEventArgs> ValueChange;
        public event EventHandler<ScreenEventArgs> ScreenChange;

        //brushes
        Brush _selected = Brushes.IndianRed;
        Brush _deSelected = Brushes.Black;
        Brush _themeBlue = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF4F8FB4");

        //internal vars
        TimeSpan _duration = new TimeSpan();
        TimeSpan _startTime = new TimeSpan();

        int _durationHours = 0;
        int _durationMinutes = 0;
        int _startHours = 0;
        int _startMinutes = 0;

        string _unit;
        string _board;
        string _subject;

        string _examDisplay = "Examination";
        string _startDisplay = "Started: ";
        string _finishDisplay = "Finish: ";

        Screen[] _screens = Screen.AllScreens;
        int _selectedScreen = 0;
        Dictionary<int, Border> _uiScreens = new Dictionary<int, Border>();

        //properties
        public int DurationHours { get { return _durationHours; } set { _durationHours = value; this.DurationH.Text = value.ToString(); } }
        public int DurationMinutes { get { return _durationMinutes; } set { _durationMinutes = value; this.DurationM.Text = value.ToString(); } }
        public int StartHours { get { return _startHours; } set { _startHours = value; this.StartH.Text = value.ToString(); } }
        public int StartMinutes { get { return _startMinutes; } set { _startMinutes = value; this.StartM.Text = value.ToString(); } }

        public string Unit { get { return _unit; } set { _unit = value; this.unitSelect.Text = value; } }
        public string Board { get { return _board; } set { _board = value; this.boardSelect.SelectedValue = value; } }
        public string Subject { get { return _subject; } set { _subject = value; this.subjectSelect.SelectedValue = value; } }

        public string ExamDisplay { get { return _examDisplay; } set { _examDisplay = value; } }
        public string StartDisplay { get { return _startDisplay; } set { _startDisplay = value; } }
        public string FinishDisplay { get { return _finishDisplay; } set { _finishDisplay = value; } }

        public Settings()
        {
            InitializeComponent();

            //populate cb
            populateComboBoxes();
            populateScreens();

        }

        public Settings(SettingsEventArgs settings) 
            : this()
        {
            _unit = settings.Unit;
            this.unitSelect.Text = settings.Unit;
            _board = settings.Board;
            this.boardSelect.SelectedValue = settings.Board;
            _subject = settings.Subject;
            this.subjectSelect.SelectedValue = settings.Subject;

            _durationHours = settings.DurationHours;
            this.DurationH.Text = settings.DurationHours.ToString(); 

            _durationMinutes = settings.DurationMinutes;
            this.DurationM.Text = settings.DurationMinutes.ToString();

            _startHours = settings.StartHours;
            this.StartH.Text = settings.StartHours.ToString();
            _startMinutes = settings.StartMinutes;
            this.StartM.Text = settings.StartMinutes.ToString();

            _duration = new TimeSpan(_durationHours, _durationMinutes, 0);
            _startTime = new TimeSpan(_startHours, _startMinutes, 0);

            buildExamDisplay();
            buildTimeDisplay();

        }

        private void populateComboBoxes()
        {
            //Board
            string[] boardSelectSource = new string[] { "AQA", "OCR", "Edexcel", "WJEC", "QCA" };
            boardSelect.ItemsSource = boardSelectSource;
            //boardSelect.SelectedValue = "AQA";
            //_board = boardSelect.SelectedValue.ToString();

            //Subject
            string[] subjectSelectSource = new string[] { "English Language", "English Literature", "Maths A", "Biology", "Chemistry", "Physics", "ICT", "French", "History B", "Geography B", "Business Studies" };
            subjectSelect.ItemsSource = subjectSelectSource;
            //subjectSelect.SelectedValue = "English Language";
            //_subject = subjectSelect.SelectedValue.ToString();

            buildExamDisplay();
        }

        private void durationSet(object sender, TextChangedEventArgs e)
        {
            bool hSuccess = int.TryParse(DurationH.Text, out _durationHours);
            bool mSuccess = int.TryParse(DurationM.Text, out _durationMinutes);

            //grab time h and m
            if (!hSuccess || DurationH.Text.Length > 1 || _durationHours > 4)
            {
                //cull last character
                int length = DurationH.Text.Length - 1;
                if (length < 0) { length = 0; }
                DurationH.Text = DurationH.Text.Substring(0, length);
                int.TryParse(DurationH.Text, out _durationHours);
            }

            if (!mSuccess || DurationM.Text.Length > 2 || _durationMinutes > 59)
            {
                //cull last character
                int length = DurationM.Text.Length - 1;
                if (length < 0) { length = 0; }
                DurationM.Text = DurationM.Text.Substring(0, length);
                int.TryParse(DurationM.Text, out _durationMinutes);
            }

            //set the duration
            _duration = new TimeSpan(_durationHours, _durationMinutes, 0);
            buildTimeDisplay();

        }

        private void startSet(object sender, TextChangedEventArgs e)
        {
            bool hSuccess = int.TryParse(StartH.Text, out _startHours);
            bool mSuccess = int.TryParse(StartM.Text, out _startMinutes);

            //grab time h and m
            if (!hSuccess || StartH.Text.Length > 2 || _startHours > 24)
            {
                //cull last character
                int length = StartH.Text.Length - 1;
                if (length < 0) { length = 0; }
                StartH.Text = StartH.Text.Substring(0, length);
                int.TryParse(StartH.Text, out _startHours);
            }

            if (!mSuccess || StartM.Text.Length > 2 || _startMinutes > 59)
            {
                //cull last character
                int length = StartM.Text.Length - 1;
                if (length < 0) { length = 0; }
                StartM.Text = StartM.Text.Substring(0, length);
                int.TryParse(StartM.Text, out _startMinutes);
            }

            //set the duration
            _startTime = new TimeSpan(_startHours, _startMinutes, 0);
            buildTimeDisplay();

        }

        private void setUnit(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox)
            {
                var senderBox = sender as System.Windows.Controls.TextBox;
                _unit = senderBox.Text;
                buildExamDisplay();
            }
        }


        private void buildExamDisplay()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(_board);
            builder.Append(" ");
            builder.Append(_subject);
            if (!String.IsNullOrEmpty(_unit))
            {
                builder.Append(" (");
                builder.Append(_unit);
                builder.Append(")");
            }

            _examDisplay = builder.ToString();

            //notify event listener of change
            fireEvent();


        }

        private void fireEvent()
        {
            var settingsArgs = new SettingsEventArgs() { 
                                                        ExamDisplay = _examDisplay, 
                                                        StartDisplay = _startDisplay, 
                                                        FinishDisplay = _finishDisplay,
                                                        Duration = _duration,
                                                        StartTime = _startTime,
                                                        DurationHours = _durationHours,
                                                        DurationMinutes = _durationMinutes,
                                                        StartHours = _startHours,
                                                        StartMinutes = _startMinutes,
                                                        Unit = _unit,
                                                        Board = _board,
                                                        Subject = _subject
                                                        };

            if (ValueChange != null)
                ValueChange(this, settingsArgs);
        }

        private void buildTimeDisplay()
        {
            
            if (_startTime != null)
                _startDisplay = "Started: " + new DateTime(_startTime.Ticks).ToString("H:mm");

            if (_duration != null && _startTime != null)
                _finishDisplay = "Finish: " + new DateTime((_startTime.Ticks + _duration.Ticks)).ToString("H:mm");

            //notify event listener of change
            fireEvent();
            
        }

        private void boardChange(object sender, SelectionChangedEventArgs e)
        {
            if (boardSelect.SelectedValue != null)
            {
                _board = boardSelect.SelectedValue.ToString();
                buildExamDisplay();
            }
        }

        private void subjectChange(object sender, SelectionChangedEventArgs e)
        {
            if (subjectSelect.SelectedValue != null)
            {
                _subject = subjectSelect.SelectedValue.ToString();
                buildExamDisplay();
            }
        }


        private void populateScreens()
        {
            int i = 1;
            Screen primary = Screen.PrimaryScreen;

            foreach(Screen screen in _screens){
                Border bdr = new Border();
                if (primary == screen)
                {
                    bdr.Background = Brushes.White;
                    bdr.BorderBrush = _selected;
                }
                else
                {
                    bdr.Background = _themeBlue;
                    bdr.BorderBrush = _deSelected;
                }
                
                bdr.BorderBrush = Brushes.Black;
                bdr.BorderThickness = new Thickness(2);
                bdr.SnapsToDevicePixels = true;
                bdr.Width = 35;
                bdr.Height = 35;
                bdr.CornerRadius = new CornerRadius(5);
                bdr.MouseDown += screenSelect_MouseDown;
                bdr.Name = "screen_" + (i-1).ToString();

                var tb = new System.Windows.Controls.TextBlock();
                tb.Text = i.ToString();
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.TextAlignment = TextAlignment.Center;
                tb.Background = Brushes.Transparent;
                tb.FontSize = 20;
                tb.FontWeight = FontWeights.DemiBold;
                //tb.MouseDown += screenSelect_MouseDown;
                //tb.Name = "screen_" + i.ToString();

                bdr.Child = tb;

                ScreenDisplay.Children.Add(bdr);
                //add border to array
                _uiScreens[i] = bdr;
                i++;

            }
            dv.Content = "Screens: " + _screens.Count() + ":: [" + (_selectedScreen + 1) + "]";
        }

        private int getScreenName(string name)
        {
            int screenNumber;
            string numericalName = name.Substring(7);
            //System.Windows.MessageBox.Show("numericalName: " + numericalName);
            int.TryParse(numericalName, out screenNumber);

            if (screenNumber != null)
            {
                return screenNumber;
            }
            else
            {
                return -1;
            }
        }

        private void screenSelect_MouseDown(object sender, MouseButtonEventArgs e)
        {

            int selectedScreen = -1;
            
            //select screen
            if (sender is Border)
            {
                var bdr = sender as Border;
                selectedScreen = getScreenName(bdr.Name);
                //System.Windows.MessageBox.Show("selectedScreen: " + selectedScreen);
  
            }
            //else if (sender is System.Windows.Controls.TextBlock)
            //{
            //    var tb = sender as System.Windows.Controls.TextBlock;
            //    selectedScreen = getScreenName(tb.Name);
            //    System.Windows.MessageBox.Show("selectedScreen: " + selectedScreen);
            //}

            if (selectedScreen != -1)
            {
                updateSelectedScreenUI(oldSelected: _selectedScreen, newSelected: selectedScreen);
                //set selected screen
                _selectedScreen = selectedScreen;

                dv.Content = "Screens: " + _screens.Count() + ":: [" + (_selectedScreen+1) + "]";

                //broadcast screenchange
                if (ScreenChange != null)
                    ScreenChange(this, new ScreenEventArgs() { SelectedScreen = _selectedScreen });
            }

        }

        private void updateSelectedScreenUI(int oldSelected, int newSelected)
        {
            //switch old screen back to deselected
            Border oldBdr = _uiScreens[oldSelected];
            oldBdr.BorderBrush = _deSelected;
            oldBdr.Background = Brushes.White;
            //switch new screen to selected
            Border newBdr = _uiScreens[newSelected];
            newBdr.BorderBrush = _selected;
            newBdr.Background = _themeBlue;

        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            //Do whatever you want before calling close..
            base.OnClosing(e);
        }

    }
}
