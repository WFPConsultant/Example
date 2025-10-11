namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class FundingDetails
    {
        /// <summary>
        /// Gets or sets CostType.
        /// </summary>
        public string? CostType { get; set; }

        /// <summary>
        /// Gets or sets VersionID.
        /// </summary>
        public long? VersionID { get; set; }

        /// <summary>
        /// Gets or sets VersionStatus.
        /// </summary>
        public string? VersionStatus { get; set; }

        /// <summary>
        /// Gets or sets Award_Number.
        /// </summary>
        public string? Award_Number { get; set; }

        /// <summary>
        /// Gets or sets Award_Name.
        /// </summary>
        public string? Award_Name { get; set; }

        /// <summary>
        /// Gets or sets Sponsor_Name.
        /// </summary>
        public string? Sponsor_Name { get; set; }

        /// <summary>
        /// Gets or sets project_Number.
        /// </summary>
        public string? Project_Number { get; set; }

        /// <summary>
        /// Gets or sets Project_Name.
        /// </summary>
        public string? Project_Name { get; set; }

        /// <summary>
        /// Gets or sets Task_Name.
        /// </summary>
        public string? Task_Name { get; set; }

        /// <summary>
        /// Gets or sets Task_Number.
        /// </summary>
        public string? Task_Number { get; set; }

        /// <summary>
        /// Gets or sets Expenditure_Type_Name  .
        /// </summary>
        public string? Expenditure_Type_Name { get; set; }

        /// <summary>
        /// Gets or sets Org_Name.
        /// </summary>
        public string? Org_Name { get; set; }

        /// <summary>
        /// Gets or sets Percentage.
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// Gets or sets FundCode.
        /// </summary>
        public string? FundCode { get; set; }

        /// <summary>
        /// Gets or sets OperatingUnit.
        /// </summary>
        public string? OperatingUnit { get; set; }

        /// <summary>
        /// Gets or sets Agency.
        /// </summary>
        public string? Agency { get; set; }

        /// <summary>
        /// Gets or sets Donor.
        /// </summary>
        public string? Donor { get; set; }

        /// <summary>
        /// Gets or sets CostCenter.
        /// </summary>
        public string? CostCenter { get; set; }

        /// <summary>
        /// Gets or sets Project.
        /// </summary>
        public string? Project { get; set; }

        /// <summary>
        /// Gets or sets Interagency.
        /// </summary>
        public string? Interagency { get; set; }

        /// <summary>
        /// Gets or sets Future.
        /// </summary>
        public string? Future { get; set; }
    }
}
