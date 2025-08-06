

namespace data.Services
{
    public interface IPiService
    {
        public Task<bool> IsBlockingAsync();
    }
}