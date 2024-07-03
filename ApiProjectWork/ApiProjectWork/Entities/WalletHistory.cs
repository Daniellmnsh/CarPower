namespace ApiProjectWork.Entities
{
    public class WalletHistory
    {
        public decimal Money { get; set; }
        public DateTime Date { get; set; }
        public int WalletId { get; set; }
        public string UserId { get; set; }
    }
}
