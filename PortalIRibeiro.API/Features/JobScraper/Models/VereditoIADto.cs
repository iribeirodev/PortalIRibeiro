namespace PortalIRibeiro.API.Features.JobScraper.Models;

public class VereditoIADto
{
    public int Score { get; set; }
    public string Justificativa { get; set; } = string.Empty;
    public List<string> Gaps { get; set; } = [];
}