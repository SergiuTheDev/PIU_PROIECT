using System;

namespace PortfolioTracker.Models
{
    // Clasa ce memoreaza pozitia/investitia noastra pe un anumit activ
    public class Position
    {
        public Guid Id { get; set; }
        
        public Asset AssetDetails { get; set; } // Legatura catre activul de pe bursa/piata
        
        public decimal Quantity { get; set; }
        public decimal AveragePurchasePrice { get; set; }

        // Proprietati calculate pentru randament
        public decimal TotalInvested => Quantity * AveragePurchasePrice;
        public decimal CurrentValue => Quantity * AssetDetails.CurrentPrice;
        public decimal ProfitLoss => CurrentValue - TotalInvested;
        
        public decimal ProfitLossPercentage
        {
            get
            {
                if (TotalInvested == 0) return 0;
                return (ProfitLoss / TotalInvested) * 100;
            }
        }

        public Position() { }

        public Position(Asset asset, decimal quantity, decimal purchasePrice)
        {
            // Evitam crearea unor pozitii invalide
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            if (quantity < 0) throw new ArgumentException("Cantitatea nu poate fi negativa.", nameof(quantity));
            if (purchasePrice < 0) throw new ArgumentException("Pretul de achizitie nu poate fi negativ.", nameof(purchasePrice));

            Id = Guid.NewGuid();
            AssetDetails = asset;
            Quantity = quantity;
            AveragePurchasePrice = purchasePrice;
        }

        public void RemoveShares(decimal quantityToRemove)
        {
            if (quantityToRemove <= 0) throw new ArgumentException("Cantitatea eliminată trebuie să fie pozitivă.");
            if (quantityToRemove > Quantity) throw new InvalidOperationException("Nu poți vinde mai multe acțiuni decât deții.");
            
            Quantity -= quantityToRemove;
        }

        // Calculeaza noul pret mediu cand mai adaugam actiuni (Dollar Cost Averaging)
        public void AddMoreShares(decimal newQuantity, decimal purchasePrice)
        {
            if (newQuantity <= 0) throw new ArgumentException("Cantitatea adaugata trebuie sa fie pozitiva.");
            if (purchasePrice < 0) throw new ArgumentException("Pretul nu poate fi negativ.");

            decimal totalCostExisting = Quantity * AveragePurchasePrice;
            decimal totalCostNew = newQuantity * purchasePrice;

            Quantity += newQuantity;
            AveragePurchasePrice = (totalCostExisting + totalCostNew) / Quantity;
        }
    }
}
