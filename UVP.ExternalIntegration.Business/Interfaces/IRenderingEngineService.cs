namespace UVP.ExternalIntegration.Business.Interfaces
{
    using System.Threading.Tasks;

    public interface IRenderingEngineService
    {
        Task<string> RenderPayloadAsync(string template, object model);
        Task<string> RenderUrlAsync(string baseUrl, string pathTemplate, object model);
        Task<string> RenderPathParametersAsync(string pathTemplate, object model);
    }
}
