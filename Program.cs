using System;
using PortfolioTracker.Models;
using PortfolioTracker.Services;

namespace PortfolioTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initializam modelul
            var portofoliulMeu = new Portfolio("Test");

            // Initializam serviciul care gestioneaza logica
            var manager = new PortfolioManager();

            // Bagam niste actiuni de test
            var apple = new Asset("AAPL", "Apple", 10, 150m);
            var bitcoin = new Asset("BTC", "Bitcoin", 0.5m, 60000m);

            // Folosim managerul pentru a modifica portofoliul
            manager.AddOrUpdateAsset(portofoliulMeu, apple);
            manager.AddOrUpdateAsset(portofoliulMeu, bitcoin);

            // Printam in consola
            Console.WriteLine($"Portofoliul lui {portofoliulMeu.OwnerName}");
            Console.WriteLine($"Total investit: {portofoliulMeu.TotalPortfolioInvested} USD");
            Console.WriteLine($"Valoare curenta: {portofoliulMeu.TotalPortfolioValue} USD");

            Console.ReadLine();
        }
    }
}