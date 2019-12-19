using Briisk.Vehicles.Common;
using Briisk.Vehicles.Common.Interfaces;
using Briisk.Vehicles.Stateful.Constants;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Briisk.Vehicles.Stateful.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly IReliableStateManager _stateManager;

        public VehicleRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task<bool> AddVehicle(VehicleModel vehicleModel, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                IReliableDictionary<Guid, VehicleModel> vehicleDictionary = await GetVehicleModelsFromReliableStateAsync();

                using (ITransaction tx = _stateManager.CreateTransaction())
                {
                    var res = await vehicleDictionary.AddOrUpdateAsync
                        (tx: tx,
                        key: vehicleModel.VehicleID,
                        addValue: vehicleModel,
                        updateValueFactory: (key, value) => VehicleModelUpdateFactory(value, vehicleModel));

                    await tx.CommitAsync();

                };

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<VehicleModel>> GetVehicles(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                IReliableDictionary<Guid, VehicleModel> vehicleDictionary = await GetVehicleModelsFromReliableStateAsync();

                List<VehicleModel> vehicleModels = new List<VehicleModel>();

                using (ITransaction tx = _stateManager.CreateTransaction())
                {
                    Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, VehicleModel>> allVehicles =
                        await vehicleDictionary.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                    using (Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, VehicleModel>> enumertor =
                        allVehicles.GetAsyncEnumerator())
                    {
                        while (await enumertor.MoveNextAsync(CancellationToken.None))
                        {
                            KeyValuePair<Guid, VehicleModel> current = enumertor.Current;
                            vehicleModels.Add(current.Value);
                        }
                    }
                }

                return vehicleModels;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal async Task<IReliableDictionary<Guid, VehicleModel>> GetVehicleModelsFromReliableStateAsync()
        {
            return await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, VehicleModel>>(DictionaryNames.RegisteredVehicleModelsDictionary);
        }

        internal VehicleModel VehicleModelUpdateFactory(VehicleModel vehicleModel, VehicleModel newVehicleModel)
        {
            try
            {
                if (vehicleModel == null && newVehicleModel != null)
                {
                    vehicleModel = newVehicleModel;

                    return vehicleModel;
                }
                else if (vehicleModel == null && newVehicleModel == null)
                    throw new ArgumentNullException(message: "Both VehicleModel are Null", paramName: nameof(VehicleModel));

                if (vehicleModel.VehicleID != newVehicleModel.VehicleID)
                    throw new InvalidOperationException(message: $"VehicleModel ID's are different [Original : {vehicleModel.VehicleID}] [New : {newVehicleModel.VehicleID}]");
                else
                    return newVehicleModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
