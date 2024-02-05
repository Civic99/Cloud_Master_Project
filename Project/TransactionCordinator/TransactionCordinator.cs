using Common.DTO;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TransactionCordinator
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TransactionCordinator : StatefulService, Common.Interfaces.ITransactionCordinator
    {
        public TransactionCordinator(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<bool> PrepareRegisterAsync(UserAuthDto dto)
        {
            var userAuthProxy = ServiceProxy.Create<IAuth>(new Uri("fabric:/Project/User"), new ServicePartitionKey(1));

            return await userAuthProxy.CheckIfAlreadyExists(dto);
        }

        public async Task<bool> PrepareLoginAsync(UserAuthDto dto)
        {
            var userAuthProxy = ServiceProxy.Create<IAuth>(new Uri("fabric:/Project/User"), new ServicePartitionKey(1));

            return await userAuthProxy.CheckIfIsAuthorized(dto);
        }

        public async Task<StatusCode> CommitRegisterAsync(UserAuthDto dto)
        {
            var userProxy = ServiceProxy.Create<IAuth>(new Uri("fabric:/Project/User"), new ServicePartitionKey(1));

            return await userProxy.Register(dto);
        }

        public async Task<StatusCode> CreateOrderAsync(OrderDto dto)
        {
            var orderProxy = ServiceProxy.Create<IOrder>(new Uri("fabric:/Project/Order"), new ServicePartitionKey(1));

            return await orderProxy.Create(dto);
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync(Guid userId)
        {
            var orderProxy = ServiceProxy.Create<IOrder>(new Uri("fabric:/Project/Order"), new ServicePartitionKey(1));

            return await orderProxy.GetAll(userId);
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var orderProxy = ServiceProxy.Create<IOrder>(new Uri("fabric:/Project/Order"), new ServicePartitionKey(1));

            return await orderProxy.GetAllProducts();
        }

        public async Task<StatusCode> RollbackAsync()
        {
            var userProxy = ServiceProxy.Create<IAuth>(new Uri("fabric:/Project/User"), new ServicePartitionKey(1));

            await userProxy.GetPreviousState();

            return StatusCode.BadRequest;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
            => this.CreateServiceRemotingReplicaListeners();

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
