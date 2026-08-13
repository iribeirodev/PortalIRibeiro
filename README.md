# Portal IRibeiro

Este repositório centraliza a arquitetura do meu portal profissional e laboratório tecnológico, projetado sob os princípios de **Clean Architecture** e focado no ecossistema moderno do .NET, engenharia de dados e inteligência artificial (RAG).

A solução é totalmente desacoplada, separando o ecossistema de APIs do front-end para garantir escalabilidade, segurança e custo zero de distribuição para a interface.

---

## 🛠️ Arquitetura Geral da Solução

```text
PortalIRibeiro/
├── docs/                     # Scripts de banco (DDL/Schema) e templates de ambiente (.env.sample)
├── PortalIRibeiro.API/       # Lógica de negócio, Web API Core, Workers e integrações
├── frontend/                 # Interface SPA em Next.js (App Router + TypeScript)
├── PortalIRibeiro.slnx       # Arquivo de solução unificado do .NET
└── README.md                 # Documentação principal
```

## Módulos em Destaque

### Assistente Inteligente Íris

<p align="center">
<img width="400" height="400" alt="resume-assist" src="https://github.com/user-attachments/assets/0aba888c-3fb1-4278-baf9-93b8c029f916" />
</p>

A Íris é um agente de inteligência artificial integrado nativamente ao portal, projetado para interagir com visitantes e recrutadores através de um chat dinâmico, respondendo perguntas estritamente baseadas no meu histórico e trajetória profissional.

Onde está o código? Toda a lógica conceitual está isolada na Feature em [PortalIRibeiro.API/Features/IrisChat](./PortalIRibeiro.API/Features/IrisChat).

Como funciona? 
* Implementação de RAG (Retrieval-Augmented Generation) consumindo a API oficial do Google Gemini.
* Camada de infraestrutura desacoplada contendo o GeminiService.cs para o gerenciamento de prompts e contexto refinado do currículo.
* Armazenamento e persistência do histórico completo de conversas em banco para auditoria e controle de sessões via UUID através do Postgres.
---

## Tecnologias Utilizadas
* Back-End: .NET 10 & C# 14 (Web API Core, Background Services, Inversão de Dependência)
* Front-End: Next.js (App Router + TypeScript) com modelo híbrido SSG/ISR/CSR.
* Banco de Dados Cloud: PostgreSQL Serverless hospedado na Neon.
* Cache & Mensageria: Redis gerenciado em nuvem via Upstash.
* Hospedagem API: Aplicação containerizada com Docker e implantada na Koyeb.
* Hospedagem Front: Vercel (plano Hobby).

## Notas sobre a execução local do projeto

Pré-requisitos
* SDK do .NET 10 instalado (para a API).
* Node.js 20+ (para o frontend).
* Docker e Docker Compose ativos na máquina (ambiente Linux testado em base Ubuntu).
* Rider IDE ou um editor de código de sua preferência (como VS Code) com suporte a C#.

### Frontend (Next.js)

```bash
cd frontend
cp .env.example .env.local   # ajuste a NEXT_PUBLIC_API_BASE_URL se necessário
npm install
npm run dev                  # http://localhost:3000
```

### Deploy na Vercel

O framework é auto-detectado (Next.js). No projeto da Vercel, defina a variável de ambiente de produção:

```
NEXT_PUBLIC_API_BASE_URL=https://portaliribeiro-api.koyeb.app/
```

O modelo de renderização híbrido (SSG + ISR de 1h no laboratório + CSR no chat/telemetria) cabe com folga no plano Hobby: o chat e a telemetria chamam a API da Koyeb direto do navegador (CORS liberado), sem consumir funções da Vercel.
