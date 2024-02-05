using Common.DTO;
using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System.ServiceModel;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface IOrder : IService
    {
        [OperationContract]
        Task<StatusCode> Create(OrderDto dto);

        [OperationContract]
        Task<List<OrderDto>> GetAll(Guid userId);

        [OperationContract]
        Task<List<ProductDto>> GetAllProducts();
    }
}
