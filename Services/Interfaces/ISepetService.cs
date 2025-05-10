using B2BUygulamasi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BUygulamasi.Services
{
    public interface ISepetService
    {
        // Async metodlar
        Task<int> GetSepetItemCountAsync();
        Task<List<SepetItem>> GetSepetAsync();
        Task AddToSepetAsync(int urunId, int adet = 1);
        Task RemoveFromSepetAsync(int urunId);
        Task ClearSepetAsync();

        // Sync metodlar (mümkünse kullanmayın, async tercih edin)
        int GetSepetItemCount();
        List<SepetItem> GetSepet();
    }
}