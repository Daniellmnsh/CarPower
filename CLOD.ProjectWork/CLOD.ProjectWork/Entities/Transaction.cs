namespace CLOD.ProjectWork.Entities
{
    public class Transaction
    {
        public int Id { get; set; } // Identificativo univoco della transazione

        public int ControllerId { get; set; } // Identificativo del controller/stazione di ricarica

        public string UserId { get; set; } // Identificativo dell'utente

        public decimal KwUsage { get; set; } // Consumo di energia in kW

        public DateTime Date { get; set; } // Data della transazione

        public decimal TotalMoney { get; set; } // Costo totale della transazione
    }
}
