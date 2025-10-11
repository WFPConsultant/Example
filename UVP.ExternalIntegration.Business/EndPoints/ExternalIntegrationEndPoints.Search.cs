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
        /// <summary>Endpoint: [Get('Worker({FirstName},{LastName},{DateOfBirth},{Gender})'] | Retrieves a worker by firstname, lastname, date of birth and gender.</summary>
        internal Endpoint GetWorkerByName => new Endpoint(method: HttpMethod.Get, path: this.GetPath("workers?q=(upper(names.LastName)={0})  and (upper(names.FirstName)={1}) and (DateOfBirth={2}) and (legislativeInfo.Gender={3})"), apicode: APICodes.ErpCheckPersonExists.ToString());

        internal Endpoint GetPayrollRelationship => new Endpoint(method: HttpMethod.Get, path: this.GetPath("payrollRelationships?finder=findByAssignmentNumberPersonNumber;AssignmentNumber={0}&expand=all"), apicode: APICodes.ErpGetPayrollRelationship.ToString());

        /// <summary>Endpoint: [Get('Worker({PersonNumber})'] | Retrieves a worker by PersonNumber.</summary>
        internal Endpoint GetPersonNumber => new Endpoint(method: HttpMethod.Get, path: this.GetPath("workers?q=PersonNumber={0}&expand=names"), apicode: APICodes.ErpCheckPersonExists.ToString());

        /// <summary>Endpoint: [Get('Worker({AssignmentNumber})'] | Retrieves a worker by AssignmentNumber.</summary>
        internal Endpoint GetQuantumAssignment => new Endpoint(method: HttpMethod.Get, path: this.GetPath("workers?q=workRelationships.assignments.AssignmentNumber={0}&expand=workRelationships.assignments"), apicode: APICodes.ErpGetHireData.ToString());
    }
}
