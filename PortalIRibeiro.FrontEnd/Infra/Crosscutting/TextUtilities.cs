using Microsoft.AspNetCore.Components;
using Markdig;

namespace PortalIRibeiro.FrontEnd.Infra.Crosscutting;


public static class TextUtilities
{
    public static MarkupString ConvertMarkdownToHtml(string markdownText)
    {
        if (string.IsNullOrWhiteSpace(markdownText)) 
            return (MarkupString)string.Empty;

        // Configura o Markdig para usar recursos avançados
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        
        // Converte Markdown para HTML string
        var htmlContent = Markdown.ToHtml(markdownText, pipeline);
        
        // Retorna como MarkupString para o Blazor renderizar as tags HTML com segurança
        return (MarkupString)htmlContent;
    }
}