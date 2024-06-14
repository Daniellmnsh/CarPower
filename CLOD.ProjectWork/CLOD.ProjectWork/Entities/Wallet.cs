// Wallet.cs
namespace CLOD.ProjectWork.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public string User { get; set; } // Deve essere string per rappresentare l'ID dell'utente
        public decimal Money { get; set; }
    }
}

