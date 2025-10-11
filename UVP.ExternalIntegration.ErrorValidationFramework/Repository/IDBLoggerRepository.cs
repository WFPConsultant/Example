namespace UVP.ExternalIntegration.ErrorValidationFramework.Repository
{
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.ErrorValidationFramework.Model;

    public interface IDBLoggerRepository
    {
        Task<ReturnMessage> CreateAPITransactionAsyc(APITransaction transaction, string action);

     }
}
