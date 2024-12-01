using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;
using static System.Net.Mime.MediaTypeNames;

namespace rzut_moneta
{
    public partial class MainWindow : Window
    {
        protected ImageAnimationController controler;
        public MainWindow()
        {
            InitializeComponent();
        }

        double wynik_p = 100, wynik = 100;
        double zysk = 2.0;
        double strata = 0.5;
        int runda = 0;
        int dlugosc = 5; 
        List <WynikData> wyniki_5 = new List<WynikData>();
        List<WynikData> wyniki_10 = new List<WynikData>();


        private void cmbDlugosc_gry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)cmbDlugosc_gry.SelectedItem;
            dlugosc = int.Parse(item.Content.ToString().Substring(0, 2));
            restart();
        }

        private void inputpkt_start_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputpkt_start.Text != "")
            {
                wynik_p = double.Parse(inputpkt_start.Text);
                restart();
            }
        }
        private void inputW_zysku_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputW_zysku.Text != "")
            {
                zysk = double.Parse(inputW_zysku.Text);
                if (cbRowne_w.IsChecked == true)
                {
                    strata = Math.Round(1 / zysk, zysk.ToString().Length);
                    inputW_straty.Text = strata.ToString();
                }
                restart();

            }
        }
        private void inputW_straty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputW_straty.Text != "")
            {
                strata = double.Parse(inputW_straty.Text);
                restart();
            }
        }

        private void cbRowne_w_Checked(object sender, RoutedEventArgs e)
        {
            inputW_straty.IsEnabled = false;
        }
        private void cbRowne_w_Unchecked(object sender, RoutedEventArgs e)
        {
            inputW_straty.IsEnabled = true;
        }


        private void btnRzut_Click(object sender, RoutedEventArgs e)
        {
            btnRzut.IsEnabled = false;

            if(controler.IsPaused)
            {
                controler.GotoFrame(0);
                controler.Play();
            }

            if (runda < dlugosc)
            {
                int rzut = losuj();
                int strona = obstawianie();

                string path = (rzut == 0) ? "Heads" : "Tails";
                imgMoneta.Source = new BitmapImage(new Uri("pack://application:,,,/" + path + ".png"));

                if (rzut == strona)
                {
                    wynik *= zysk;
                    lbltest.Content = rzut.ToString() + " " + strona.ToString();
                }
                else
                {
                    wynik *= strata;
                }
                lblWynik.Content = wynik.ToString();
            }
            else if (runda == dlugosc)
            {
                double roznica = wynik_p - wynik;
                if (roznica > 0)
                {
                    MessageBox.Show("Straciłeś: " + roznica + " monet");
                }
                else if( roznica < 0)
                {
                    MessageBox.Show("Wygrałeś: " + -roznica + " monet");
                }
                else
                {
                    MessageBox.Show("Nic nie straciłeś");

                }

                if (dlugosc == 5)
                {
                    lvTabela_5.ItemsSource = null;
                    wyniki_5.Insert(0, new WynikData { Wynik = wynik.ToString(), Czas = DateTime.Now.ToString("HH:mm:ss") });
                    lvTabela_5.ItemsSource = wyniki_5;
                }
                else if (dlugosc == 10)
                {
                    lvTabela_10.ItemsSource = null;
                    wyniki_10.Insert(0, new WynikData { Wynik = wynik.ToString(), Czas = DateTime.Now.ToString("HH:mm:ss") } );
                    lvTabela_10.ItemsSource = wyniki_10;
                }
                restart();
                
            }
            runda++;
        }

        public int losuj()
        {
            Random rng = new Random();
            int rzut = rng.Next(2);
            return rzut;
        }

        public int obstawianie()
        {
            if(rbOrzel.IsChecked == true)
            {
                return 1;
            }
            return 0;
        }

        public void restart()
        {
            runda = 0;
            wynik = wynik_p;
            lblWynik.Content = wynik.ToString();
        }


        private void walidacja(object sender, TextCompositionEventArgs e)
        {
            string currentText = ((System.Windows.Controls.TextBox)sender).Text + e.Text;
            string pattern = @"^[0-9]+(,[0-9]*)?$";

            e.Handled = !Regex.IsMatch(currentText, pattern);
        }


        private void txtNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void AnimationLoaded(object sender, EventArgs e)
        {
            this.controler = ImageBehavior.GetAnimationController(gifImage);
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            btnRzut.IsEnabled = true;
            controler.Pause();
        }
    }
}