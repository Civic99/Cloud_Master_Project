using Common.DTO;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Validator
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Validator : StatelessService, IValidator
    {
        public Validator(StatelessServiceContext context)
            : base(context)
        { }

        public async Task<StatusCode> RegisterAsync(UserAuthDto user)
        {
            if (!ValidateUserAuth(user)) return StatusCode.BadRequest;

            var fabricClient = new FabricClient();
            var serviceUri = new Uri("fabric:/Project/TransactionCordinator");

            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
            foreach (var partition in partitionList)
            {
                var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                var proxy = ServiceProxy.Create<ITransactionCordinator>(new Uri("fabric:/Project/TransactionCordinator"), partitionKey);

                try
                {
                    var result = await proxy.PrepareRegisterAsync(user);

                    if (!result) return await proxy.CommitRegisterAsync(user);

                    return StatusCode.BadRequest;
                }
                catch (Exception)
                {
                    return await proxy.RollbackAsync();
                }
            }
            return StatusCode.BadRequest;
        }

        public async Task<UserWithOrdersDto> LoginAsync(UserAuthDto user)
        {
            if (!ValidateUserAuth(user)) throw new UnauthorizedAccessException();
            var fabricClient = new FabricClient();
            var serviceUri = new Uri("fabric:/Project/TransactionCordinator");

            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
            foreach (var partition in partitionList)
            {
                var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                var proxy = ServiceProxy.Create<ITransactionCordinator>(new Uri("fabric:/Project/TransactionCordinator"), partitionKey);

                try
                {
                    return await proxy.PrepareLoginAsync(user);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return new UserWithOrdersDto();
        }

        public async Task<StatusCode> CreateOrderAsync(OrderDto order)
        {
            if (!ValidateOrder(order)) return StatusCode.BadRequest;
            var fabricClient = new FabricClient();
            var serviceUri = new Uri("fabric:/Project/TransactionCordinator");

            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
            foreach (var partition in partitionList)
            {
                var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                var proxy = ServiceProxy.Create<ITransactionCordinator>(new Uri("fabric:/Project/TransactionCordinator"), partitionKey);

                try
                {
                    return await proxy.CreateOrderAsync(order); ;
                }
                catch (Exception)
                {
                    return StatusCode.InternalServerError;
                }
            }

            return StatusCode.BadRequest;
        }

        public async Task<List<OrderDto>> GetAllOrderAsync(Guid userId)
        {
            var fabricClient = new FabricClient();
            var serviceUri = new Uri("fabric:/Project/TransactionCordinator");

            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
            foreach (var partition in partitionList)
            {
                var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                var proxy = ServiceProxy.Create<ITransactionCordinator>(new Uri("fabric:/Project/TransactionCordinator"), partitionKey);

                try
                {
                    return await proxy.GetAllOrdersAsync(userId);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return new List<OrderDto>();
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var fabricClient = new FabricClient();
            var serviceUri = new Uri("fabric:/Project/TransactionCordinator");

            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
            foreach (var partition in partitionList)
            {
                var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                var proxy = ServiceProxy.Create<ITransactionCordinator>(new Uri("fabric:/Project/TransactionCordinator"), partitionKey);

                try
                {
                    return await proxy.GetAllProductsAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return new List<ProductDto>();
        }

        public async Task<StatusCode> Pay(Guid orderId, OrderType orderType)
        {
            var fabricClient = new FabricClient();
            var serviceUri = new Uri("fabric:/Project/TransactionCordinator");

            var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
            foreach (var partition in partitionList)
            {
                var partitionKey = new ServicePartitionKey(((Int64RangePartitionInformation)partition.PartitionInformation).LowKey);
                var proxy = ServiceProxy.Create<ITransactionCordinator>(new Uri("fabric:/Project/TransactionCordinator"), partitionKey);

                try
                {
                    return await proxy.PayOrderAsync(orderId, orderType);
                }
                catch (Exception)
                {
                    return StatusCode.InternalServerError;
                }
            }
            return StatusCode.BadRequest;
        }

        private bool ValidateUserAuth(UserAuthDto user)
        {
            if (user is null) return false;
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password)) return false;

            return true;
        }

        private bool ValidateOrder(OrderDto order)
        {
            if (order is null) return false;
            if (!order.Products.Any()) return false;

            return true;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            => this.CreateServiceRemotingInstanceListeners();

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
