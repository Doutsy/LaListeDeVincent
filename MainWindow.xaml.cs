using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace LaListeDeVincent
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        
        ObservableCollection<Eleve> eleves = new ObservableCollection<Eleve>();
        public string NewEleveName { get; set; } = "Nom de l'élève";
        public string NewEleveGroup { get; set; } = "Groupe de l'élève";

        public MainWindow()
        {
            InitializeComponent();
            
            
            var result = JsonConvert.DeserializeObject<List<Eleve>>(Get(@"http://137.74.194.177:5000/api/eleves/"));
            for (int i = 0; i < result.Count; i++)
            {
                var newEleve = result[i];
                newEleve.Id = i;
                eleves.Add(newEleve);
            }
            
            ListBoxGroupes.ItemsSource = eleves;

            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            eleves.Add(new Eleve() { Nom = NewEleveName, Groupe = NewEleveGroup, Id = eleves.Count});
            string data = JsonConvert.SerializeObject(eleves.Last());
            Post(@"http://137.74.194.177:5000/api/eleves/", data, "application/json");
        }

        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public string Post(string uri, string data, string contentType, string method = "POST")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;

            using (Stream requestBody = request.GetRequestStream())
            {
                requestBody.Write(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private void DeleteCurrentEleve(object sender, RoutedEventArgs e)
        {
            var id = (int) ((Button) sender).Tag;
            var deletedEleve = eleves[id];
            string data = JsonConvert.SerializeObject(deletedEleve);
            Post(@"http://137.74.194.177:5000/api/eleves/5", data, "application/json");

            eleves.RemoveAt(id);
            for (int i = 0; i < eleves.Count; i++)
            {
                eleves[i].Id = i;
            }
            var tempEleves = new ObservableCollection<Eleve>(eleves);
            eleves.Clear();
            foreach (var eleve in tempEleves)
            {
                eleves.Add(eleve);
            }
        }
    }

    public class Eleve
    {
        public string Nom { get; set; }
        public string Groupe { get; set; }
        public int Id { get; set; }
    }
}
