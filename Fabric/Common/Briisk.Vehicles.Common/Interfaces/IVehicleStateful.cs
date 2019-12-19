using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Briisk.Vehicles.Common.Interfaces
{
    public interface IVehicleStateful :IService
    {
        Task<List<VehicleModel>> GetAllVehiclesAsync(CancellationToken cancellationToken);

        Task<bool> AddVehicleAsync(VehicleModel vehicleModel, CancellationToken cancellationToken);

    }
}
