namespace UVP.ExternalIntegration.ErrorValidationFramework.Repository
{
    using System.Data;
    using System.Threading.Tasks;
    using Koa.Platform.Injection;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using UVP.ExternalIntegration.ErrorValidationFramework.Model;

    [Injectable(typeof(IDBLoggerRepository))]
    public class DBLoggerRepository : IDBLoggerRepository
    {
        private readonly ErrorDBContext dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBLoggerRepository"/> class.
        /// </summary>
        /// <param name="dataContext">DataContext.</param>
        public DBLoggerRepository(ErrorDBContext errorDbContext)
        {
            this.dataContext = errorDbContext;
        }
        public async Task<ReturnMessage> CreateAPITransactionAsyc(APITransaction transaction, string apiAction)
        {

            var transactionId = new SqlParameter("@p_TransactionId", transaction.Id);
            var apiCode = new SqlParameter("@p_APICode", transaction.APIcode);
            var statusCode = new SqlParameter("@p_StatusCode", transaction.APIStatusCode);
            var errorCode = new SqlParameter("@p_ErrorCode", transaction.APIErrorCode);
            var action = new SqlParameter("@p_Action", apiAction);
            var requestObject = new SqlParameter("@p_RequestObject", transaction.RequestObject);
            var responseObject = new SqlParameter("@p_ResponseObject", transaction.ResponseObject);
            var initialStatusCode = new SqlParameter("@p_InitialStatusCode", transaction.InitialStatusId);
            var isReTriggered = new SqlParameter("@p_IsReTriggered", transaction.IsReTriggered);
            var latestRequestObject = new SqlParameter("@p_LatestRequestObject", transaction.LatestRequestObject);
            var lastestResponseObject = new SqlParameter("@p_LastestResponseObject", transaction.LatestResponseObject);
            var user = new SqlParameter("@p_User", "System");
            var doaCandidateId = new SqlParameter("@p_DoaCandidateId", transaction.DoaCandidateId);

            // OUT PARAMETERS
            var returnCode = new SqlParameter("@p_ReturnCode", SqlDbType.Int);
            var returnMessage = new SqlParameter("@p_ReturnMessage", SqlDbType.VarChar, 100);
            var newTransactionId = new SqlParameter("@p_NewTransactionId", SqlDbType.VarChar, 100);

            returnCode.Direction = ParameterDirection.Output;
            returnMessage.Direction = ParameterDirection.Output;
            newTransactionId.Direction = ParameterDirection.Output;


            await this.dataContext.Database.ExecuteSqlRawAsync(
            "exec [dbo].[Erp_SaveAPITransaction] @p_TransactionId, @p_APICode, @p_StatusCode, @p_ErrorCode, @p_Action, @p_RequestObject, @p_ResponseObject " +
            " , @p_InitialStatusCode, @p_IsReTriggered, @p_LatestRequestObject, @p_LastestResponseObject, @p_User, @p_DoaCandidateId, @p_ReturnCode out, @p_ReturnMessage out, @p_NewTransactionId out ",
            transactionId,
            apiCode,
            statusCode,
            errorCode,
            action,
            requestObject,
            responseObject,
            initialStatusCode,
            isReTriggered,
            latestRequestObject,
            lastestResponseObject,
            user,
            doaCandidateId,
            returnCode,
            returnMessage,
            newTransactionId);


            //await this.dataContext.Database.ExecuteSqlRawAsync($"[dbo].[Erp_SaveAPITransaction] {transaction.Id} ");

            ReturnMessage returnObject = new ReturnMessage();
            returnObject.MessageText = returnMessage.Value.ToString();
            returnObject.MessageCode =  int.Parse( returnCode.Value.ToString());
            returnObject.NewId = newTransactionId.Value.ToString();


            return returnObject;
        }
    }
}
