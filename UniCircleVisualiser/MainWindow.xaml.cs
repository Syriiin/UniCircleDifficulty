using System;
using System.Windows;
using Microsoft.Win32;

using UniCircleTools;
using UniCircleTools.Beatmaps;
using UniCircleDifficulty;

namespace UniCircleVisualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string beatmapPath;
        private DifficultyCalculator calculator;

        public string LoadedBeatmapTitle => calculator?.Beatmap.Title;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void InitaliseCalculator()
        {
            if (beatmapPath == null) // If beatmapPath is null, a beatmap hasnt been loaded
            {
                return;
            }

            Mods mods = Mods.None;

            // Settings mods
            if (checkboxEZ.IsChecked ?? false)
            {
                mods |= Mods.Easy;
            }
            else if (checkboxHR.IsChecked ?? false)
            {
                mods |= Mods.HardRock;
            }
            
            // Time mods
            if (checkboxHT.IsChecked ?? false)
            {
                mods |= Mods.HalfTime;
            }
            else if (checkboxDT.IsChecked ?? false)
            {
                mods |= Mods.DoubleTime;
            }

            // Visual mods
            if (checkboxHD.IsChecked ?? false)
            {
                mods |= Mods.Hidden;
            }
            if (checkboxFL.IsChecked ?? false)
            {
                mods |= Mods.Flashlight;
            }

            calculator = new DifficultyCalculator(new Beatmap(beatmapPath), mods);
            calculator.CalculateDifficulty();

            textBlockOpenBeatmap.Text = String.Format("{0} - {1} [{2}]", calculator.Beatmap.Artist, calculator.Beatmap.Title, calculator.Beatmap.Version);
            DisplayData();
        }

        private void Recalculate()
        {
            if (calculator == null) // If calculator is null, a beatmap hasnt been loaded
            {
                return;
            }

            // TODO: expose skill constants so they can be modified at runtime for testing
            calculator.CalculateDifficulty();

            DisplayData();
        }

        private void DisplayData()
        {
            // Populate Difficulty labels
            labelAimingDifficulty.Content = String.Format("Aiming: {0:0.##} stars", calculator.Aiming.Value);
            labelClickingDifficulty.Content = String.Format("Clicking: {0:0.##} stars", calculator.Clicking.Value);
            labelReadingDifficulty.Content = String.Format("Reading: {0:0.##} stars", calculator.Reading.Value);

            // Populate datagrids
            dataGridAimPoint.ItemsSource = calculator.Aiming.CalculatedPoints;
            dataGridClickPoint.ItemsSource = calculator.Clicking.CalculatedPoints;
            dataGridVisualPoint.ItemsSource = calculator.Reading.CalculatedPoints;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "osu! Beatmap files (*.osu)|*.osu"
            };

            if (openFileDialog.ShowDialog(this) == true)
            {
                beatmapPath = openFileDialog.FileName;
                InitaliseCalculator();
            }
        }

        private void CheckboxMod_Changed(object sender, RoutedEventArgs e)
        {
            InitaliseCalculator();
        }

        private void CheckboxEZ_Checked(object sender, RoutedEventArgs e)
        {
            checkboxHR.IsChecked = false;
            CheckboxMod_Changed(sender, e);
        }

        private void CheckboxHR_Checked(object sender, RoutedEventArgs e)
        {
            checkboxEZ.IsChecked = false;
            CheckboxMod_Changed(sender, e);
        }

        private void CheckboxHT_Checked(object sender, RoutedEventArgs e)
        {
            checkboxDT.IsChecked = false;
            CheckboxMod_Changed(sender, e);
        }

        private void CheckboxDT_Checked(object sender, RoutedEventArgs e)
        {
            checkboxHT.IsChecked = false;
            CheckboxMod_Changed(sender, e);
        }
    }
}
