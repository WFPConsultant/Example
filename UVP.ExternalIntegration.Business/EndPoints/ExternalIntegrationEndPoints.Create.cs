namespace UVP.ExternalIntegration.Business.EndPoints
{
    using System.Net.Http;
    using UVP.ExternalIntegration.Business.Http;
    using UVP.ExternalIntegration.ErrorValidationFramework.Enum;

    /// <summary>
    /// Micro endpoint addresses.
    /// </summary>
    internal partial class ExternalIntegrationEndPoints
    {
        private readonly string RestApiFolder;
        private readonly string PatchRestApiFolder;


        public ExternalIntegrationEndPoints(string restApiFolder)
        {
            this.RestApiFolder = $"hcmRestApi/resources/{restApiFolder}/";
            this.PatchRestApiFolder = $"fscmRestApi/resources/{restApiFolder}/";


        }

        private string GetPath(string path) => this.RestApiFolder + path;

        /// <summary>Endpoint: POST /workers/create | Creates an employee.</summary>
        internal Endpoint CreateEmployee => new Endpoint(method: HttpMethod.Post, path: this.GetPath("workers"), apicode: APICodes.ErpHire.ToString());

        /// <summary>Endpoint: POST /workers/update nationality | Update a nationality.</summary>
        internal Endpoint UpdateNationality => new Endpoint(method: HttpMethod.Post, path: this.GetPath("workers/{0}/child/workersEFF/{1}/child/PersonExtraInformationContextNationality__InformationprivateVO"), apicode: APICodes.ErpNationality.ToString());

        /// <summary>Endpoint: POST /workers/update nationality | Update a nationality.</summary>
        internal Endpoint UpdateAddress => new Endpoint(method: HttpMethod.Post, path: this.GetPath("workers/{0}/child/addresses"), apicode: APICodes.ErpAddress.ToString());

        /// <summary>Endpoint: POST /workers/update Agency | Update agency Info.</summary>
        internal Endpoint UpdateAgency => new Endpoint(method: HttpMethod.Post, path: this.GetPath("workers/{0}/child/workersEFF/{1}/child/PersonExtraInformationContextUNV__Specific__InformationprivateVO"), apicode: APICodes.ErpAgency.ToString());

        /// <summary>Endpoint: POST /workers/update salary | Update salary details.</summary>
        internal Endpoint UpdateSalary => new Endpoint(method: HttpMethod.Post, path: this.GetPath("salaries"), apicode: APICodes.ErpSalaryInfo.ToString());

        /// <summary>Endpoint: POST /workers/update dependents | Update dependents.</summary>
        internal Endpoint UpdateDependents => new Endpoint(method: HttpMethod.Post, path: this.GetPath("hcmContacts"), apicode: APICodes.ErpDepedent.ToString());

        /// <summary>Endpoint: POST /workers/Set External Identifiers.</summary>
        internal Endpoint SetExernalIdentifiers => new Endpoint(method: HttpMethod.Post, path: this.GetPath("workers/{0}/child/externalIdentifiers"), apicode: APICodes.ErpExternalIdentifier.ToString());

        /// <summary>Endpoint: POST /workers/update Chart of Accounts | Update Chart of accounts.</summary>
        internal Endpoint UpdateChartOfAccountsPPM => new Endpoint(method: HttpMethod.Post, path: this.PatchRestApiFolder + "personAssignmentLaborSchedules", apicode: APICodes.ErpCoaPPM.ToString());

        /// <summary>Endpoint: POST /workers/update Chart of Accounts | Update Chart of accounts.</summary>
        internal Endpoint UpdateChartOfAccountsNonPPM => new Endpoint(method: HttpMethod.Post, path: this.GetPath("workers/{0}/child/workersEFF/{1}/child/PersonExtraInformationContextInteragency__CostingprivateVO"), apicode: APICodes.ErpCoaNonPPM.ToString());

        /// <summary>Endpoint: POST /workers/update Chart of Accounts | Update Chart of accounts.</summary>
        internal Endpoint UpdateChartOfAccountsNonQuantum => new Endpoint(method: HttpMethod.Post, path: this.GetPath("workers/{0}/child/workersEFF/{1}/child/PersonExtraInformationContextNationality__InformationprivateVO"), apicode: APICodes.ErpCoaNonQuantum.ToString());

        /// <summary>Endpoint: POST /OTE - One time Payroll Entries to Quantum.</summary>
        internal Endpoint PostOneTimeEntryPayrollToQuantum => new Endpoint(method: HttpMethod.Post, path: this.GetPath("elementEntries"), apicode: APICodes.ErpOTEPayroll.ToString());

        /// <summary>
        /// Endpoint: PPM Version Status- Version status to be set to Active.
        /// </summary>
        internal Endpoint UpdateChartOfAccountsPPMVersionStatus => new Endpoint(method: HttpMethod.Patch, path: this.PatchRestApiFolder + "personAssignmentLaborSchedules/{0}/child/versions/{1}", apicode: APICodes.ErpCoaPPMVersionStatus.ToString());

        /// <summary>
        /// Endpoint: Update assignment status. (From Active to Suspend Payroll)
        /// </summary>
        internal Endpoint UpdateAssignmentStatus => new Endpoint(method: HttpMethod.Patch, path: this.GetPath("workers/{0}/child/workRelationships/{1}/child/assignments/{2}"), apicode: APICodes.ErpUpdateAssignmentStatus.ToString());
    }
}
