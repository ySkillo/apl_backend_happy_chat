# Happy Chat - Backend

API do Happy Chat, sistema de mensagens em tempo real, construída com ASP.NET Core.

> **Status:** em desenvolvimento.

## Tecnologias

- C# / .NET 8
- ASP.NET Core Web API
- Autenticação JWT (Bearer)
- Swagger / OpenAPI

## Requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download)

## Executando localmente

```bash
dotnet restore
dotnet build
dotnet user-secrets set "Jwt:Key" "<chave-com-no-minimo-32-caracteres>" --project src/HappyChat.API
dotnet run --project src/HappyChat.API
```

A API sobe em `https://localhost:8081` (ou `http://localhost:8080`).
Em ambiente de desenvolvimento:

| Recurso      | Caminho    |
| ------------ | ---------- |
| Swagger UI   | `/swagger` |
| Health check | `/health`  |

## Configuração

Segredos e configurações locais não são versionados.

| Chave               | Descrição                           | Obrigatória |
| ------------------- | ----------------------------------- | ----------- |
| `Jwt:Key`           | Chave de assinatura (mín. 32 bytes) | Sim         |
| `Jwt:Issuer`        | Emissor do token                    | Sim         |
| `Jwt:Audience`      | Audiência do token                  | Sim         |
| `Jwt:ExpireMinutes` | Validade do token, em minutos       | Não (60)    |

Em desenvolvimento, use `dotnet user-secrets`. Em produção, use variáveis de ambiente (ex.: `Jwt__Key`).

## Endpoints

| Método | Rota              | Acesso      | Descrição                  |
| ------ | ----------------- | ----------- | -------------------------- |
| POST   | `/api/users`      | Público     | Cadastra um usuário        |
| GET    | `/api/users`      | Autenticado | Lista os usuários          |
| GET    | `/api/users/{id}` | Autenticado | Busca um usuário pelo id   |
| POST   | `/api/auth/login` | Público     | Autentica e retorna o JWT  |
| GET    | `/api/auth/me`    | Autenticado | Retorna o usuário do token |

## Arquitetura

O projeto segue Clean Architecture, com o código agrupado por funcionalidade dentro de cada camada.

```
src/
├── HappyChat.API             # Controllers, middlewares e configuração da aplicação
├── HappyChat.Application     # Casos de uso, DTOs e contratos
├── HappyChat.Domain          # Entidades, regras e exceções de domínio
└── HappyChat.Infrastructure  # Implementações (repositórios, JWT, hash de senha)
```

> A persistência atual é em memória: os dados são perdidos ao reiniciar a API.

## Licença

Distribuído sob a licença descrita em [LICENSE](LICENSE).