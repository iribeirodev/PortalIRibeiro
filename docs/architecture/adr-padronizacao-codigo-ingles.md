Por favor crie o arquivo 'docs/arquitetura/padronizacao-codigo-ingles.md' com o conteúdo abaixo. 

NÃO altere nenhum arquivo de código (.cs, .ts, .tsx) do projeto ainda. Apenas leia o contexto, salve o documento e apresente no terminal um plano detalhado de como você fará a execução passo a passo (dry-run).

---

# Padronização do Código e Contratos em Inglês

## Status
Proposto / Em Análise

## Contexto
O projeto continha um padrão misto de nomeação (português e inglês) entre entidades de domínio, DTOs de API, manipuladores de rotas e interfaces no front-end. Para alinhar o código às melhores práticas de mercado e garantir uniformidade com bibliotecas do ecossistema .NET e Next.js, foi definida a migração completa do código-fonte para o inglês.

## Plano de Execução (Dry Run / OpenCode)

### 1. Back-end (.NET API)
* Namespaces & Pastas:
  - Features/Contato -> Features/Contact
  - Features/Projeto -> Features/Projects
  - Features/Telemetria -> Features/Telemetry
* Entidades e DTOs:
  - HistoricoConversa -> ChatHistory
  - Projeto -> Project
  - RegistrarVisitaRequest -> RegisterVisitRequest
  - MensagemContato -> ContactMessage
* Repositórios e Interfaces:
  - IHistoricoConversaRepository / HistoricoConversaRepository -> IChatHistoryRepository / ChatHistoryRepository
  - IProjetoRepository / ProjetoRepository -> IProjectRepository / ProjectRepository
* Infraestrutura:
  - Atualização dos mapeamentos AOT em AppJsonContext.cs.
  - Atualização dos registros de DI em Program.cs.

### 2. Front-end (Next.js)
* Tipos (lib/types.ts): Atualização de interfaces TypeScript.
* Cliente de API (lib/api.ts): Atualização de endpoints e payload de requisições.
* Componentes (components/): Atualização das propriedades consumidas nos componentes de UI.

### 3. Validação
* Execução de 'dotnet build' na pasta PortalIRibeiro.API.
* Execução de 'npm run build' na pasta frontend.
