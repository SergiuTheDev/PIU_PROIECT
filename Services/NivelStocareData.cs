using System;
using System.IO;
using System.Text.Json;
using PortfolioTracker.Models;

namespace PortfolioTracker.Services
{
    public class NivelStocareData
    {
        public void SavePortfolio(Portfolio portfolio)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(portfolio, options);
                File.WriteAllText(Constants.DataFilePath, jsonString);
                Console.WriteLine("[STOCARE] Portofoliul a fost salvat cu succes în fișier.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[STOCARE EROARE] Nu s-a putut salva portofoliul: {ex.Message}");
            }
        }

        public Portfolio LoadPortfolio()
        {
            try
            {
                if (File.Exists(Constants.DataFilePath))
                {
                    string jsonString = File.ReadAllText(Constants.DataFilePath);
                    var portfolio = JsonSerializer.Deserialize<Portfolio>(jsonString);
                    if (portfolio != null)
                    {
                        Console.WriteLine("[STOCARE] Portofoliul a fost încărcat cu succes din fișier.");
                        return portfolio;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[STOCARE EROARE] Nu s-a putut încărca portofoliul: {ex.Message}");
            }
            
            // Daca fisierul nu exista sau sunt erori, returnam un portofoliu nou
            Console.WriteLine("[STOCARE] Se crează un portofoliu nou (fișier lipsă sau corupt).");
            return new Portfolio("User");
        }
    }
}
