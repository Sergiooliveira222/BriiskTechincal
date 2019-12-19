using System;

namespace Briisk.Vehicles.Common
{
    public class VehicleModel 
    {
        #region Properties
        public Guid VehicleID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        #endregion
    }
}
