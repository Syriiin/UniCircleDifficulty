using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

using CefSharp;
using Newtonsoft.Json.Linq;

using UniCircleTools;
using UniCircleTools.Beatmaps;
using DifficultyCalculator = UniCircle.Difficulty.Standard.DifficultyCalculator;

namespace UniCircle.Visualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DifficultyCalculator Calculator { get; set; } = new DifficultyCalculator();

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
            
            // Enable hitobject data button
            HitObjectDataButton.IsEnabled = true;

            // Load d3 graphs
            LoadGraphs();
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

        private void GraphBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
        }

        private void GraphBrowser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (GraphBrowser.IsBrowserInitialized)
            {
                GraphBrowser.LoadHtml(Properties.Resources.VisualiserGraphs, "https://syrin.me/unicircle-visualiser");
            }
        }

        private void LoadGraphs()
        {
            JObject beatmapSettings = new JObject(
                new JProperty("cs", Calculator.Beatmap.Difficulty.CS),
                new JProperty("ar", Calculator.Beatmap.Difficulty.AR)
            );

            JArray hitObjects = new JArray(Calculator.DifficultyPoints.Where(p => p.BaseHitObject.GetType() != typeof(Spinner)).Select(p => new JObject(
                new JProperty("x", p.BaseHitObject.X),
                new JProperty("y", p.BaseHitObject.Y),
                new JProperty("time", p.BaseHitObject.Time),
                new JProperty("newCombo", p.BaseHitObject.NewCombo)
            )));

            JArray dataPoints = new JArray( // array of skill
                Calculator.DifficultyPoints[0].SkillDatas.Select((skillData, i) => new JObject(
                    new JProperty("name", skillData.SkillType.Name),
                    new JProperty("dataSets", new JArray(   // array of datapoint type
                        skillData.DataPoints.Keys.Select(dataPointName => new JObject(
                            new JProperty("name", dataPointName),
                            new JProperty("dataPoints", new JArray(
                                Calculator.DifficultyPoints.Where(p => p.BaseHitObject.GetType() != typeof(Spinner)).Select(difficultyPoint => new JValue(difficultyPoint.SkillDatas[i].DataPoints[dataPointName]))
                            ))
                        ))
                    ))
                ))
            );

            GraphBrowser.ExecuteScriptAsync($"setData({beatmapSettings.ToString()}, {hitObjects.ToString()}, {dataPoints.ToString()});");
        }
    }
}
