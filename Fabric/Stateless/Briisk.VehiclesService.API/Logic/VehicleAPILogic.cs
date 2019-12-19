using Briisk.Vehicles.Common;
using Briisk.Vehicles.Common.Constants;
using Briisk.Vehicles.Common.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Briisk.VehiclesService.API.Logic
{
    public class VehicleAPILogic : IVehicleAPILogic
    {
        private readonly IVehicleStateful _vehicleStateful;

        private ILogger _logger;


        public VehicleAPILogic(ILogger logger)
        {
            if (logger == default)
                throw new ArgumentNullException($"Logger cannot be null");

            _logger = logger;

            var proxyFactory = new ServiceProxyFactory(
                c => new FabricTransportServiceRemotingClientFactory());

            Uri vehicleStatefulSericeUri = new Uri(Environment.GetEnvironmentVariable(EnvironmentVariables.VehicleStateful));

            _vehicleStateful = proxyFactory.CreateServiceProxy<IVehicleStateful>(
                vehicleStatefulSericeUri,
                new ServicePartitionKey(0));
        }


        public async Task<List<VehicleDTO>> GetVehicleDTOs(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogInformation("Call to Vehicle Stateful");

                List<VehicleModel> vehicleModels = await _vehicleStateful.GetAllVehiclesAsync(cancellationToken);

                if (vehicleModels.Count() == 0)
                    _logger.LogInformation("No vehicles is reliable storage");

                List<VehicleDTO> vehicleDTOs = new List<VehicleDTO>();

                foreach (VehicleModel vehicleModel in vehicleModels)
                {
                    vehicleDTOs.Add(
                        MapToVehicleDTO(vehicleModel)
                        );
                }

                return vehicleDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<bool> AddVehicle(VehicleDTO vehicleDTO, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

               bool result = await _vehicleStateful.AddVehicleAsync(
                    MapToVehicleModel(vehicleDTO),
                    cancellationToken
                    );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

        }

        private VehicleDTO MapToVehicleDTO(VehicleModel vehicleModel)
        {
            return new VehicleDTO
            {
                VehicleID = vehicleModel.VehicleID,
                Color = vehicleModel.Color,
                Description = vehicleModel.Description,
                Make = vehicleModel.Make,
                Model = vehicleModel.Model,
                Price = vehicleModel.Price
            };

        }

        private VehicleModel MapToVehicleModel(VehicleDTO vehicleDTO)
        {
            return new VehicleModel
            {
                VehicleID = vehicleDTO.VehicleID,
                Color = vehicleDTO.Color,
                Description = vehicleDTO.Description,
                Make = vehicleDTO.Make,
                Model = vehicleDTO.Model,
                Price = vehicleDTO.Price
            };

        }
    }
}
