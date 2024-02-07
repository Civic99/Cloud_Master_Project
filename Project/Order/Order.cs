using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Common;
using Common.DTO;
using Common.Entities;
using Common.Interfaces;
using Common.Mappers;
using Common.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using OrderModel = Common.Models.Order;
using ProductModel = Common.Models.Product;

namespace Order
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Order : StatefulService, IOrder
    {
        public Order(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
            => this.CreateServiceRemotingReplicaListeners();

        public async Task<StatusCode> Create(OrderDto dto)
        {
            var orderDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, OrderModel>>("Orders");

            using (var transaction = StateManager.CreateTransaction())
            {
                var proxy = ServiceProxy.Create<ITransactionCordinator>(new Uri("fabric:/Project/TransactionCordinator"), new ServicePartitionKey(2));
                var productsDto = await proxy.GetAllProductsAsync();

                foreach (var product in productsDto)
                {
                    foreach (var orderProduct in dto.Products)
                    {
                        if (product.Id.Equals(orderProduct.Key))
                        {
                            if (product.Quantity < orderProduct.Value)
                            {
                                return StatusCode.BadRequest;
                            }

                            product.Quantity -= orderProduct.Value;
                        }
                    }

                }
                var products = new List<Product>();

                await orderDictionary.AddAsync(transaction, Guid.NewGuid(), OrderMapper.FromDto(dto));
                productsDto.ForEach(p => products.Add(ProductMapper.FromDto(p)));
                await UpdateProducts(products);

                await transaction.CommitAsync();
            }

            return StatusCode.Success;
        }

        public async Task<StatusCode> Pay(Guid id, OrderType orderType)
        {
            var orderDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, OrderModel>>("Orders");

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerableNew = await orderDictionary.CreateEnumerableAsync(transaction);
                var enumerator = enumerableNew.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;
                    if (current.Value.Id.Equals(id))
                    {
                        var paidOrder = current.Value;
                        paidOrder.OrderStatus = OrderStatus.Paid;
                        paidOrder.OrderType = orderType;
                        await orderDictionary.AddOrUpdateAsync(transaction, current.Key, current.Value, (k, v) => paidOrder);
                    }
                }

                await transaction.CommitAsync();
            }

            return StatusCode.Success;
        }

        public async Task<List<OrderDto>> GetAll(Guid userId)
        {
            var orderDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, OrderModel>>("Orders");

            var orders = new List<OrderModel>();
            var ordersDto = new List<OrderDto>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerableNew = await orderDictionary.CreateEnumerableAsync(transaction);
                var enumerator = enumerableNew.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;
                    if (current.Value.UserId.Equals(userId))
                    {
                        orders.Add(current.Value);
                    }
                }

                orders.ForEach(o => ordersDto.Add(OrderMapper.ToDto(o)));
            }

            return ordersDto;
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var productDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, ProductModel>>("Products");

            var products = new List<ProductModel>();
            var productsDto = new List<ProductDto>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerableNew = await productDictionary.CreateEnumerableAsync(transaction);
                var enumerator = enumerableNew.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;
                    products.Add(current.Value);
                }

                products.ForEach(o => productsDto.Add(ProductMapper.ToDto(o)));
            }

            return productsDto;
        }

        private async Task UpdateProducts(List<ProductModel> products)
        {
            var productDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, ProductModel>>("Products");

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerableNew = await productDictionary.CreateEnumerableAsync(transaction);
                var enumerator = enumerableNew.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;
                    foreach( var item in products)
                    {
                        if (current.Value.Id.Equals(item.Id))
                        {
                            await productDictionary.AddOrUpdateAsync(transaction, current.Key, current.Value, (k, v) => item);
                        }
                    }
                }

                await transaction.CommitAsync();
            }

        }

        public async Task InitializeAsync()
        {
            List<Product> products = new()
            {
                new Product { Id = Guid.NewGuid(), Name = "Buritto", Category = "Mrsno", Description = "Lep bas burito", Price = 500, Quantity = 10 },
                new Product { Id = Guid.NewGuid(), Name = "Churros", Category = "Mrsno", Description = "Lep bas churros", Price = 250, Quantity = 11 },
                new Product { Id = Guid.NewGuid(), Name = "Chips", Category = "Posno", Description = "Lep bas chips", Price = 150, Quantity = 12 },
                new Product { Id = Guid.NewGuid(), Name = "Nachos", Category = "Posno", Description = "Lep bas nachos", Price = 300, Quantity = 13 },
            };

            var productDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("Products");

            using var transaction = StateManager.CreateTransaction();

            foreach (var product in products)
            {
                await productDictionary.AddOrUpdateAsync(transaction, product.Id, product, (k, v) => product);
            }

            await transaction.CommitAsync();
        }

        public async Task Migrate()
        {
            var tableClient = AzureTable.GetTableClient("Orders");
            var orderDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, OrderModel>>("Orders");

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerableNew = await orderDictionary.CreateEnumerableAsync(transaction);
                var enumerator = enumerableNew.GetAsyncEnumerator();

                if (await orderDictionary.GetCountAsync(transaction) < 1)
                {
                    var orders = tableClient.Query<OrderEntity>();

                    if (orders.Count() > 0)
                    {
                        foreach (var order in orders)
                        {
                            await orderDictionary.AddAsync(transaction, Guid.Parse(order.RowKey), OrderEntityMapper.FromEntity(order));
                        }

                        await transaction.CommitAsync();
                        return;
                    }
                }

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;

                    try
                    {
                        await tableClient.UpsertEntityAsync(OrderEntityMapper.ToEntity(current.Value));
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            Thread.Sleep(3000);
        }

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

            await InitializeAsync();

            while (true)
            {
                await Migrate();

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
