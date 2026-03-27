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

        public void LogSale(Portfolio portfolio, string assetSymbol)
        {
            Console.WriteLine($"[MANAGER] Se inregistreaza ELIMINAREA totala a pozitiei: {assetSymbol}...");
            bool success = portfolio.RemovePosition(assetSymbol);

            if (success)
                Console.WriteLine("[MANAGER] Pozitia a fost stearsa cu succes din tracker!");
            else
                Console.WriteLine("[MANAGER] Eroare: Pozitia nu a fost gasita in portofoliu.");
        }
    }
}