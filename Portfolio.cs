using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioTracker.Models
{
    // Modeleaza portofoliul financiar al utilizatorului
    public class Portfolio
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string OwnerName { get; private set; }

        private readonly List<Position> _positions;
        
        // Expunem lista doar citire pentru siguranta datelor
        public IReadOnlyList<Position> Positions => _positions.AsReadOnly();

        public decimal TotalPortfolioValue => _positions.Sum(p => p.CurrentValue);
        public decimal TotalPortfolioInvested => _positions.Sum(p => p.TotalInvested);
        public decimal TotalPortfolioProfitLoss => TotalPortfolioValue - TotalPortfolioInvested;

        public Portfolio(string ownerName)
        {
            if (string.IsNullOrWhiteSpace(ownerName))
                throw new ArgumentException("Numele detinatorului nu poate fi gol.");

            OwnerName = ownerName;
            _positions = new List<Position>();
        }

        // Adauga o entitate noua sau ii face update daca exista deja 
        public void AddOrUpdatePosition(Asset asset, decimal quantity, decimal purchasePrice)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));

            var existingPosition = _positions.FirstOrDefault(p => p.AssetDetails.Symbol.Equals(asset.Symbol, StringComparison.OrdinalIgnoreCase));

            if (existingPosition != null)
            {
                // Daca o detinem deja, recalculam media de cumparare si adaugam actiunile noi
                existingPosition.AddMoreShares(quantity, purchasePrice);
            }
            else
            {
                // In caz contrar, o instantiem in portofoliu
                _positions.Add(new Position(asset, quantity, purchasePrice));
            }
        }

        public bool RemovePosition(string symbol)
        {
            var targetPosition = _positions.FirstOrDefault(p => p.AssetDetails.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
            
            if (targetPosition != null)
            {
                _positions.Remove(targetPosition);
                return true;
            }
            return false;
        }
    }
}