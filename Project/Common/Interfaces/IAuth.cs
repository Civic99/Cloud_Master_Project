using Common.DTO;
using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface IAuth : IService
    {
        [OperationContract]
        Task<StatusCode> Register(UserAuthDto userAuthDto);

        [OperationContract]
        Task<bool> CheckIfAlreadyExists(UserAuthDto userAuthDto);

        [OperationContract]
        Task<StatusCode> GetPreviousRegisterState();
    }
}
