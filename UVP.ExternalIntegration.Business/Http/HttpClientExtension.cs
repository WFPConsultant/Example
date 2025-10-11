namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// HttpClient extension class.
    /// </summary>
    public static class HttpClientExtension
    {
        /// <summary>
        /// Creates a HttpRequestMessage.
        /// </summary>
        /// <param name="httpClient">HttpClient.</param>
        /// <param name="method">HTTP Method.</param>
        /// <param name="path">Path.</param>
        /// <param name="segments">Segments of the path.</param>
        /// <param name="parameters">QueryString parameters.</param>
        /// <param name="content">Content.</param>
        /// <param name="headers">Headers.</param>
        /// <returns><see cref="HttpRequestMessage"/>.</returns>
        public static async Task<HttpResponseMessage> SendClientAsync(
            this HttpClient httpClient,
            HttpMethod method,
            string path,
            string[] segments = null,
            NameValueCollection parameters = null,
            HttpContent content = null,
            IDictionary<string, string> headers = null)
        {
            // Add Segments to Path
            if (segments != null)
            {
                path = string.Format(path, segments);
            }

            // Add QueryString to Path
            if (parameters != null && parameters.Count > 0)
            {
                var queryStringCollection = HttpUtility.ParseQueryString(string.Empty);
                queryStringCollection.Add(parameters);
                path += string.Concat('?', queryStringCollection.ToString());
            }

            var request = new HttpRequestMessage() { Method = method, RequestUri = new Uri(path, UriKind.Relative) };

            // Add Headers
            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }


            //request.Headers.Add("Accept", "*/*");
            //request.Headers.Add("Accept-Encoding", "gzip,deflate,br");
            //request.Headers.Add("Cache-Control", "no-cache");
            //request.Headers.Add("REST-Framework-Version", "4");

            if (content != null)
            {
                request.Content = content;
                request.Content.Headers.ContentType = Headers.MediaTypeHeaderValue.Parse("application/json");
            }



            return await httpClient.SendAsync(request);
        }
    }
}
