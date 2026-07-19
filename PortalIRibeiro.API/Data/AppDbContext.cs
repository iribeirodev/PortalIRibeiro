using Microsoft.EntityFrameworkCore;
using PortalIRibeiro.API.Entities;
using PortalIRibeiro.API.Features.Contato;
using PortalIRibeiro.API.Features.Iris;


namespace PortalIRibeiro.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) =>
        // Garante o comportamento do PostgreSQL para armazenar DateTimeOffset como TIMESTAMPTZ
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<MensagemContato> MensagensContato { get; set; }
    public DbSet<HistoricoConversa> HistoricosConversas { get; set; }
    public DbSet<Parameter> Parameters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamento explícito do Array de strings na tabela Projetos
        modelBuilder.Entity<Projeto>()
            .Property(p => p.Tecnologias)
            .HasColumnType("varchar(50)[]");

        modelBuilder.Entity<HistoricoConversa>()
            .HasIndex(h => h.SessaoId)
            .HasDatabaseName("idx_conversas_sessao");

        modelBuilder.Entity<Parameter>(entity =>
        {
            entity.ToTable("parameters");
            entity.HasKey(e => e.ParamKey);
            entity.Property(e => e.ParamKey).HasColumnName("param_key").HasMaxLength(30);
            entity.Property(e => e.ParamValue).HasColumnName("param_value");
        });
    }
}