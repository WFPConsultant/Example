namespace UVP.ExternalIntegration.Business.Http
{
    using System.Net.Http;

    /// <summary>
    /// Endpoint.
    /// </summary>
    public struct Endpoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> struct.
        /// </summary>
        /// <param name="method">Method.</param>
        /// <param name="path">Path.</param>
        public Endpoint(HttpMethod method, string path, string apicode)
        {
            this.Method = method;
            this.Path = path;
            this.APICode = apicode;
        }

        /// <summary>
        /// Gets method of the endpoint.
        /// </summary>
        public HttpMethod Method { get; private set; }

        /// <summary>
        /// Gets path of the endpoint.
        /// </summary>
        public string Path { get; private set; }

        public string APICode { get; private set; }

        /// <summary>
        /// Get the endpoint info.
        /// </summary>
        /// <returns>[{METHOD}('{PATH}')].</returns>
        public override string ToString() => this.ToString();

        /// <summary>
        /// Get the endpoint info.
        /// </summary>
        /// <param name="segments">Path segments.</param>
        /// <returns>[{METHOD}('{PATH}')].</returns>
        public string ToString(params string[] segments) => string.Format($"[{this.Method}('{this.PathFormat(segments)})']");

        /// <summary>
        /// Apply an string format to the path in order to add the segments.
        /// </summary>
        /// <param name="segments">Segment o replace into the path string.</param>
        /// <returns>Path already formatted containing its segments.</returns>
        public string PathFormat(params string[] segments) => string.Format(this.Path, segments);
    }
}
