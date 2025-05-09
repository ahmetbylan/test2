using B2BUygulamasi.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace B2BUygulamasi.Services
{
    public class SepetService : ISepetService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SepetService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<SepetItem> GetSepet()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var sessionData = session.GetString("Sepet");
            return string.IsNullOrEmpty(sessionData)
                ? new List<SepetItem>()
                : JsonSerializer.Deserialize<List<SepetItem>>(sessionData);
        }

        public void AddToSepet(int urunId, int adet = 1)
        {
            var sepet = GetSepet();
            var item = sepet.FirstOrDefault(s => s.UrunId == urunId);

            if (item != null)
            {
                item.Adet += adet;
            }
            else
            {
                sepet.Add(new SepetItem { UrunId = urunId, Adet = adet });
            }

            SaveSepet(sepet);
        }

        public void RemoveFromSepet(int urunId)
        {
            var sepet = GetSepet();
            var item = sepet.FirstOrDefault(s => s.UrunId == urunId);

            if (item != null)
            {
                sepet.Remove(item);
                SaveSepet(sepet);
            }
        }

        public void ClearSepet()
        {
            _httpContextAccessor.HttpContext.Session.Remove("Sepet");
        }

        public int GetSepetItemCount()
        {
            return GetSepet().Sum(item => item.Adet);
        }

        private void SaveSepet(List<SepetItem> sepet)
        {
            _httpContextAccessor.HttpContext.Session.SetString("Sepet",
                JsonSerializer.Serialize(sepet));
        }
    }
}