using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Briisk.Vehicles.Common.Interfaces
{
    public interface IVehicleRepository
    {
        Task<List<VehicleModel>> GetVehicles(CancellationToken cancellationToken);

        Task<bool> AddVehicle(VehicleModel vehicleModel, CancellationToken cancellationToken);

    }
}
