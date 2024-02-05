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
    public interface IValidator : IService
    {
        [OperationContract]
        Task<StatusCode> RegisterAsync(UserAuthDto user);

        [OperationContract]
        Task<StatusCode> LoginAsync(UserAuthDto user);

        [OperationContract]
        Task<StatusCode> CreateOrderAsync(OrderDto order);

        [OperationContract]
        Task<List<OrderDto>> GetAllOrderAsync(Guid userId);

        [OperationContract]
        Task<List<ProductDto>> GetAllProductsAsync();
    }
}
