using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioTracker.Models
{
    // 1. DATA MODEL: Clasa care tine doar datele portofoliului
    public class Portfolio
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Adaugat pentru Baza de Date
        public string OwnerName { get; set; }
        public List<Asset> Assets { get; set; }

        public decimal TotalPortfolioValue => Assets.Sum(a => a.CurrentValue);
        public decimal TotalPortfolioInvested => Assets.Sum(a => a.TotalInvested);
        public decimal TotalPortfolioProfitLoss => TotalPortfolioValue - TotalPortfolioInvested;

        public Portfolio(string ownerName)
        {
            OwnerName = ownerName;
            Assets = new List<Asset>();
        }
    }
}