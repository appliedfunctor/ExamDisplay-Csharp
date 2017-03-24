using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ExamDisplay.Properties;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using TextBox = System.Windows.Controls.TextBox;

namespace ExamDisplay
{
    public partial class MainWindow : Window
    {
        //set up event handler
        //public event EventHandler<SettingsEventArgs> ValueChange;
        //public event EventHandler<ScreenEventArgs> ScreenChange;

        //settings timer
        DispatcherTimer settingsIoTimer = new DispatcherTimer();

        //data
        public DataIo SettingsIoData = new DataIo();

        //internal vars
        TimeSpan _duration;
        TimeSpan _startTime;

        int _durationHours;
        int _durationMinutes;
        int _startHours;
        int _startMinutes;

        string _centre;
        string _unit;
        string _board;
        string _subject;

        readonly Screens _screens = new Screens();
        readonly Dictionary<int, Border> _uiScreens = new Dictionary<int, Border>();
        private Display _display;

        public MainWindow()
        {
            InitializeComponent();

            SetInitialData();

            CreateDisplayWindow();

            //populate cb
            PopulateComboBoxes();
            PopulateUiScreens();

            UpdateUidIsplay();

            Topmost = true;

        }

        private void SetInitialData()
        {
            //set inital data
            _startHours = SettingsIoData.StartH;
            _startMinutes = SettingsIoData.StartM;
            _startTime = new TimeSpan(_startHours, _startMinutes, 0);

            _durationHours = SettingsIoData.DurationH;
            _durationMinutes = SettingsIoData.DurationM;
            _duration = new TimeSpan(_durationHours, _durationMinutes, 0);

            _centre = SettingsIoData.Centre;
            _unit = SettingsIoData.Unit;
            _board = SettingsIoData.Board;
            _subject = SettingsIoData.Subject;

            //setup timer
            settingsIoTimer.Tick += SettingsIoTimerTick;
        }

        private void CreateDisplayWindow()
        {
            _display = new Display(_screens, _startTime, _duration);
            _display.Closed += (localSender, args) => _display = null;
            _display.Topmost = false;
            _display.SetCentreDisplay(_centre);
            _display.SetExamDisplay(_board, _subject, _unit);
            _display.StartTime = _startTime;
            _display.Duration = _duration;
            _display.Show();
        }

        private void UpdateUidIsplay()
        {
            centreSelect.Text = _centre;
            unitSelect.Text = _unit;
            boardSelect.SelectedValue = _board;
            subjectSelect.SelectedValue = _subject;

            DurationH.Text = _durationHours.ToString();
            DurationM.Text = _durationMinutes.ToString();

            StartH.Text = _startHours.ToString();
            StartM.Text = _startMinutes.ToString();

            _display.SetExamDisplay(_board, _subject, _unit);
            _display.StartTime = _startTime;
            _display.Duration = _duration;
        }

        private void PopulateComboBoxes()
        {
            //Board
            boardSelect.ItemsSource = SettingsIoData.Boards;

            //Subject
            subjectSelect.ItemsSource = SettingsIoData.Subjects;

            _display.SetExamDisplay(_board, _subject, _unit);
        }

        private void DurationSet(object sender, TextChangedEventArgs e)
        {
            bool hSuccess = false;
            bool mSuccess = false;

            var tb = sender as TextBox;
            if (tb != null)
            {
                if (tb.Name == "DurationH")
                {
                    hSuccess = int.TryParse(DurationH.Text, out _durationHours);

                    if (!hSuccess || DurationH.Text.Length > 1 || _durationHours > 4)
                    {
                        //cull last character
                        int length = DurationH.Text.Length - 1;
                        if (length < 0) { length = 0; }
                        DurationH.Text = DurationH.Text.Substring(0, length);
                        int.TryParse(DurationH.Text, out _durationHours);
                    }
                }
                else if (tb.Name == "DurationM")
                {
                    mSuccess = int.TryParse(DurationM.Text, out _durationMinutes);

                    if (!mSuccess || DurationM.Text.Length > 2 || _durationMinutes > 59)
                    {
                        //cull last character
                        int length = DurationM.Text.Length - 1;
                        if (length < 0) { length = 0; }
                        DurationM.Text = DurationM.Text.Substring(0, length);
                        int.TryParse(DurationM.Text, out _durationMinutes);
                    }
                }

                //set the duration
                _duration = new TimeSpan(_durationHours, _durationMinutes, 0);
                _display.Duration = _duration;
            }
            
        }

        private void StartSet(object sender, TextChangedEventArgs e)
        {
            bool hSuccess = false;
            bool mSuccess = false;

            var tb = sender as TextBox;
            if (tb != null)
            {
                if (tb.Name == "StartH")
                {
                    hSuccess = int.TryParse(StartH.Text, out _startHours);

                    if (!hSuccess || StartH.Text.Length > 2 || _startHours > 24)
                    {
                        //cull last character
                        int length = StartH.Text.Length - 1;
                        if (length < 0) { length = 0; }
                        StartH.Text = StartH.Text.Substring(0, length);
                        int.TryParse(StartH.Text, out _startHours);
                    }
                }
                else if (tb.Name == "StartM")
                {
                    mSuccess = int.TryParse(StartM.Text, out _startMinutes);

                    if (!mSuccess || StartM.Text.Length > 2 || _startMinutes > 59)
                    {
                        //cull last character
                        int length = StartM.Text.Length - 1;
                        if (length < 0) { length = 0; }
                        StartM.Text = StartM.Text.Substring(0, length);
                        int.TryParse(StartM.Text, out _startMinutes);
                    }
                }

                //set the duration
                _startTime = new TimeSpan(_startHours, _startMinutes, 0);
                _display.StartTime = _startTime;
            }

        }



        private void CentreSet(object sender, TextChangedEventArgs e)
        {
            var senderBox = sender as TextBox;
            if (senderBox != null)
            {
                _centre = senderBox.Text;
                _display.SetCentreDisplay(_centre);

                //update Settings in data for saving.
                settingsIoTimer.Interval = new TimeSpan(0, 0, 0, 3);
                settingsIoTimer.Start();
            }
        }

        private void SettingsIoTimerTick(object sender, EventArgs e)
        {
            //update centre number for data
            SettingsIoData.Centre = _centre;
            settingsIoTimer.Stop();
            //MessageBox.Show("Timer Completed, Settings Written");
        }

        private void UnitSet(object sender, TextChangedEventArgs e)
        {
            var senderBox = sender as TextBox;
            if (senderBox != null)
            {
                _unit = senderBox.Text;
                _display.SetExamDisplay(_board, _subject, _unit);
            }
        }
        

        private void BoardChange(object sender, SelectionChangedEventArgs e)
        {
            if (boardSelect.SelectedValue != null)
            {
                _board = boardSelect.SelectedValue.ToString();
                _display.SetExamDisplay(_board, _subject, _unit);
            }
        }

        private void SubjectChange(object sender, SelectionChangedEventArgs e)
        {
            if (subjectSelect.SelectedValue != null)
            {
                _subject = subjectSelect.SelectedValue.ToString();
                _display.SetExamDisplay(_board, _subject, _unit);
            }
        }


        private void PopulateUiScreens()
        {
            int i = 1;


            foreach (var screen in _screens.AllScreens)
            {

                Border bdr = new Border();

                if (_screens.SelectedScreen == screen)
                {
                    bdr = CreateUiBorder(name: i, selected: true);
                }
                else
                {
                    bdr = CreateUiBorder(name: i, selected: false);
                }

                ScreenDisplay.Children.Add(bdr);

                //add border to array - --index for 0 start
                _uiScreens[i - 1] = bdr;
                i++;

            }
            //dv.Content = "Screens: " + _display.AllScreens.Length + ":: [" + (_display.SelectedScreenId + 1) + "]";
            dv.Content = "Screens: " + _screens.AllScreens.Length;
        }

        private Border CreateUiBorder(int name, bool selected = false)
        {
            Brush border;
            Brush foreground;

            //handle selected styling
            if (selected)
            {
                border = Theme.SelectedBrush;
                foreground = Theme.SelectedBrush;
            }
            else
            {
                border = Theme.DeselectedBrush;
                foreground = Theme.DeselectedBrush;
            }

            Brush background = Brushes.White;


            Border bdr = new Border
            {
                Background = background,
                BorderBrush = border,
                BorderThickness = new Thickness(2),
                SnapsToDevicePixels = true,
                Width = 35,
                Height = 35,
                CornerRadius = new CornerRadius(5),
                Name = "screen_" + (name - 1)
            };
            bdr.MouseDown += screenSelect_MouseDown;

            var tb = new TextBlock
            {
                Text = name.ToString(),
                Foreground = foreground,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Background = Brushes.Transparent,
                FontSize = 20,
                FontWeight = FontWeights.DemiBold
            };

            bdr.Child = tb;

            return bdr;

        }

        private int GetScreenName(string name)
        {
            int screenNumber = -1;
            string numericalName = name.Substring(7);
            //System.Windows.MessageBox.Show("numericalName: " + numericalName);
            int.TryParse(numericalName, out screenNumber);

            if (screenNumber != -1)
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

            int selectedScreenId = -1;

            //select screen
            if (sender is Border)
            {
                var bdr = (Border)sender;
                selectedScreenId = GetScreenName(bdr.Name);

            }

            //update ui for selected screen and switch screens
            if (selectedScreenId != -1)
            {
                UpdateSelectedScreenUi(oldSelected: _screens.SelectedScreenId, newSelected: selectedScreenId);

                //set selected screen
                _screens.SetScreen(selectedScreenId);

                //dv.Content = "Screens: " + _display.AllScreens.Length + ":: [" + (selectedScreenId + 1) + "]";

                //broadcast screenchange
                //if (ScreenChange != null)
                //    ScreenChange(this, new ScreenEventArgs() { SelectedScreen = selectedScreenId });
            }

        }

        private void UpdateSelectedScreenUi(int oldSelected, int newSelected)
        {
            if (oldSelected != newSelected)
            {
                //MessageBox.Show(String.Format("oldSelected: {0}, newSelected: {1}", oldSelected, newSelected));

                //switch old screen back to deselected
                Border oldBdr = _uiScreens[oldSelected];
                oldBdr.BorderBrush = Theme.DeselectedBrush;
                oldBdr.Background = Brushes.White;
                TextBlock oldTxt = oldBdr.Child as TextBlock;
                if (oldTxt != null)
                    oldTxt.Foreground = Theme.DeselectedBrush;

                //switch new screen to selected
                Border newBdr = _uiScreens[newSelected];
                newBdr.BorderBrush = Theme.SelectedBrush;
                newBdr.Background = Brushes.White;
                TextBlock newTxt = newBdr.Child as TextBlock;
                if (newTxt != null)
                    newTxt.Foreground = Theme.SelectedBrush;
            }
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            base.OnClosing(e);
        }

    }
}

