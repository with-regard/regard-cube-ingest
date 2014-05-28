using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorkerRoleWithSBQueue1
{
    public class CubePoster : ICubePoster
    {
        private readonly string m_CubeEndpoint;

        public CubePoster(string cubeEndpoint)
        {
            m_CubeEndpoint = cubeEndpoint;
        }

        public async Task Post(string body)
        {
            var build = new CubeMessageFactory().Build(body);

            using (var httpClient = new HttpClient())
            {
                using (var stringContent = new StringContent(build))
                {
                    var httpResponseMessage = await httpClient.PostAsync(m_CubeEndpoint, stringContent);

                    if (!httpResponseMessage.IsSuccessStatusCode)
                        Trace.WriteLine("Unable to write event");
                }
            }
        }
    }
}