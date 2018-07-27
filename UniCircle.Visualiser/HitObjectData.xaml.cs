using System;
using System.Windows;
using System.Collections.Generic;

using UniCircle.Difficulty;

namespace UniCircle.Visualiser
{
    /// <summary>
    /// Interaction logic for HitObjectData.xaml
    /// </summary>
    public partial class HitObjectData : Window
    {
        public HitObjectData(List<DifficultyHitObject> hitObjects)
        {
            InitializeComponent();

            HitObjectDataGrid.ItemsSource = hitObjects;
        }
    }
}
