# Happy Chat - Backend

API do Happy Chat, um sistema de mensagens em tempo real. Construída com ASP.NET Core.

> ⚠️ **Projeto em construção.** Este README é temporário e será atualizado conforme o desenvolvimento avança.

## Stack

- C# / .NET 8
- ASP.NET Core (Web API)
- Entity Framework Core

## Requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download)
- Git

## Como rodar

```bash
# clonar o repositório
git clone <url-do-repositorio>
cd <pasta-do-repositorio>

# restaurar dependências e compilar
dotnet restore
dotnet build

# rodar a API
dotnet run --project src/HappyChat.API
```

A API sobe em `https://localhost:<porta>` (a porta aparece no terminal). Em ambiente Development, a documentação fica em `/swagger` e o health check em `/health`.

## Estrutura

```
HappyChat.sln
└── src/
    └── HappyChat.API/    # projeto ASP.NET Core (ponto de entrada)
```

Novos projetos (domínio, aplicação, infraestrutura, testes) serão adicionados à solution conforme a arquitetura for definida.

## Configuração

Segredos e configurações locais **não** ficam no repositório. Em desenvolvimento, use `dotnet user-secrets` ou variáveis de ambiente.

## Frontend

O cliente desktop fica em outro repositório: `apl_frontend_happy_chat` (Electron + React).
