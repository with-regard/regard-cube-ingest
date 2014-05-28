using System.Threading.Tasks;

namespace WorkerRoleWithSBQueue1
{
    public interface ICubePoster
    {
        Task Post(string body);
    }
}