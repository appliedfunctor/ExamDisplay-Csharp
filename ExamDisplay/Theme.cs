using System.Windows.Media;

namespace ExamDisplay
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>

    public static class Theme
    {
        //brushes
        private static Brush _selected = Brushes.Black;
        private static Brush _deSelected = Brushes.DarkGray;
        private static Brush _themeBlue = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF4F8FB4");

        //properties
        public static Brush SelectedBrush { get { return _selected; } }
        public static Brush DeselectedBrush { get { return _deSelected; } }
        public static Brush Blue { get { return _themeBlue; } }

    }
}