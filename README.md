# Portal IRibeiro

Este é meu portfólio profissional e laboratório tecnológico. A aplicação demonstra minha experiência em desenvolvimento **Full Stack**, combinando **.NET/C#**, **Angular**, **PostgreSQL**, **Redis**, **Docker** e integração com **IA/RAG**.

> Resumo: o repositório centraliza a arquitetura do portal, projetado sob os princípios de **Clean Architecture** e focado no ecossistema moderno do .NET, engenharia de dados e inteligência artificial (RAG). A solução é totalmente desacoplada, separando o ecossistema de APIs do front-end para garantir escalabilidade, segurança e custo zero de distribuição para a interface.

---

## 🛠️ Arquitetura Geral da Solução

```text
PortalIRibeiro/
├── docs/                     # Scripts de banco (DDL/Schema), templates de ambiente (.env.sample)
├── PortalIRibeiro.API/       # Lógica de negócio, Web API Core (Minimal APIs, .NET 10 / Native AOT)
├── frontend/                 # Interface SPA em Angular 21 (TypeScript)
├── PortalIRibeiro.slnx       # Arquivo de solução unificado do .NET
└── README.md                 # Documentação principal
```

**Fluxo:** o usuário acessa o HTML estático servido pela Vercel. O Angular chama a API da Koyeb via proxy/rewrite em `/api/*` (mesma origem no browser, a Vercel reencaminha para a Koyeb); chat e telemetria são chamadas **client-side** (CORS liberado), sem intermediário. A API orquestra o RAG no Gemini, persiste histórico/visitas na Neon e usa Redis para o **rate limit diário do chat da Íris**.

```mermaid
flowchart LR
    U[Visitante / Recrutador] -->|HTTPS| V[Vercel<br/>SPA Angular 21]

    V -->|"/api/* (rewrite) – projetos<br/>(mesma origem)"| K[API .NET 10 / Native AOT<br/>Koyeb]
    V -->|"chat Íris + telemetria<br/>(client-side, CORS)"| K

    K -->|"RAG / prompts"| G[Google Gemini]
    K -->|"currículo · histórico · visitas"| N[(PostgreSQL Neon)]
    K -->|"rate limit diário<br/>chat Íris (fail-closed)"| R[(Redis Upstash)]
```

## Módulos em Destaque

### Assistente Inteligente Íris

<p align="center">
<img width="400" height="400" alt="resume-assist" src="./frontend/public/images/laboratorio/resume-assist.jpeg" />
</p>

A Íris é um agente de inteligência artificial integrado nativamente ao portal, projetado para interagir com visitantes e recrutadores através de um chat dinâmico, respondendo perguntas estritamente baseadas no meu histórico e trajetória profissional.

Onde está o código? Toda a lógica conceitual está isolada na Feature em [PortalIRibeiro.API/Features/Iris](./PortalIRibeiro.API/Features/Iris).

Como funciona?
* Implementação de RAG (Retrieval-Augmented Generation) consumindo a API oficial do Google Gemini, com **fallback automático** entre modelos (`gemini-3.5-flash-lite` primário → `gemini-3.6-flash` fallback).
* Camada de infraestrutura desacoplada contendo o `GeminiService.cs` para o gerenciamento de prompts e contexto refinado do currículo.
* Armazenamento e persistência do histórico completo de conversas em banco para auditoria e controle de sessões via UUID através do Postgres.

### Fluxo de processamento de perguntas e respostas

```mermaid
flowchart TD
    U[Visitante] -->|faz uma pergunta no chat| F[Chat do portal]
    F -->|"API envia pergunta + contexto do currículo"| G[Google Gemini]
    G -->|"retorna a resposta"| F
    F -->|"Íris responde ao visitante"| U
    F -->|"conversa é salva"| B[(PostgreSQL)]
```

O `GeminiService` monta o payload com o contexto do currículo e a pergunta do usuário, tentando primeiro o modelo `gemini-3.5-flash-lite` e, em falha, o fallback `gemini-3.6-flash`. A conversa é persistida no PostgreSQL via `IrisChatHandler`.

---

## Tecnologias Utilizadas
* Back-End: .NET 10 & C# 14 (Minimal APIs, Native AOT, Inversão de Dependência)
* Front-End: Angular 21 (TypeScript), SPA de página única com componentes standalone
* Banco de Dados Cloud: PostgreSQL Serverless hospedado na Neon.
* Cache & Mensageria: Redis gerenciado em nuvem via Upstash (rate limit do chat da Íris).
* Hospedagem API: Aplicação containerizada com Docker (Native AOT) e implantada na Koyeb (plano gratuito).
* Hospedagem Front: Vercel (plano Hobby).

## Frontend (Angular) — Configuração e Renderização

O frontend é uma **SPA Angular 21** renderizada totalmente no cliente (CSR), servida como conteúdo estático pela Vercel. Como não há SSR/SSG, todo o consumo de dados acontece no navegador.

### Como as partes se conectam

- **Componentes standalone:** `Navbar`, `Hero`, `About`, `Laboratory` (projetos), `Services`, `Contact`, `ResumeAssistant` (chat Íris) e `Telemetry`.
- **API via `/api` relativo:** o `environment.prod.ts` usa `apiUrl: '/api'`. Na Vercel, o `frontend/vercel.json` reescreve (`rewrites`) `/api/:path*` para `https://portaliribeiro-api.koyeb.app/api/:path*` — o navegador vê mesma-origem e elimina CORS em produção.
- **Token:** um `HttpInterceptor` (`app-token.interceptor.ts`) injeta o header `X-App-Token` nas chamadas `* /api*`.
- **Proxy local:** em dev (`ng serve`), o `proxy.conf.mjs` encaminha `/api` para `http://localhost:5125` (API local).
- **Markdown:** as respostas do chat são renderizadas com `ngx-markdown`, reproduzindo a saída que o Blazor produzia com Markdig.
- **Estilo:** Bootstrap 5 + Tailwind CSS 4 (PostCSS).
- **Telemetria:** o registro de visita é deduplicado pela API por IP + página na tabela-cache `portal.visit_cache` do Postgres, com janela de 15 minutos.

### Stack do frontend

| Camada | Tecnologia |
|---|---|
| Framework | Angular 21 + TypeScript 5.9 |
| UI | Bootstrap 5 + Tailwind CSS 4 (via PostCSS) + `styles.css` |
| Dados | `portal-api.service.ts` (fetch via `HttpClient`), `portal.models.ts` |
| Markdown | `ngx-markdown` |
| Testes | Vitest (`ng test`, via `@angular/build:unit-test`) |