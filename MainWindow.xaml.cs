using System;
using System.Windows;
using System.Windows.Media;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PortfolioTracker.Models;
using PortfolioTracker.Services;

namespace PortfolioTracker
{
    public partial class MainWindow : Window
    {
        private Portfolio _portfolio;
        private PortfolioManager _manager;

        public MainWindow()
        {
            InitializeComponent();
            
            // Initializare Backend
            _portfolio = new Portfolio("User");
            _manager = new PortfolioManager();
            
            // Populam cu niste date demonstrative
            try
            {
                var appleStock = new Asset("AAPL", "Apple Inc.", 150m);
                var microsoftStock = new Asset("MSFT", "Microsoft Corporation", 350m);
                
                _manager.LogPurchase(_portfolio, appleStock, 10m, 150m);
                _manager.LogPurchase(_portfolio, microsoftStock, 5m, 350m);
            }
            catch { }
            
            RefreshUI();
        }

        private async void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (chkConfirm.IsChecked != true)
            {
                MessageBox.Show("Vă rugăm să bifați căsuța de confirmare înainte de a executa tranzacția.", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string symbol = txtSymbol.Text.Trim().ToUpper();
            
            if (string.IsNullOrWhiteSpace(symbol))
            {
                MessageBox.Show("Introduceți un simbol valid.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                btnExecute.IsEnabled = false;

                if (rbBuy.IsChecked == true)
                {
                    if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
                        throw new Exception("Cantitatea trebuie să fie un număr pozitiv.");
                        
                    if (!decimal.TryParse(txtPurchasePrice.Text, out decimal userPurchasePrice) || userPurchasePrice < 0)
                        throw new Exception("Prețul de achiziție trebuie să fie un număr pozitiv.");

                    // Fetch data from API automatically
                    decimal price = 0;
                    string name = symbol;

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                        string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{symbol}";
                        string response = await client.GetStringAsync(url);

                        using (JsonDocument doc = JsonDocument.Parse(response))
                        {
                            var root = doc.RootElement;
                            var result = root.GetProperty("chart").GetProperty("result")[0];
                            var meta = result.GetProperty("meta");

                            if (meta.TryGetProperty("regularMarketPrice", out JsonElement priceElement))
                                price = priceElement.GetDecimal();
                            else
                                throw new Exception("Nu s-a putut găsi prețul curent pe piață.");

                            if (meta.TryGetProperty("shortName", out JsonElement nameElement))
                                name = nameElement.GetString() ?? symbol;
                        }
                    }

                    Asset asset = new Asset(symbol, name, price);
                    _manager.LogPurchase(_portfolio, asset, quantity, userPurchasePrice);
                    
                    MessageBox.Show($"Ați introdus cu succes {quantity} x {symbol} ({name}) la prețul de {userPurchasePrice:F2} USD/buc.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (rbSell.IsChecked == true)
                {
                    _manager.LogSale(_portfolio, symbol);
                    MessageBox.Show($"Ați vândut (șters) poziția {symbol}.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Curățăm input-urile
                txtSymbol.Clear();
                txtQuantity.Clear();
                txtPurchasePrice.Clear();
                chkConfirm.IsChecked = false;

                RefreshUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare la procesare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnExecute.IsEnabled = true;
            }
        }

        private void RbTransaction_Changed(object sender, RoutedEventArgs e)
        {
            if (rbSell != null && rbBuy != null && txtQuantity != null && txtPurchasePrice != null)
            {
                bool isBuy = rbBuy.IsChecked == true;
                txtQuantity.IsEnabled = isBuy;
                txtPurchasePrice.IsEnabled = isBuy;
                lblQuantity.Opacity = isBuy ? 1.0 : 0.5;
                if (lblPurchasePrice != null) lblPurchasePrice.Opacity = isBuy ? 1.0 : 0.5;
            }
        }

        private async void BtnRefreshAll_Click(object sender, RoutedEventArgs e)
        {
            if (_portfolio.Positions.Count == 0)
            {
                MessageBox.Show("Portofoliul este gol.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                btnRefreshAll.IsEnabled = false;
                btnRefreshAll.Content = "Se actualizează...";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                    foreach (var position in _portfolio.Positions)
                    {
                        string symbol = position.AssetDetails.Symbol;
                        string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{symbol}";
                        try
                        {
                            string response = await client.GetStringAsync(url);
                            using (JsonDocument doc = JsonDocument.Parse(response))
                            {
                                var meta = doc.RootElement.GetProperty("chart").GetProperty("result")[0].GetProperty("meta");
                                if (meta.TryGetProperty("regularMarketPrice", out JsonElement priceElement))
                                {
                                    position.AssetDetails.CurrentPrice = priceElement.GetDecimal();
                                }
                            }
                        }
                        catch
                        {
                            // Ignorăm erorile punctuale per simbol la actualizarea globală
                        }
                    }
                }

                RefreshUI();
            }
            finally
            {
                btnRefreshAll.IsEnabled = true;
                btnRefreshAll.Content = "🔄 Actualizează Prețurile";
            }
        }        private void RefreshUI()
        {
            // Actualizăm DataGrid-ul
            gridPositions.ItemsSource = null;
            gridPositions.ItemsSource = _portfolio.Positions;

            // Actualizăm Label-urile
            lblTotalInvested.Text = $"{_portfolio.TotalPortfolioInvested:F2} USD";
            lblCurrentValue.Text = $"{_portfolio.TotalPortfolioValue:F2} USD";
            
            decimal pnl = _portfolio.TotalPortfolioProfitLoss;
            lblProfitLoss.Text = $"{pnl:F2} USD";
            
            if (pnl > 0)
                lblProfitLoss.Foreground = Brushes.Green;
            else if (pnl < 0)
                lblProfitLoss.Foreground = Brushes.Red;
            else
                lblProfitLoss.Foreground = Brushes.Black;
        }
    }
}
