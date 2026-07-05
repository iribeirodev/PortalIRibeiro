namespace PortalIRibeiro.API.Features.JobScraper.Enums;

public enum StatusTriagem
{
    /// <summary>
    /// Vaga capturada pelo RSS que passou pelo filtro estático e aguarda o envio para o Gemini.
    /// </summary>
    AguardandoAnalise = 0,

    /// <summary>
    /// Vaga analisada pela IA e classificada com alta aderência ao seu perfil.
    /// </summary>
    AltaAderencia = 1,

    /// <summary>
    /// Vaga analisada pela IA e classificada com média/aceitável aderência.
    /// </summary>
    MediaAderencia = 2,

    /// <summary>
    /// Reprovada sumariamente na RN01 pelos filtros de string (evita custo com a API do Gemini).
    /// </summary>
    ReprovadoFiltroEstatico = 3,

    /// <summary>
    /// Analisada pelo Gemini, mas o Score/Justificativa determinou que não serve (Baixa Aderência).
    /// </summary>
    ReprovadoIA = 4
}