using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PortfolioTracker.Enums;
using PortfolioTracker.Helpers;
using PortfolioTracker.Models;
using PortfolioTracker.Services;

namespace PortfolioTracker.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly Portfolio _portfolio;
        private readonly PortfolioManager _manager;
        private readonly FinanceApiService _apiService;
        private readonly NivelStocareData _storageService;

        public ObservableCollection<Position> PositionsList { get; set; }
        public ICollectionView PositionsView { get; set; }

        public MainViewModel()
        {
            _manager = new PortfolioManager();
            _apiService = new FinanceApiService();
            _storageService = new NivelStocareData();

            _portfolio = _storageService.LoadPortfolio();

            PositionsList = new ObservableCollection<Position>(_portfolio.Positions);
            PositionsView = CollectionViewSource.GetDefaultView(PositionsList);
            PositionsView.Filter = FilterPositions;

            ExecuteCommand = new RelayCommand(async _ => await ExecuteTransactionAsync(), _ => CanExecuteTransaction());
            RefreshCommand = new RelayCommand(async _ => await RefreshPricesAsync(), _ => true);

            UpdateMetrics();
        }

        #region Properties Bound to UI

        private string _symbol;
        public string Symbol
        {
            get => _symbol;
            set => SetProperty(ref _symbol, value);
        }

        private string _quantityText;
        public string QuantityText
        {
            get => _quantityText;
            set => SetProperty(ref _quantityText, value);
        }

        private string _purchasePriceText;
        public string PurchasePriceText
        {
            get => _purchasePriceText;
            set => SetProperty(ref _purchasePriceText, value);
        }

        private TransactionType _transactionType = TransactionType.Buy;
        public TransactionType SelectedTransactionType
        {
            get => _transactionType;
            set
            {
                if (SetProperty(ref _transactionType, value))
                {
                    OnPropertyChanged(nameof(IsBuyTransaction));
                    OnPropertyChanged(nameof(IsSellTransaction));
                }
            }
        }

        public bool IsBuyTransaction
        {
            get => _transactionType == TransactionType.Buy;
            set { if (value) SelectedTransactionType = TransactionType.Buy; }
        }

        public bool IsSellTransaction
        {
            get => _transactionType == TransactionType.Sell;
            set { if (value) SelectedTransactionType = TransactionType.Sell; }
        }

        private bool _isConfirmed;
        public bool IsConfirmed
        {
            get => _isConfirmed;
            set => SetProperty(ref _isConfirmed, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    PositionsView.Refresh();
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public decimal TotalInvested => _portfolio.TotalPortfolioInvested;
        public decimal CurrentValue => _portfolio.TotalPortfolioValue;
        public decimal ProfitLoss => _portfolio.TotalPortfolioProfitLoss;

        #endregion

        #region Commands

        public ICommand ExecuteCommand { get; }
        public ICommand RefreshCommand { get; }

        private bool CanExecuteTransaction()
        {
            return !IsBusy;
        }

        private async Task ExecuteTransactionAsync()
        {
            if (!IsConfirmed)
            {
                MessageBox.Show("Vă rugăm să bifați căsuța de confirmare înainte de a executa tranzacția.", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string symbol = Symbol?.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(symbol))
            {
                MessageBox.Show("Introduceți un simbol valid.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                IsBusy = true;

                if (IsBuyTransaction)
                {
                    if (!decimal.TryParse(QuantityText, out decimal quantity) || quantity <= 0)
                        throw new Exception("Cantitatea trebuie să fie un număr pozitiv.");

                    if (!decimal.TryParse(PurchasePriceText, out decimal userPurchasePrice) || userPurchasePrice < 0)
                        throw new Exception("Prețul de achiziție trebuie să fie un număr pozitiv.");

                    var (price, name) = await _apiService.GetAssetDataAsync(symbol);
                    Asset asset = new Asset(symbol, name, price);
                    
                    _manager.LogPurchase(_portfolio, asset, quantity, userPurchasePrice);
                    MessageBox.Show($"Ați introdus cu succes {quantity} x {symbol} ({name}) la prețul de {userPurchasePrice:F2} USD/buc.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    decimal? quantityToSell = null;
                    if (!string.IsNullOrWhiteSpace(QuantityText))
                    {
                        if (!decimal.TryParse(QuantityText, out decimal q) || q <= 0)
                            throw new Exception("Cantitatea de vândut trebuie să fie un număr pozitiv, sau lăsați gol pentru a vinde tot.");
                        quantityToSell = q;
                    }

                    _manager.LogSale(_portfolio, symbol, quantityToSell);
                    string msg = quantityToSell.HasValue 
                        ? $"Ați vândut {quantityToSell.Value} unități din poziția {symbol}." 
                        : $"Ați vândut (șters) întreaga poziție {symbol}.";
                    MessageBox.Show(msg, "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Salvează în fișier după modificare
                _storageService.SavePortfolio(_portfolio);

                // Resetare formular
                Symbol = string.Empty;
                QuantityText = string.Empty;
                PurchasePriceText = string.Empty;
                IsConfirmed = false;

                RefreshCollectionAndMetrics();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare la procesare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshPricesAsync()
        {
            if (_portfolio.Positions.Count == 0)
            {
                MessageBox.Show("Portofoliul este gol.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                IsBusy = true;

                foreach (var position in _portfolio.Positions)
                {
                    try
                    {
                        var data = await _apiService.GetAssetDataAsync(position.AssetDetails.Symbol);
                        position.AssetDetails.CurrentPrice = data.price;
                    }
                    catch
                    {
                        // Ignorăm erorile punctuale per simbol la actualizarea globală
                    }
                }

                // Salvam noile preturi in fisier (optional)
                _storageService.SavePortfolio(_portfolio);

                RefreshCollectionAndMetrics();
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Helpers

        private bool FilterPositions(object item)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (item is Position pos)
            {
                return pos.AssetDetails.Symbol.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                       pos.AssetDetails.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private void RefreshCollectionAndMetrics()
        {
            PositionsList.Clear();
            foreach (var p in _portfolio.Positions)
            {
                PositionsList.Add(p);
            }
            UpdateMetrics();
        }

        private void UpdateMetrics()
        {
            OnPropertyChanged(nameof(TotalInvested));
            OnPropertyChanged(nameof(CurrentValue));
            OnPropertyChanged(nameof(ProfitLoss));
        }

        #endregion
    }
}
