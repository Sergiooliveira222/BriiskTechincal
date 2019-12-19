using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Briisk.VehiclesService.API.Logic
{
    public interface IVehicleAPILogic
    {
        Task<bool> AddVehicle(VehicleDTO vehicleDTO, CancellationToken cancellationToken);
        Task<List<VehicleDTO>> GetVehicleDTOs(CancellationToken cancellationToken);
    }
}