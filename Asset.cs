using System;

namespace PortfolioTracker.Models
{
    // Clasa ce reprezinta un instrument financiar de pe piata 
    public class Asset
    {
        public Guid Id { get; private set; } 
        
        public string Symbol { get; private set; }
        public string Name { get; private set; }
        
        public decimal CurrentPrice { get; set; }

        public Asset(string symbol, string name, decimal initialPrice)
        {
            // Verificam datele la instantiere
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Simbolul nu poate fi gol.", nameof(symbol));
                
            if (initialPrice < 0)
                throw new ArgumentException("Pretul initial nu poate fi negativ.", nameof(initialPrice));

            Id = Guid.NewGuid(); 
            Symbol = symbol;
            Name = name;
            CurrentPrice = initialPrice;
        }
    }
}