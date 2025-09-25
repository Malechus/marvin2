using marvin2.Models.PiModels;

namespace data.Services
{
    public interface IPiService
    {
        public bool IsBlocking();
        public List<QueryClient> GetTopClients();
        public List<QueryClient> GetTopBlockedClients();
    }
}