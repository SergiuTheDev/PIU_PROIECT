using System;
using PortfolioTracker.Models;
using PortfolioTracker.Services;

namespace PortfolioTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PORTFOLIO TRACKER OOP SYSTEM ===");
            
            // Definim cateva instrumente care se tranzactioneaza la bursa
            var appleStock = new Asset("AAPL", "Apple Inc.", 150m);
            var bitcoinCrypto = new Asset("BTC", "Bitcoin", 60000m);

            var portofoliulMeu = new Portfolio("Alexandru (Student CS)");
            var manager = new PortfolioManager();

            try
            {
                Console.WriteLine("\\n--- INITIAL BUY ---");
                manager.ExecuteBuyOrder(portofoliulMeu, appleStock, 10m, 150m);
                manager.ExecuteBuyOrder(portofoliulMeu, bitcoinCrypto, 0.5m, 60000m);

                // EX. Test invalid: Cantitate negativa va arunca throw.
                // manager.ExecuteBuyOrder(portofoliulMeu, appleStock, -5m, 100m); 

                // Testam adaugarea de pozitii suplimentare pentru acelasi instrument (mediazare) 
                Console.WriteLine("\\n--- DCA BUY ---");
                manager.ExecuteBuyOrder(portofoliulMeu, appleStock, 10m, 130m); 
                
                // Exemplu - manager.ExecuteSellOrder(portofoliulMeu, "BTC");
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"\\n[EROARE FATALA]: {ex.Message}");
            }

            // Rezumat final
            Console.WriteLine("\\n==================================");
            Console.WriteLine($"Portofoliul lui {portofoliulMeu.OwnerName}");
            Console.WriteLine($"Total Investit (Capital Blocat): {portofoliulMeu.TotalPortfolioInvested:F2} USD");
            Console.WriteLine($"Valoare Curenta (Market): {portofoliulMeu.TotalPortfolioValue:F2} USD");
            Console.WriteLine($"Profit / Pierdere Curenta: {portofoliulMeu.TotalPortfolioProfitLoss:F2} USD");
            Console.WriteLine("==================================\\n");

            // Printam pozitiile din lista detinuta
            Console.WriteLine("--- POZITII DETINUTE ---");
            foreach (var pozitie in portofoliulMeu.Positions)
            {
                Console.WriteLine($"- {pozitie.AssetDetails.Symbol}: {pozitie.Quantity} unitati | Pret Mediu Achizitie: {pozitie.AveragePurchasePrice:F2} USD | Sub-Total Curent: {pozitie.CurrentValue:F2} USD");
            }

            Console.WriteLine("\\nApasa Enter pentru a iesi...");
            Console.ReadLine();
        }
    }
}