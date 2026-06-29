# Portal IRibeiro

Este repositório centraliza a arquitetura do meu portal profissional e laboratório tecnológico, projetado sob os princípios de **Clean Architecture** e focado no ecossistema moderno do .NET, engenharia de dados e inteligência artificial (RAG).

A solução é totalmente desacoplada, separando o ecossistema de APIs do front-end para garantir escalabilidade, segurança e custo zero de distribuição para a interface.

---

## 🛠️ Arquitetura Geral da Solução

```text
PortalIRibeiro/
├── docs/                     # Scripts de banco (DDL/Schema) e templates de ambiente (.env.sample)
├── src/
│   ├── PortalIRibeiro.API/       # Lógica de negócio, Web API Core, Workers e integrações
│   └── PortalIRibeiro.FrontEnd/  # Interface SPA interativa em Blazor WebAssembly (WASM)
├── PortalIRibeiro.slnx       # Arquivo de solução unificado do .NET
└── README.md                 # Documentação principal
```

## Módulos em Destaque

### Assistente Inteligente Íris

<p align="center">
<img width="400" height="400" alt="resume-assist" src="https://github.com/user-attachments/assets/0aba888c-3fb1-4278-baf9-93b8c029f916" />
</p>

A Íris é um agente de inteligência artificial integrado nativamente ao portal, projetado para interagir com visitantes e recrutadores através de um chat dinâmico, respondendo perguntas estritamente baseadas no meu histórico e trajetória profissional.

Onde está o código? Toda a lógica conceitual está isolada na Feature em PortalIRibeiro.API/Features/IrisChat.

Como funciona? 
* Implementação de RAG (Retrieval-Augmented Generation) consumindo a API oficial do Google Gemini.
* Camada de infraestrutura desacoplada contendo o GeminiService.cs para o gerenciamento de prompts e contexto refinado do currículo.
* Armazenamento e persistência do histórico completo de conversas em banco para auditoria e controle de sessões via UUID através do Postgres.
---
###  JobScraper & Triagem Automática
Um ecossistema focado na automação de rotinas para coleta e ingestão de dados externos de mercado.

Onde está o código? A esteira de processamento está localizada em PortalIRibeiro.API/Features/JobScraper.

Como funciona?

Execução contínua em segundo plano através de um .NET BackgroundService (RssBackgroundWorker.cs).

Ingestão contínua de feeds RSS (ex: Inoreader, LinkedIn) configurados dinamicamente na tabela rss_feeds.

Camada de cache e triagem para evitar duplicidade de registros através de identificadores únicos globais (GUID), persistindo os dados limpos no Postgres.

## Tecnologias Utilizadas
* Back-End: .NET 10 & C# 14 (Web API Core, Background Services, Inversão de Dependência)
* Front-End: Blazor WebAssembly (WASM) com componentes interativos e Pixel Art customizada.
* Banco de Dados Cloud: PostgreSQL Serverless hospedado na Neon.
* Cache & Mensageria: Redis gerenciado em nuvem via Upstash.
* Hospedagem API: Aplicação containerizada com Docker e implantada na Koyeb.
* Hospedagem Front: Distribuição estática global via Netlify.

## Notas sobre a execução local do projeto

Pré-requisitos
* SDK do .NET 10 instalado.
* Docker e Docker Compose ativos na máquina (ambiente Linux testado em base Ubuntu).
