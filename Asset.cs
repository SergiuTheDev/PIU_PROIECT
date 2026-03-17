using System;

namespace PortfolioTracker.Models
{
    // Clasa de baza pentru un activ
    public class Asset
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal AveragePurchasePrice { get; set; }
        public decimal CurrentPrice { get; set; }

        // Proprietati calculate (Data Binding pentru UI)
        public decimal TotalInvested => Quantity * AveragePurchasePrice;
        public decimal CurrentValue => Quantity * CurrentPrice;
        public decimal ProfitLoss => CurrentValue - TotalInvested;

        public decimal ProfitLossPercentage
        {
            get
            {
                if (TotalInvested == 0) return 0;
                return (ProfitLoss / TotalInvested) * 100;
            }
        }

        public Asset(string symbol, string name, decimal quantity, decimal averagePurchasePrice)
        {
            Symbol = symbol;
            Name = name;
            Quantity = quantity;
            AveragePurchasePrice = averagePurchasePrice;
            CurrentPrice = averagePurchasePrice;
        }
    }
}