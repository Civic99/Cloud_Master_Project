using Common.DTO;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using UserModel = Common.Models.User;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Common.Mappers;
using System.Transactions;

namespace User
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class User : StatefulService, IAuth
    {
        public User(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<bool> CheckIfAlreadyExists(UserAuthDto userAuthDto)
        {
            var userDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("Users");
            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerableNew = await userDictionary.CreateEnumerableAsync(transaction);
                var enumerator = enumerableNew.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;
                    if (current.Value.Username.Equals(userAuthDto.Username))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> CheckIfIsAuthorized(UserAuthDto userAuthDto)
        {
            var userDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("Users");

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerableNew = await userDictionary.CreateEnumerableAsync(transaction);
                var enumerator = enumerableNew.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;
                    if (current.Value.Username.Equals(userAuthDto.Username) && current.Value.Password.Equals(userAuthDto.Password))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<StatusCode> Register(UserAuthDto userAuthDto)
        {
            var userDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("Users");

            using (var transaction = StateManager.CreateTransaction())
            {
                await userDictionary.AddAsync(transaction, Guid.NewGuid(), UserAuthMapper.FromDto(userAuthDto));

                await transaction.CommitAsync();
            }

            return StatusCode.Success;
        }

        public async Task<StatusCode> Login(UserAuthDto userAuthDto)
        {
            var userDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("Users");

            //TODO: return dto with current orders

            return StatusCode.Success;
        }

        public async Task<StatusCode> GetPreviousState()
        {
            var userDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("Users");
            var previousUserDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("PreviousUsers");

            using (var transaction = StateManager.CreateTransaction())
            {
                if (await previousUserDictionary.GetCountAsync(transaction) == 0) return StatusCode.NotFound;

                var enumerablePrev = await previousUserDictionary.CreateEnumerableAsync(transaction);
                var enumerator = enumerablePrev.GetAsyncEnumerator();

                var enumerableNew = await userDictionary.CreateEnumerableAsync(transaction);
                var newEnumerator = enumerableNew.GetAsyncEnumerator();

                while (await newEnumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = newEnumerator.Current;
                    await userDictionary.TryRemoveAsync(transaction, current.Key);
                }

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;
                    await previousUserDictionary.AddAsync(transaction, current.Key, current.Value);
                }

                await previousUserDictionary.ClearAsync();

                await transaction.CommitAsync();
            }

            return StatusCode.InternalServerError;
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
