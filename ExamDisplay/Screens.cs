using System;
using System.Linq;
using System.Windows.Forms;

namespace ExamDisplay
{
    public class Screens
    {
        //internal vars
        private Screen[] _screens = Screen.AllScreens;
        private Screen _primary = Screen.PrimaryScreen;
        private Screen _selectedScreen = null;
        private int _selectedScreenId = 0;

        //properties
        public Screen[] AllScreens { get { return _screens; } }
        public Screen PrimaryScreen { get { return _primary; } }
        public Screen SelectedScreen { get { return _selectedScreen; } }
        public int SelectedScreenId { get { return _selectedScreenId; } }

        //set up event handler
        public event EventHandler<ScreenEventArgs> ScreenChange;

        public Screens()
        {
            //get primary screen
            Screen primary = Screen.PrimaryScreen;

            //set selected screen to non primary
            if (_screens.Count() > 1)
            {
                _selectedScreenId = _screens[0] == primary ? 1 : 0;
            }
            else
            {
                _selectedScreenId = 0;
            }

            _selectedScreen = _screens[_selectedScreenId];
        }

        public void SetScreen(int screenId)
        {
            //check new screenId is within bounds
            if (screenId >= 0 && screenId <= _screens.Count())
            {
                //switch selecyed screen
                _selectedScreenId = screenId;
                _selectedScreen = _screens[_selectedScreenId];

                //fire event for change
                if (ScreenChange != null)
                    ScreenChange(this, new ScreenEventArgs() { SelectedScreen = _selectedScreenId });
            }
        }

        
    }
}