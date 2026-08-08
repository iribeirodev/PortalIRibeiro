namespace PortalIRibeiro.API.Entities;

public class Visita
{
    public int Id { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Regiao { get; set; } = string.Empty;
    public string Pagina { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public DateTime DataAcesso { get; set; } = DateTime.UtcNow;
}