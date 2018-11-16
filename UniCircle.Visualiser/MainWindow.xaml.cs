using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;

using UniCircleTools;
using UniCircleTools.Beatmaps;
using UniCircle.Difficulty;
using DifficultyCalculator = UniCircle.Difficulty.Standard.DifficultyCalculator;

namespace UniCircle.Visualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DifficultyCalculator Calculator { get; set; } = new DifficultyCalculator();

        public SeriesCollection AimingChartSeries { get; set; } = new SeriesCollection(Mappers.Xy<DifficultyPoint>().X(p => p.BaseHitObject.Time).Y(p => p.SkillDatas[0].Difficulty));
        public SeriesCollection ClickingChartSeries { get; set; } = new SeriesCollection(Mappers.Xy<DifficultyPoint>().X(p => p.BaseHitObject.Time).Y(p => p.SkillDatas[1].Difficulty));
        public SeriesCollection ReadingChartSeries { get; set; } = new SeriesCollection(Mappers.Xy<DifficultyPoint>().X(p => p.BaseHitObject.Time).Y(p => p.SkillDatas[2].Difficulty));

        public Func<double, string> XFormatter => ms => TimeSpan.FromMilliseconds(ms).ToString(@"mm\:ss\:fff");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Disable hitobject data button by default
            HitObjectDataButton.IsEnabled = false;

            DataGridAimPoint.ItemsSource = Calculator.DifficultyPoints;
            DataGridClickPoint.ItemsSource = Calculator.DifficultyPoints;
            DataGridVisualPoint.ItemsSource = Calculator.DifficultyPoints;
        }

        private Mods GetMods()
        {
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

            return mods;
        }

        private void Recalculate()
        {
            if (Calculator.Beatmap == null)
            {
                return;
            }

            Calculator.SetMods(GetMods());

            Calculator.CalculateDifficulty();

            DisplayData();
        }

        private void DisplayData()
        {
            // Populate Difficulty labels
            labelTotalDifficulty.Content = String.Format("Total: {0:0.##} stars", Calculator.Difficulty);
            labelAimingDifficulty.Content = String.Format("Aiming: {0:0.##} stars", Calculator.DifficultyPoints.Where(p => p.SkillDatas.Count > 0).Max(p => p.SkillDatas[0].Difficulty));
            labelClickingDifficulty.Content = String.Format("Clicking: {0:0.##} stars", Calculator.DifficultyPoints.Where(p => p.SkillDatas.Count > 0).Max(p => p.SkillDatas[1].Difficulty));
            labelReadingDifficulty.Content = String.Format("Reading: {0:0.##} stars", Calculator.DifficultyPoints.Where(p => p.SkillDatas.Count > 0).Max(p => p.SkillDatas[2].Difficulty));

            DataGridAimPoint.Items.Refresh();
            DataGridClickPoint.Items.Refresh();
            DataGridVisualPoint.Items.Refresh();
            
            // Draw charts
            AimingChartSeries.Clear();
            AimingChartSeries.Add(
                new LineSeries
                {
                    Values = new ChartValues<DifficultyPoint>(Calculator.DifficultyPoints.Where(p => p.SkillDatas.Count > 0)),
                    PointGeometry = null
                }
            );
            ClickingChartSeries.Clear();
            ClickingChartSeries.Add(
                new LineSeries
                {
                    Values = new ChartValues<DifficultyPoint>(Calculator.DifficultyPoints.Where(p => p.SkillDatas.Count > 0)),
                    PointGeometry = null
                }
            );
            ReadingChartSeries.Clear();
            ReadingChartSeries.Add(
                new LineSeries
                {
                    Values = new ChartValues<DifficultyPoint>(Calculator.DifficultyPoints.Where(p => p.SkillDatas.Count > 0)),
                    PointGeometry = null
                }
            );
            
            // Enable hitobject data button
            HitObjectDataButton.IsEnabled = true;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "osu! Beatmap files (*.osu)|*.osu"
            };

            if (openFileDialog.ShowDialog(this) == true)
            {
                LoadBeatmap(openFileDialog.FileName);
            }
        }

        private void HitObjectData_Click(object sender, RoutedEventArgs e)
        {
            new HitObjectData(Calculator.DifficultyPoints).Show();
        }

        private void LoadBeatmap(string filePath)
        {
            Calculator.SetBeatmap(new Beatmap(filePath));
            textBlockOpenBeatmap.Text = $"{Calculator.Beatmap.Artist} - {Calculator.Beatmap.Title} [{Calculator.Beatmap.Version}]";
            Recalculate();
        }

        private void CheckboxMod_Changed(object sender, RoutedEventArgs e)
        {
            Recalculate();
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

        private void SkillSetting_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            Recalculate();
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                LoadBeatmap(files[0]);
            }
        }
    }
}
