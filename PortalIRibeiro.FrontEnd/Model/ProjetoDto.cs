namespace PortalIRibeiro.FrontEnd.Model;

public class ProjetoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string[] Tecnologias { get; set; } = [];
    public string? UrlImagem { get; set; }
    public string? UrlGithub { get; set; }
    public string? UrlDemonstracao { get; set; }
    public DateTimeOffset DataCriacao { get; set; }
}