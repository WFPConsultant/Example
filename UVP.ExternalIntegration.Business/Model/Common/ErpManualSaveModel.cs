namespace UVP.ExternalIntegration.Business.Model.Common
{
    public class ErpManualSaveModel
    {
        public long? PersonId { get; set; }

        public string PersonNumber { get; set; }

        public string WorkerHash { get; set; }

        public long? AssignmentId { get; set; }

        public string AssignmentNumber { get; set; }

        public string AssignmentHash { get; set; }

        public long? ContractId { get; set; }

        public string ContractNumber { get; set; }

        public string ContractHash { get; set; }
    }
}
