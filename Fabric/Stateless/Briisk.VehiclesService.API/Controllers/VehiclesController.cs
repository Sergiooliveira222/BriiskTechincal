using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Briisk.Vehicles.Common.Constants;
using Briisk.Vehicles.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Client;
using Briisk.Vehicles.Common;
using System.Threading;
using Briisk.VehiclesService.API.Logic;

namespace Briisk.VehiclesService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleAPILogic _vehicleAPILogic;

        private readonly ILogger _logger;

        public VehiclesController(ILogger logger)
        {
            if (logger == default)
                throw new ArgumentNullException($"Logger cannot be null");

            _vehicleAPILogic = new VehicleAPILogic(logger);
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<VehicleDTO>> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return await _vehicleAPILogic.GetVehicleDTOs(cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task PostAsync([FromBody] VehicleDTO vehicleDTO, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (vehicleDTO.VehicleID == Guid.Empty)
                    throw new ArgumentNullException(paramName: nameof(vehicleDTO), $"VehicleDTO ID is null");

                bool result = await _vehicleAPILogic.AddVehicle(vehicleDTO,cancellationToken);

                if (result)
                    _logger.LogInformation($"Vehicle : {vehicleDTO.VehicleID} was successful added");
                else
                    _logger.LogError($"Error while adding Vehicle : {vehicleDTO.VehicleID}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
