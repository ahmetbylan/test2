using Microsoft.EntityFrameworkCore;
using B2BUygulamasi.Models;

namespace B2BUygulamasi.Data
{
    public  class  B2BContext : DbContext
    {
        public  B2BContext(DbContextOptions<B2BContext> options) : base(options) { }

        public virtual DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
        public virtual DbSet<Rol> Roller => Set<Rol>();
        public virtual DbSet<KullaniciRol> KullaniciRolleri => Set<KullaniciRol>();
        // ÜRÜN LİSTELEME
        public virtual DbSet<Urun> Urunler => Set<Urun>();
        // Yeni eklenen DbSet'ler
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylar { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Siparis ilişkileri
            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Siparis)
                .WithMany(s => s.SiparisDetaylar)
                .HasForeignKey(sd => sd.SiparisId);

            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Urun)
                .WithMany()
                .HasForeignKey(sd => sd.UrunId);
            // KullaniciRol için composite primary key
            modelBuilder.Entity<KullaniciRol>()
                .HasKey(kr => new { kr.KullaniciID, kr.RolID });

            // Kullanici ile KullaniciRol ilişkisi
            modelBuilder.Entity<KullaniciRol>()
                .HasOne(kr => kr.Kullanici)
                .WithMany(k => k.KullaniciRolleri)
                .HasForeignKey(kr => kr.KullaniciID);

            // Rol ile KullaniciRol ilişkisi
            modelBuilder.Entity<KullaniciRol>()
                .HasOne(kr => kr.Rol)
                .WithMany(r => r.KullaniciRolleri)
                .HasForeignKey(kr => kr.RolID);

            // Roller için seed data
            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolID = 1, RolAdi = "Admin" },
                new Rol { RolID = 2, RolAdi = "Standart" },
                new Rol { RolID = 3, RolAdi = "FirmaAdmin" }
            );


        }
    }
}