namespace PortalIRibeiro.API.Features.Iris;

public class RequisicaoChat
{
    public Guid SessaoId { get; set; }
    public string Texto { get; set; } = string.Empty;
}

public class RespostaChat
{
    public Guid SessaoId { get; set; }
    public string Texto { get; set; } = string.Empty;
}