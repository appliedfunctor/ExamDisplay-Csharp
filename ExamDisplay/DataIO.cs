using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ExamDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public class DataIo
    {
        //default data
        private string _settingsFilePath;
        private readonly string _settingsFile = @"settings.cfg";
        private readonly string _settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ExamDisplay");
        private const string DefaultCentre = "Unknown";
        private const string DefaultUnit = "";
        private readonly string[] _defaultSubjects = { "English Language", "English Literature", "Maths A", "Biology", "Chemistry", "Physics", "Computer Science", "French", "History B", "Geography B", "Business Studies" };
        private readonly string[] _defaultBoards = { "AQA", "OCR", "Edexcel", "WJEC", "QCA" };
        private const int DefaultStartH = 9;
        private const int DefaultStartM = 0;
        private const int DefaultDurationH = 1;
        private const int DefaultDurationM = 0;

        private string[] _settingsArray;
        private bool _writeSettings = true;
        private string _centre;

        public string Centre {
            get { return _centre; }
            set
            {
                _centre = value;
                WriteSettingsToFile();
            }
        }

        public string[] Subjects { get; private set; } 
        public string[] Boards { get; private set; }

        public string Unit { get; private set; }
        public string Board { get; private set; }
        public string Subject { get; private set; }

        public int StartH { get; private set; }
        public int StartM { get; private set; }
        public int DurationH { get; private set; }
        public int DurationM { get; private set; }

        public DataIo()
        {
            //initialise values
            SetDefaults();
            
            //ensure write path exists
            Directory.CreateDirectory(_settingsDir);
            _settingsFilePath = Path.Combine(_settingsDir, _settingsFile);

            //read data
            ReadSettingsFomFile();
            
    }
        
        private void ReadSettingsFomFile()
        {
            try
            {
                _settingsArray = File.ReadAllLines(_settingsFilePath);
                ParseSettingsArray(_settingsArray);
            }
            catch(FileNotFoundException)
            {
                //check if file exists
                if(!System.IO.File.Exists(_settingsFilePath))
                    WriteSettingsToFile();
            }
        }

        private void SetDefaults()
        {
            Subjects = _defaultSubjects;
            Boards = _defaultBoards;
            _centre = DefaultCentre;

            Unit = DefaultUnit;
            Board = _defaultBoards[0];
            Subject = _defaultSubjects[0];

            StartH = DefaultStartH;
            StartM = DefaultStartM;
            DurationH = DefaultDurationH;
            DurationM = DefaultDurationM;
        }

        private void ParseSettingsArray(string[] settingsArray)
        {
            foreach (var settingsLine in settingsArray)
            {
                //trim whitespace
                var trimmedSettingsLine = settingsLine.Trim();

                //explode each line into a key value pair on the first instance of an ' = '
                string[] settingsPair = trimmedSettingsLine.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries);
                
                //ignore malformed lines
                if (settingsPair.Count() == 2)
                {
                    switch (settingsPair[0])
                    {
                        case "Centre":
                            if (!String.IsNullOrEmpty(settingsPair[1]))
                                Centre = settingsPair[1];
                            break;

                        case "Subjects":
                            var sjt = decodeSettingsString(settingsPair[1]);
                            if (sjt.Any())
                                Subjects = sjt;
                            break;

                        case "Boards":
                            var brd = decodeSettingsString(settingsPair[1]);
                            if (brd.Any())
                                Boards = brd;
                            break;

                        case "Start":
                            if (!String.IsNullOrEmpty(settingsPair[1]))
                                SetStart(settingsPair[1]);
                            break;

                        case "Duration":
                            if (!String.IsNullOrEmpty(settingsPair[1]))
                                SetDuration(settingsPair[1]);
                            break;
                    }
                }

                
            }
        }

        private string[] decodeSettingsString(string settingsString)
        {
            return settingsString.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }
        
        private void SetStart(string s)
        {
            //remove whitespace
            s = s.Trim();

            //decode the HH:MM format
            StartH = GetTime(s, TimeValue.Hours);
            StartM = GetTime(s, TimeValue.Minutes);
        }

        private void SetDuration(string s)
        {
            //remove whitespace
            s = s.Trim();

            //decode the HH:MM format
            DurationH = GetTime(s, TimeValue.Hours);
            DurationM = GetTime(s, TimeValue.Minutes);

        }

        private int GetTime(string t, TimeValue timeValue)
        {
            //set default
            int tValue = 0;

            //determine that string is expected length
            if (t.Length >= 5)
            {
                //grab the appropriate section
                t = t.Substring((int)timeValue, 2);
                int.TryParse(t, out tValue);
            }

            return tValue;
        }

        private void WriteSettingsToFile()
        {
            //only attempt write if enabled
            if (_writeSettings)
            {
                //create the data
                string[] settingsArrayFull = new string[]
                {
                    "Centre = " + _centre,
                    "Subjects = " + String.Join(",", Subjects),
                    "Boards = " + String.Join(",", Boards),
                    "Start = " + StartH.ToString().PadLeft(2).Replace(' ', '0') + ":" + StartM.ToString().PadLeft(2).Replace(' ', '0'),
                    "Duration = " + DurationH.ToString().PadLeft(2).Replace(' ', '0') + ":" + DurationM.ToString().PadLeft(2).Replace(' ', '0')
                };

                try
                {
                    System.IO.File.WriteAllLines(_settingsFilePath, settingsArrayFull);
                }
                catch
                {
                    _writeSettings = false;
                    MessageBox.Show("Error - Could not write to settings file.\nPlease check permissions in executing folder.\nSaving settings has been disabled for this session.");
                }
            }
        }
    }
}