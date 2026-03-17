using System;
using System.Linq;
using PortfolioTracker.Models;

namespace PortfolioTracker.Services
{
    // 2. BUSINESS LOGIC: Clasa care se ocupa de operatiuni
    public class PortfolioManager
    {
        // Metoda sa adaugam chestii noi in portofoliu
        public void AddOrUpdateAsset(Portfolio portfolio, Asset newAsset)
        {
            var existingAsset = portfolio.Assets.FirstOrDefault(a => a.Symbol.Equals(newAsset.Symbol, StringComparison.OrdinalIgnoreCase));

            if (existingAsset != null)
            {
                // Calculam noul pret mediu de achizitie (DCA)
                decimal totalCostExisting = existingAsset.Quantity * existingAsset.AveragePurchasePrice;
                decimal totalCostNew = newAsset.Quantity * newAsset.AveragePurchasePrice;

                existingAsset.Quantity += newAsset.Quantity;
                existingAsset.AveragePurchasePrice = (totalCostExisting + totalCostNew) / existingAsset.Quantity;

                // Actualizam si pretul curent la cel mai recent
                existingAsset.CurrentPrice = newAsset.CurrentPrice;
            }
            else
            {
                portfolio.Assets.Add(newAsset);
            }
        }

        // Scoatem o actiune din lista dupa simbol
        public void RemoveAsset(Portfolio portfolio, string symbol)
        {
            var assetToRemove = portfolio.Assets.FirstOrDefault(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
            if (assetToRemove != null)
            {
                portfolio.Assets.Remove(assetToRemove);
            }
        }
    }
}