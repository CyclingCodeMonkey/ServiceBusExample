using System;

namespace ConsoleAppServiceBus.Example.Common
{
    [Serializable]
    public class Address : IMessage
    {
        public int Id { get; set; }
        public string BuildingName { get; set; }
        public string LevelNumber { get; set; }
        public string UnitNumber { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }

        public Address()
        {
            Created = DateTime.Now;
            Modified = DateTime.Now;
        } 
    }
}
