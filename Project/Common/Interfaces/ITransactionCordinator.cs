using Common.DTO;
using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface ITransactionCordinator : IService
    {
        [OperationContract]
        Task<bool> PrepareRegisterAsync(UserAuthDto dto);

        [OperationContract]
        Task<bool> PrepareLoginAsync(UserAuthDto dto);

        [OperationContract]
        Task<StatusCode> CommitRegisterAsync(UserAuthDto dto);

        [OperationContract]
        Task<StatusCode> RollbackAsync();

        [OperationContract]
        Task<StatusCode> CreateOrderAsync(OrderDto dto);

        [OperationContract]
        Task<List<OrderDto>> GetAllOrdersAsync(Guid userId);

        [OperationContract]
        Task<List<ProductDto>> GetAllProductsAsync();
    }
}
