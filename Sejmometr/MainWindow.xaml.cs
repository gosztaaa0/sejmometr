using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading.Tasks;

namespace Sejmometr
{
    public partial class MainWindow : Window
    {
        private const string AdresApi = "https://api.sejm.gov.pl/sejm/term10";
        private List<Posel> wszyscyPoslowie = new List<Posel>();

        public MainWindow()
        {
            InitializeComponent();
            InicjalizujDane();
        }


        private async void InicjalizujDane()
        {
            try
            {
                using (HttpClient klient = new HttpClient())
                {
                    string json = await klient.GetStringAsync($"{AdresApi}/proceedings");
                    var posiedzenia = JsonConvert.DeserializeObject<List<Posiedzenie>>(json);

                    CmbPosiedzenia.ItemsSource = posiedzenia.OrderByDescending(p => p.Number).ToList();

                    if (CmbPosiedzenia.Items.Count > 0) CmbPosiedzenia.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania listy posiedzeń: " + ex.Message);
            }
        }

        private async Task PobierzPoslow()
        {
            try
            {
                using (HttpClient klient = new HttpClient())
                {
                    string url = $"{AdresApi}/MP";
                    string json = await klient.GetStringAsync(url);
                    wszyscyPoslowie = JsonConvert.DeserializeObject<List<Posel>>(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd krytyczny: Nie udało się pobrać listy posłów.\n" + ex.Message);
            }
        }


        private async void BtnPobierzListeGlosowan_Click(object sender, RoutedEventArgs e)
        {
            if (CmbPosiedzenia.SelectedValue == null)
            {
                MessageBox.Show("Proszę najpierw wybrać posiedzenie z listy.");
                return;
            }

            try
            {
                if (wszyscyPoslowie.Count == 0) await PobierzPoslow();
                if (wszyscyPoslowie.Count == 0) return;

                string nrPosiedzenia = CmbPosiedzenia.SelectedValue.ToString();
                string url = $"{AdresApi}/votings/{nrPosiedzenia}";

                using (HttpClient klient = new HttpClient())
                {
                    string json = await klient.GetStringAsync(url);
                    var listaGlosowan = JsonConvert.DeserializeObject<List<GlosowanieOpcja>>(json);
                    listaGlosowan.Reverse();
                    CmbGlosowania.ItemsSource = listaGlosowan;
                    CmbGlosowania.IsDropDownOpen = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania listy głosowań:\n" + ex.Message);
            }
        }

        private async void BtnPobierzGlosowanie_Click(object sender, RoutedEventArgs e)
        {
            if (wszyscyPoslowie.Count == 0 || CmbGlosowania.SelectedValue == null || CmbPosiedzenia.SelectedValue == null) return;

            try
            {
                string nrPosiedzenia = CmbPosiedzenia.SelectedValue.ToString();
                int nrGlosowania = (int)CmbGlosowania.SelectedValue;
                string url = $"{AdresApi}/votings/{nrPosiedzenia}/{nrGlosowania}";

                using (HttpClient klient = new HttpClient())
                {
                    string json = await klient.GetStringAsync(url);
                    var glosowanie = JsonConvert.DeserializeObject<Glosowanie>(json);
                    TxtTytulGlosowania.Text = glosowanie.Title;

                    var glosyWKlubach = new Dictionary<string, List<string>>();
                    foreach (var glos in glosowanie.Votes)
                    {
                        var posel = wszyscyPoslowie.FirstOrDefault(p => p.Id == glos.MP);
                        if (posel != null)
                        {
                            if (!glosyWKlubach.ContainsKey(posel.Club)) glosyWKlubach[posel.Club] = new List<string>();
                            glosyWKlubach[posel.Club].Add(glos.Vote);
                        }
                    }

                    var liniaPartii = new Dictionary<string, string>();
                    foreach (var klub in glosyWKlubach.Keys)
                    {
                        liniaPartii[klub] = glosyWKlubach[klub].GroupBy(g => g).OrderByDescending(g => g.Count()).First().Key;
                    }

                    List<WynikWiersz> ladnaLista = new List<WynikWiersz>();
                    int countYes = 0, countNo = 0, countAbstain = 0, countAbsent = 0;

                    foreach (var glos in glosowanie.Votes)
                    {
                        if (glos.Vote == "YES") countYes++;
                        else if (glos.Vote == "NO") countNo++;
                        else if (glos.Vote == "ABSTAIN") countAbstain++;
                        else if (glos.Vote == "ABSENT") countAbsent++;

                        var posel = wszyscyPoslowie.FirstOrDefault(p => p.Id == glos.MP);
                        if (posel != null)
                        {
                            string decyzjaKlubu = liniaPartii.ContainsKey(posel.Club) ? liniaPartii[posel.Club] : "";

                            bool jestBuntownikiem = (glos.Vote != decyzjaKlubu) &&
                                                    (glos.Vote == "YES" || glos.Vote == "NO") &&
                                                    (decyzjaKlubu == "YES" || decyzjaKlubu == "NO");
                            bool czyPokazacTyp = false;
                            if (glos.Vote == "YES" && ChkZa.IsChecked == true) czyPokazacTyp = true;
                            else if (glos.Vote == "NO" && ChkPrzeciw.IsChecked == true) czyPokazacTyp = true;
                            else if (glos.Vote == "ABSTAIN" && ChkWstrzymal.IsChecked == true) czyPokazacTyp = true;
                            else if (glos.Vote == "ABSENT" && ChkNieobecny.IsChecked == true) czyPokazacTyp = true;

                            if (!czyPokazacTyp) continue;
                            if (ChkBuntownicy.IsChecked == true && !jestBuntownikiem) continue;

                            ladnaLista.Add(new WynikWiersz
                            {
                                NazwiskoImie = posel.FirstLastName,
                                Klub = posel.Club,
                                Glos = glos.Vote,
                                ZdjecieUrl = $"{AdresApi}/MP/{posel.Id}/photo",
                                KolorTla = jestBuntownikiem ? System.Windows.Media.Brushes.MistyRose : System.Windows.Media.Brushes.Transparent,
                                InfoDodatkowe = jestBuntownikiem ? $"[BUNT! Klub: {decyzjaKlubu}]" : ""
                            });
                        }
                    }
                    ListaWynikow.ItemsSource = ladnaLista.OrderBy(w => w.Glos).ThenBy(w => w.NazwiskoImie);

                    // --- AKTUALIZACJA WYKRESU ---
                    SeriesCollection wykresDane = new SeriesCollection();
                    if (countYes > 0) wykresDane.Add(new PieSeries { Title = "ZA", Values = new ChartValues<int> { countYes }, Fill = System.Windows.Media.Brushes.Green, DataLabels = true });
                    if (countNo > 0) wykresDane.Add(new PieSeries { Title = "PRZECIW", Values = new ChartValues<int> { countNo }, Fill = System.Windows.Media.Brushes.Red, DataLabels = true });
                    if (countAbstain > 0) wykresDane.Add(new PieSeries { Title = "WSTRZYMAŁ SIĘ", Values = new ChartValues<int> { countAbstain }, Fill = System.Windows.Media.Brushes.Orange, DataLabels = true });
                    if (countAbsent > 0) wykresDane.Add(new PieSeries { Title = "NIEOBECNY", Values = new ChartValues<int> { countAbsent }, Fill = System.Windows.Media.Brushes.Gray, DataLabels = true });
                    WykresGlosow.Series = wykresDane;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd analizy:\n" + ex.Message);
            }
        }

        private void BtnZapiszPDF_Click(object sender, RoutedEventArgs e)
        {
            if (ListaWynikow.ItemsSource == null) return;

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                StackPanel container = new StackPanel { Margin = new Thickness(40) };

                container.Children.Add(new TextBlock { Text = "RAPORT GŁOSOWANIA", FontSize = 20, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) });
                container.Children.Add(new TextBlock { Text = TxtTytulGlosowania.Text, FontSize = 12, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 20) });

                foreach (WynikWiersz wiersz in ListaWynikow.ItemsSource)
                {
                    container.Children.Add(new TextBlock
                    {
                        Text = $"{wiersz.Glos.PadRight(10)} | {wiersz.Klub.PadRight(10)} | {wiersz.NazwiskoImie} {wiersz.InfoDodatkowe}",
                        FontFamily = new System.Windows.Media.FontFamily("Courier New"),
                        FontSize = 10
                    });
                }

                container.Measure(new Size(printDialog.PrintableAreaWidth, double.PositiveInfinity));
                container.Arrange(new Rect(new Point(0, 0), container.DesiredSize));
                printDialog.PrintVisual(container, "Raport Sejmometr");
            }
        }
    }
}