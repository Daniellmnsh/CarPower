namespace CLOD.ProjectWork.Entities
{

    public class ChargingStation
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool IsActive { get; set; }
        public bool HasFastCharge { get; set; }
        public decimal KwPrice { get; set; }
    }
}
