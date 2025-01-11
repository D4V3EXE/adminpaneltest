using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;

namespace IKT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Type.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string comboBoxValue = (Type.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            string textBox1Value = Name.Text;
            string textBox2Value = Specs.Text;
            string textBox3Value = Price.Text;

            string data = $"{Environment.NewLine}{comboBoxValue};{textBox1Value};{textBox2Value};{textBox3Value}";
            if (string.IsNullOrWhiteSpace(comboBoxValue) || string.IsNullOrWhiteSpace(textBox1Value) || string.IsNullOrWhiteSpace(textBox2Value) || string.IsNullOrWhiteSpace(textBox3Value))
            {
                ResultsListBox.Items.Clear();
                ResultsListBox.Items.Add("Tölts ki minden mezőt!");
            }
            else{
                File.AppendAllText("adat.txt", data);
                ResultsListBox.Items.Clear();
                ResultsListBox.Items.Add("Sikeres mentés");
            }
                
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsListBox.Items.Clear();

            string searchTerm = SearchBar.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                ResultsListBox.Items.Add("Adj meg egy keresést!");
                return;
            }

            string filePath = "adat.txt";

            if (!File.Exists(filePath))
            {
                ResultsListBox.Items.Add("A txt fájl nem létezik.");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);

            var results = lines.Where(line => line.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            if (results.Count == 0)
            {
                ResultsListBox.Items.Add("Nincs találat.");
            }
            else
            {
                foreach (var result in results)
                {
                    ResultsListBox.Items.Add(result);
                }
            }
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "adat.txt";

            int intelCpus = 0;
            int amdCpus = 0;
            int nvidiaGpus = 0;
            int amdGpus = 0;

            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(';');
                if (parts.Length > 1)
                {
                    string category = parts[0];
                    string name = parts[1];

                    if (category == "CPU" && name.Contains("Intel"))
                    {
                        intelCpus++;
                    }

                    if (category == "CPU" && name.Contains("AMD"))
                    {
                        amdCpus++;
                    }

                    if (category == "GPU" && name.Contains("NVIDIA"))
                    {
                        nvidiaGpus++;
                    }

                    if (category == "GPU" && name.Contains("AMD"))
                    {
                        amdGpus++;
                    }
                    }
                }

            ResultsListBox.Items.Clear();
            ResultsListBox.Items.Add($"Intel CPU-k: {intelCpus}\nAMD CPU-k: {amdCpus}\nNVIDIA GPU-k: {nvidiaGpus}\nAMD GPU-k: {amdGpus}");
        }

        private void DiscountButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(DiscountTextBox.Text) || DiscountComboBox.SelectedItem == null)
            {
                ResultsListBox.Items.Clear();
                ResultsListBox.Items.Add("Tölts ki minden mezőt!");
                return;
            }

            if (!int.TryParse(DiscountTextBox.Text, out int discountPercentage) || discountPercentage <= 0 || discountPercentage > 100)
            {
                ResultsListBox.Items.Clear();
                ResultsListBox.Items.Add("Kérlek adj meg egy érvényes százalékot 1-100 között!");
                return;
            }

            string selectedCategory = (DiscountComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? string.Empty;

            var lines = File.ReadAllLines("adat.txt");
            var updatedLines = new List<string>();

            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length < 4)
                {
                    updatedLines.Add(line);
                    continue;
                }

                string type = parts[0];
                string name = parts[1];
                string details = parts[2];

                if (!int.TryParse(parts[3], out int price))
                {
                    updatedLines.Add(line);
                    continue;
                }

                if (selectedCategory == "ÖSSZES" || selectedCategory == type)
                {
                    price = (int)(price * (1 - discountPercentage / 100.0));
                }

                updatedLines.Add($"{type};{name};{details};{price}");
            }

            File.WriteAllLines("adat.txt", updatedLines, System.Text.Encoding.UTF8);

            ResultsListBox.Items.Clear();
            ResultsListBox.Items.Add($"Az árak {discountPercentage}%-os akcióra állítva.");
            ResultsListBox.Items.Add($"Kategória: {selectedCategory}");
        }

    }
}