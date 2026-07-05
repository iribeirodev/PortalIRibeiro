using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using PortalIRibeiro.API.Data;

namespace PortalIRibeiro.API.Features.JobScraper.Services;

public class EmailService(
    IServiceScopeFactory scopeFactory,
    ILogger<EmailService> logger
    ) : IEmailService
{
    public async Task EnviarAlertaVagaAsync(VagaTriada vaga)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Busca todas as configurações de e-mail de uma só vez do banco de dados
            var chavesEmail = new[] { "Email:SmtpHost", "Email:SmtpPort", "Email:SmtpUser", "Email:SmtpPass", "Email:EmailDestino", "Email:TemplateAlertaVaga" };
            
            var parametros = await context.Parameters
                .Where(p => chavesEmail.Any(chave => chave == p.ParamKey))
                .ToDictionaryAsync(p => p.ParamKey, p => p.ParamValue);

            var smtpHost = parametros.GetValueOrDefault("Email:SmtpHost", "");
            var smtpPort = int.Parse(parametros.GetValueOrDefault("Email:SmtpPort", "587"));
            var smtpUser = parametros.GetValueOrDefault("Email:SmtpUser", "");
            var smtpPass = parametros.GetValueOrDefault("Email:SmtpPass", "");
            var emailDestino = parametros.GetValueOrDefault("Email:EmailDestino", "");

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(emailDestino))
            {
                logger.LogWarning("Configurações de e-mail incompletas na tabela de parâmetros. O envio foi abortado.");
                return;
            }

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Portal IRibeiro - JobScraper", "ribeirojr.itamar@gmail.com")); //smtpUser));
            mensagem.To.Add(new MailboxAddress("Itamar Ribeiro", emailDestino));
            mensagem.Subject = $"🚨 Oportunidade de Alta Aderência ({vaga.ScoreAderencia}%): {vaga.TituloVaga}";

            var htmlTemplate = parametros.GetValueOrDefault("Email:TemplateAlertaVaga", "Vaga: {TituloVaga} - Score: {ScoreAderencia}%");
            var htmlFinal = htmlTemplate
                .Replace("{TituloVaga}", vaga.TituloVaga)
                .Replace("{ScoreAderencia}", vaga.ScoreAderencia.ToString())
                .Replace("{JustificativaIa}", vaga.JustificativaIa)
                .Replace("{GapsTecnologicos}", !string.IsNullOrEmpty(vaga.GapsTecnologicos) ? vaga.GapsTecnologicos : "Nenhum gap crítico identificado.")
                .Replace("{LinkVaga}", vaga.LinkVaga);
            
            var bodyBuilder = new BodyBuilder { HtmlBody = htmlFinal };

            mensagem.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(mensagem);
            await client.DisconnectAsync(true);

            logger.LogInformation("E-mail de alerta enviado com sucesso para a vaga ID {Id}.", vaga.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao enviar e-mail de alerta para a vaga ID {Id}.", vaga.Id);
        }
    }
}