using System;
using System.Windows;
using System.Windows.Media;
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

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (chkConfirm.IsChecked != true)
            {
                MessageBox.Show("Vă rugăm să bifați căsuța de confirmare înainte de a executa tranzacția.", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string symbol = txtSymbol.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(symbol))
            {
                MessageBox.Show("Introduceți un simbol valid.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (rbBuy.IsChecked == true)
                {
                    // Cumpărare (Adăugare)
                    string name = string.IsNullOrWhiteSpace(txtName.Text) ? "Necunoscut" : txtName.Text.Trim();
                    
                    if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
                        throw new Exception("Cantitatea trebuie să fie un număr pozitiv.");
                        
                    if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
                        throw new Exception("Prețul trebuie să fie un număr pozitiv.");

                    Asset asset = new Asset(symbol, name, price);
                    _manager.LogPurchase(_portfolio, asset, quantity, price);
                    
                    MessageBox.Show($"Ați cumpărat cu succes {quantity} x {symbol}.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (rbSell.IsChecked == true)
                {
                    // Vânzare (Ștergere totală poziție pentru simplitate)
                    _manager.LogSale(_portfolio, symbol);
                    MessageBox.Show($"Ați vândut (șters) poziția {symbol}.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Curățăm input-urile
                txtSymbol.Clear();
                txtName.Clear();
                txtQuantity.Clear();
                txtPrice.Clear();
                chkConfirm.IsChecked = false;

                RefreshUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare la procesare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RbTransaction_Changed(object sender, RoutedEventArgs e)
        {
            // Ascundem controalele inutile dacă este Vânzare
            if (rbSell != null && rbBuy != null && txtName != null)
            {
                bool isBuy = rbBuy.IsChecked == true;
                
                txtName.IsEnabled = isBuy;
                txtQuantity.IsEnabled = isBuy;
                txtPrice.IsEnabled = isBuy;
                
                lblAssetName.Opacity = isBuy ? 1.0 : 0.5;
                lblQuantity.Opacity = isBuy ? 1.0 : 0.5;
                lblPrice.Opacity = isBuy ? 1.0 : 0.5;
            }
        }

        private void RefreshUI()
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
