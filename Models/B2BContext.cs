using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BUygulamasi.Data
{
    public class B2BContext : DbContext
    {
        public B2BContext(DbContextOptions<B2BContext> options) : base(options) { }

        public virtual DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
        public virtual DbSet<Rol> Roller => Set<Rol>();
        public virtual DbSet<KullaniciRol> KullaniciRolleri => Set<KullaniciRol>();
        public virtual DbSet<Urun> Urunler => Set<Urun>();
        public virtual DbSet<Siparis> Siparisler => Set<Siparis>();
        public virtual DbSet<SiparisDetay> SiparisDetaylar => Set<SiparisDetay>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal hassasiyet ayarları
            modelBuilder.Entity<Urun>(entity =>
            {
                entity.Property(e => e.Fiyat)
                      .HasPrecision(18, 6)
                      .IsRequired();
            });

            modelBuilder.Entity<SiparisDetay>(entity =>
            {
                entity.Property(e => e.BirimFiyat)
                      .HasPrecision(18, 6)
                      .IsRequired();
            });

            modelBuilder.Entity<Siparis>(entity =>
            {
                entity.Property(e => e.ToplamTutar)
                      .HasPrecision(18, 6);
            });

            // İlişkisel konfigürasyonlar
            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Siparis)
                .WithMany(s => s.SiparisDetaylar)
                .HasForeignKey(sd => sd.SiparisId);

            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Urun)
                .WithMany()
                .HasForeignKey(sd => sd.UrunId);

            // Kullanıcı-Rol ilişkileri
            modelBuilder.Entity<KullaniciRol>()
                .HasKey(kr => new { kr.KullaniciID, kr.RolID });

            modelBuilder.Entity<KullaniciRol>()
                .HasOne(kr => kr.Kullanici)
                .WithMany(k => k.KullaniciRolleri)
                .HasForeignKey(kr => kr.KullaniciID);

            modelBuilder.Entity<KullaniciRol>()
                .HasOne(kr => kr.Rol)
                .WithMany(r => r.KullaniciRolleri)
                .HasForeignKey(kr => kr.RolID);

            // Seed data
            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolID = 1, RolAdi = "Admin" },
                new Rol { RolID = 2, RolAdi = "Standart" },
                new Rol { RolID = 3, RolAdi = "FirmaAdmin" }
            );
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Tüm decimal property'ler için global ayar
            configurationBuilder.Properties<decimal>()
                               .HavePrecision(18, 6);
        }
    }
}