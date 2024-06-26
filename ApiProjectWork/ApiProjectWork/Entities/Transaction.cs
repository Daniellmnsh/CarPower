namespace ApiProjectWork.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public int ControllerId { get; set; }

        public string UserId { get; set; } 

        public decimal KwUsage { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalMoney { get; set; }
    }
}
