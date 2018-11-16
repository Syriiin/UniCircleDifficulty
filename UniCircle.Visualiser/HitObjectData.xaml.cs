using System;
using System.Data;
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
        public HitObjectData(List<DifficultyPoint> difficultyPoints)
        {
            InitializeComponent();

            // this table population is so ugly i regret even writing it
            var dataTable = new DataTable();

            var columns = new List<KeyValuePair<string, string>>();

            // set columns
            dataTable.Columns.Add("Offset", typeof(int));
            foreach (var difficultyPoint in difficultyPoints)
            {
                foreach (var skillData in difficultyPoint.SkillDatas)
                {
                    // add difficulty column
                    var difficultyColumn = $"{skillData.SkillType.Name} Difficulty";
                    if (!dataTable.Columns.Contains(difficultyColumn))
                    {
                        columns.Add(new KeyValuePair<string, string>(skillData.SkillType.Name, ""));
                        dataTable.Columns.Add(difficultyColumn, typeof(double));
                    }
                    // add data point columns
                    foreach (string key in skillData.DataPoints.Keys)
                    {
                        var column = $"{skillData.SkillType.Name} {key}";
                        if (!dataTable.Columns.Contains(column))
                        {
                            columns.Add(new KeyValuePair<string, string>(skillData.SkillType.Name, key));
                            dataTable.Columns.Add(column, typeof(double));
                        }
                    }
                }
            }

            // set rows
            foreach (var difficultyPoint in difficultyPoints)
            {
                var row = dataTable.NewRow();
                row[0] = difficultyPoint.BaseHitObject.Time;
                int i = 1;
                foreach (var column in columns)
                {
                    if (difficultyPoint.SkillDatas.Count > 0)
                    {
                        double value;
                        if (column.Value == "")
                        {
                            // add difficulty item
                            value = difficultyPoint.SkillDatas.Find(d => d.SkillType.Name == column.Key).Difficulty;
                        }
                        else
                        {
                            // add data point items
                            difficultyPoint.SkillDatas.Find(d => d.SkillType.Name == column.Key).DataPoints.TryGetValue(column.Value, out value);
                        }

                        // round output for better display
                        row[i] = Math.Round(value, 3);
                        i++;
                    }
                }
                dataTable.Rows.Add(row);
            }

            DifficultyPointDataGrid.ItemsSource = dataTable.DefaultView;
        }
    }
}
