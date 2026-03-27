using System;
using PortfolioTracker.Models;

namespace PortfolioTracker.Services
{
    // Realizeaza tranzactiile in portofoliul utilizatorului pe diverse instrumente
    public class PortfolioManager
    {
        public void ExecuteBuyOrder(Portfolio portfolio, Asset asset, decimal quantity, decimal purchasePrice)
        {
            Console.WriteLine($"[MANAGER] Se executa ordin de CUMPARARE pentru portofoliul \"{portfolio.OwnerName}\"...");
            Console.WriteLine($"[MANAGER] Tranzactie: +{quantity}x {asset.Symbol} @ {purchasePrice:F2} USD.");
            
            // Trimite mai departe in logica portofoliului
            portfolio.BuyOrUpdatePosition(asset, quantity, purchasePrice);
            
            // (aici va urma instiintarea de UI Update sau salvarea in db a facturii efectuate)
            Console.WriteLine("[MANAGER] Ordin executat cu succes!");
        }

        public void ExecuteSellOrder(Portfolio portfolio, string assetSymbol)
        {
            Console.WriteLine($"[MANAGER] Se executa ordin de VANZARE totala: pozitia {assetSymbol}...");
            bool success = portfolio.SellPosition(assetSymbol);

            if (success)
                Console.WriteLine("[MANAGER] Pozitia stearsa cu succes!");
            else
                Console.WriteLine("[MANAGER] Eroare: Pozitia nu a fost gasita in portofoliu.");
        }
    }
}