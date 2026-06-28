namespace PortalIRibeiro.FrontEnd.Model;

public record RequisicaoChat(Guid SessaoId, string Texto);

public record RespostaChat
{
    public string Texto { get; init; } = string.Empty;
    public Guid SessaoId { get; init; }
}