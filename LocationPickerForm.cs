using System;
using System.Windows.Forms;

namespace MilkAnalyzerTest
{
    public class LocationPickerForm : Form
    {
        public string SelectedLocation { get; private set; } = string.Empty;
        public LocationPickerForm()
        {
            Width = 400;
            Height = 150;
            Text = "Set Location";
            StartPosition = FormStartPosition.CenterParent;

            var lbl = new Label { Text = "Select Location:", Left = 10, Top = 10, Width = 100 };
            var cmb = new ComboBox { Left = 120, Top = 8, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            cmb.Items.AddRange(new[] { "Clinic A", "Clinic B", "Other" });
            cmb.SelectedIndex = 0;

            var btnOk = new Button { Text = "OK", Left = 200, Top = 50, Width = 70 };
            var btnCancel = new Button { Text = "Cancel", Left = 280, Top = 50, Width = 70 };

            btnOk.Click += (s, e) =>
            {
                SelectedLocation = cmb.SelectedItem?.ToString() ?? string.Empty;
                DialogResult = DialogResult.OK;
                Close();
            };

            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.Add(lbl);
            Controls.Add(cmb);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
        }
    }
}
