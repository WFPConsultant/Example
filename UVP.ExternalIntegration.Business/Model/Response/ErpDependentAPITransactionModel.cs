namespace UVP.ExternalIntegration.Business.Model.Response
{
    using System;

    public class ErpDependentAPITransactionModel
    {
        public Guid Pk_TransactionId { get; set; }
        public string StatusCode { get; set; }
        public DependentResponseModel Dependent { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
