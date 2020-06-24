using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RSSLTraceViewerGUI
{
    /// <summary>
    ///     Interaction logic for DisplayMRNData.xaml
    /// </summary>
    public partial class DisplayMRNData : Window
    {
        public DisplayMRNData()
        {
            InitializeComponent();
            RawHeaderText = "";
            DataContext = this;
        }

        public string RawData { get; set; }
        public string RawHeaderText { get; set; }
        public JToken JsonUnpackToken { get; set; }

        private void SaveJsonBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.Filter = "Json files (*.json)|*.json";
                saveFileDialog.InitialDirectory = Environment.CurrentDirectory;
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var fs = File.Open(saveFileDialog.FileName, FileMode.OpenOrCreate))
                    using (var sw = new StreamWriter(fs))
                    using (var jsw = new JsonTextWriter(sw))
                    {
                        jsw.Formatting = Formatting.Indented;

                        var serializer = new JsonSerializer();
                        serializer.Serialize(jsw, JsonUnpackToken);
                    }

                    MessageBox.Show($"Save json data to {saveFileDialog.FileName} Completed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Writing Json to file Error\r\n {ex.Message}");
            }
        }
    }
}