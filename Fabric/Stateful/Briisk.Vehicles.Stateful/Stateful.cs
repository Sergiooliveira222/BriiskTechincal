using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Briisk.Vehicles.Common;
using Briisk.Vehicles.Common.Interfaces;
using Briisk.Vehicles.Stateful.Repository;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Briisk.Vehicles.Stateful
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Stateful : StatefulService, IVehicleStateful
    {
        private IVehicleRepository _vehicleRepository;

        public Stateful(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<bool> AddVehicleAsync(VehicleModel vehicleModel, CancellationToken cancellationToken)
        {
            return await _vehicleRepository.AddVehicle(vehicleModel, cancellationToken);
        }

        public async Task<List<VehicleModel>> GetAllVehiclesAsync(CancellationToken cancellationToken)
        {
            return (await _vehicleRepository.GetVehicles(cancellationToken)).ToList();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(Context =>
                new FabricTransportServiceRemotingListener(Context,this))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _vehicleRepository = new VehicleRepository(this.StateManager);

            var car = new VehicleModel
            {
                VehicleID = Guid.NewGuid(),
                Model = "Polo",
                Make = "VW",
                Color = "Blue",
                Description = "Really Bad condition",
                Price = 10000
            };

            await _vehicleRepository.AddVehicle(car, cancellationToken);

            IEnumerable<VehicleModel> all = await _vehicleRepository.GetVehicles(cancellationToken);
        }
    }
}
