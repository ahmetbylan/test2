using B2BUygulamasi.Models;

namespace B2BUygulamasi.Services
{
    public interface ISepetService
    {
        List<SepetItem> GetSepet();
        void AddToSepet(int urunId, int adet = 1);
        void RemoveFromSepet(int urunId);
        void ClearSepet();
        int GetSepetItemCount();
    }
}