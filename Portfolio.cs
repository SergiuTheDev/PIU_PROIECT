using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioTracker.Models
{
    // Modeleaza portofoliul financiar al utilizatorului
    public class Portfolio
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OwnerName { get; set; }

        public List<Position> Positions { get; set; } = new List<Position>();

        public decimal TotalPortfolioValue => Positions.Sum(p => p.CurrentValue);
        public decimal TotalPortfolioInvested => Positions.Sum(p => p.TotalInvested);
        public decimal TotalPortfolioProfitLoss => TotalPortfolioValue - TotalPortfolioInvested;

        public Portfolio() { }

        public Portfolio(string ownerName)
        {
            if (string.IsNullOrWhiteSpace(ownerName))
                throw new ArgumentException("Numele detinatorului nu poate fi gol.");

            OwnerName = ownerName;
            Positions = new List<Position>();
        }

        // Adauga o entitate noua sau ii face update daca exista deja 
        public void AddOrUpdatePosition(Asset asset, decimal quantity, decimal purchasePrice)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));

            var existingPosition = Positions.FirstOrDefault(p => p.AssetDetails.Symbol.Equals(asset.Symbol, StringComparison.OrdinalIgnoreCase));

            if (existingPosition != null)
            {
                // Daca o detinem deja, recalculam media de cumparare si adaugam actiunile noi
                existingPosition.AddMoreShares(quantity, purchasePrice);
            }
            else
            {
                // In caz contrar, o instantiem in portofoliu
                Positions.Add(new Position(asset, quantity, purchasePrice));
            }
        }

        public bool RemovePosition(string symbol)
        {
            var targetPosition = Positions.FirstOrDefault(p => p.AssetDetails.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
            
            if (targetPosition != null)
            {
                Positions.Remove(targetPosition);
                return true;
            }
            return false;
        }

        public void SellPosition(string symbol, decimal quantityToSell)
        {
            var targetPosition = Positions.FirstOrDefault(p => p.AssetDetails.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
            if (targetPosition == null) throw new Exception($"Nu s-a găsit nicio poziție deschisă cu simbolul {symbol} în portofoliu.");

            if (quantityToSell >= targetPosition.Quantity)
            {
                Positions.Remove(targetPosition); // Vinde tot
            }
            else
            {
                targetPosition.RemoveShares(quantityToSell); // Vinde partial
            }
        }
    }
}