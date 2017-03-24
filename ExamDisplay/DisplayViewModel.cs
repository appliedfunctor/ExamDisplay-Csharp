using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using ExamDisplay.Annotations;

namespace ExamDisplay
{
    /// <summary>
    /// Interaction logic for Display.xaml
    /// </summary>
    /// 
    
    public class DisplayViewModel : INotifyPropertyChanged
    {

        //Timer
        private readonly DispatcherTimer _clockTimer = new DispatcherTimer(DispatcherPriority.Render);


        private string _centreDisplay;
        public string CentreDisplay {
            get { return _centreDisplay; }
            set
            {
                _centreDisplay = value;
                OnPropertyChanged(nameof(CentreDisplay));
            }
        }

        private string _timeDisplay = "-:--";

        public string TimeDisplay
        {
            get { return _timeDisplay; }
            set
            {
                _timeDisplay = value;
                OnPropertyChanged(nameof(TimeDisplay));
            }
        }

        private string _dateDisplay;
        public string DateDisplay
        {
            get { return _dateDisplay; }
            set
            {
                _dateDisplay = value;
                OnPropertyChanged(nameof(DateDisplay));
            }
        }

        public string ExamDisplay { get; set; }
        
        public string StartDisplay { get; set; }
        public string FinishDisplay { get; set; }


        public DisplayViewModel()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            //Call tick
            clockTimer_Tick(new object(), new EventArgs());

            //start timer
            _clockTimer.Tick += clockTimer_Tick;
            _clockTimer.Interval = new TimeSpan(0, 0, 1);
            _clockTimer.Start();
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            //handle clock tick
            TimeDisplay = DateTime.Now.ToString("H:mm");

            var dateString = DateTime.Now.ToString("dd MMM yyyy");

            if (DateDisplay != dateString)
                DateDisplay = dateString;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}