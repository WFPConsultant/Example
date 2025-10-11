namespace UVP.ExternalIntegration.Business.Utilities
{
    using System.Threading.Tasks;

    public class CommonUtilities 
    {
        private readonly string BaseUrl;

        public CommonUtilities(string baseUrl) => this.BaseUrl = baseUrl;

        public async Task<string> BuildQuantumAssignmentLink(string PersonId, string AssignmentId, string EffectiveDate)
        {
            var link = string.Empty;

            var otherURLPart = "hcmUI/faces/FndOverview?fnd=";
            var part1_fixed = "/WEB-INF/oracle%252Fapps%252Fhcm%252Femployment%252Fmanage%252Fui%252Fflow%252FManageEmploymentTF.xml%2523ManageEmploymentTF%3BassignmentId%253D";

            var effectiveDatePrefix = "%253BeffectiveDate%253D";
            var employment = "%3B%3BEmployment%3Bfalse%3B256%3B%3B%3B&&fndGlobalItemNodeId=itemNode_workforce_management_person_management&pAssignmentId=";
            var paramEffectiveDate = "&pEffectiveDate=";
            var paramPersonId = "&pPersonId=";
            var paramPage = "&pageParams=pPersonId%3D";
            var ppAssignment = "%3BpAssignmentId%3D";
            var ppEffectiveDate = "%3BpEffectiveDate%3D";

            link = this.BaseUrl + otherURLPart + part1_fixed + AssignmentId + effectiveDatePrefix + EffectiveDate + employment + AssignmentId
                                + paramEffectiveDate + EffectiveDate + paramPersonId + PersonId + paramPage + PersonId
                                + ppAssignment + AssignmentId + ppEffectiveDate + EffectiveDate;
            return link;
        }

    }
}
