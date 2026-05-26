using System;
using PortfolioTracker.Models;

namespace PortfolioTracker.Services
{
    // Realizeaza inregistrarea tranzactiilor in portofoliul utilizatorului
    public class PortfolioManager
    {
        public void LogPurchase(Portfolio portfolio, Asset asset, decimal quantity, decimal purchasePrice)
        {
            Console.WriteLine($"[MANAGER] Se inregistreaza ADAUGAREA de active pentru portofoliul \"{portfolio.OwnerName}\"...");
            Console.WriteLine($"[MANAGER] S-au adaugat: +{quantity}x {asset.Symbol} @ {purchasePrice:F2} USD.");
            
            // Trimite mai departe in logica portofoliului
            portfolio.AddOrUpdatePosition(asset, quantity, purchasePrice);
            
            // (aici va urma instiintarea de UI Update sau salvarea in db a tranzactiei)
            Console.WriteLine("[MANAGER] Inregistrare efectuata cu succes!");
        }

        public void LogSale(Portfolio portfolio, string assetSymbol, decimal? quantity = null)
        {
            if (quantity.HasValue)
            {
                portfolio.SellPosition(assetSymbol, quantity.Value);
            }
            else
            {
                bool success = portfolio.RemovePosition(assetSymbol);
                if (!success)
                    throw new Exception($"Eroare: Nu s-a găsit nicio poziție deschisă cu simbolul {assetSymbol} în portofoliu.");
            }
        }
    }
}