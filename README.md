# Happy Chat - Backend

API do Happy Chat, sistema de mensagens em tempo real, construída com ASP.NET Core.

> **Status:** em desenvolvimento.

## Tecnologias

- C# / .NET 8
- ASP.NET Core Web API
- Autenticação JWT (Bearer)
- Swagger / OpenAPI
- Integração com o [email-service](https://github.com/ySkillo/email-service) (API externa) para envio de e-mails

## Requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download)
- [email-service](https://github.com/ySkillo/email-service) em execução (API externa responsável pelo envio de e-mails)

## Executando localmente

```bash
dotnet restore
dotnet build
dotnet user-secrets set "Jwt:Key" "<chave-com-no-minimo-32-caracteres>" --project src/HappyChat.API
dotnet run --project src/HappyChat.API
```

O HappyChat depende do [email-service](https://github.com/ySkillo/email-service), uma API externa, para enviar dois e-mails:

- **Código de verificação** (cadastro): obrigatório. Se o email-service estiver fora do ar, o cadastro não é iniciado e a API responde `503`.
- **Boas-vindas** (após confirmar o código): se falhar, a conta já foi criada e a falha é apenas registrada em log.

Suba o email-service antes de usar o cadastro, seguindo o README dele (por padrão em `http://localhost:5290`).

### Configurando o email-service

As credenciais SMTP ficam no **email-service**, não no HappyChat. No repositório do email-service, configure-as com `user-secrets`:

```bash
cd src/EmailService.API
dotnet user-secrets init
dotnet user-secrets set "Smtp:User" "seuemail@gmail.com"
dotnet user-secrets set "Smtp:Password" "senha-de-app-do-gmail"
dotnet user-secrets set "Smtp:FromAddress" "seuemail@gmail.com"
```

> No Gmail, use uma [senha de app](https://support.google.com/accounts/answer/185833), não a senha normal da conta.

A API sobe em `https://localhost:8081` (ou `http://localhost:8080`).
Em ambiente de desenvolvimento:

| Recurso      | Caminho    |
| ------------ | ---------- |
| Swagger UI   | `/swagger` |
| Health check | `/health`  |

## Configuração

Segredos e configurações locais não são versionados.

| Chave                  | Descrição                                                  | Obrigatória |
| ---------------------- | ---------------------------------------------------------- | ----------- |
| `Jwt:Key`              | Chave de assinatura (mín. 32 bytes)                        | Sim         |
| `Jwt:Issuer`           | Emissor do token                                           | Sim         |
| `Jwt:Audience`         | Audiência do token                                         | Sim         |
| `Jwt:ExpireMinutes`    | Validade do token, em minutos                              | Não (60)    |
| `EmailService:BaseUrl` | URL base do email-service (ex.: `http://localhost:5290/`)  | Sim         |

Em desenvolvimento, use `dotnet user-secrets`. Em produção, use variáveis de ambiente (ex.: `Jwt__Key`, `EmailService__BaseUrl`).

## Endpoints

| Método | Rota                 | Acesso      | Descrição                                          |
| ------ | -------------------- | ----------- | -------------------------------------------------- |
| POST   | `/api/users`         | Público     | Inicia o cadastro e envia um código por e-mail     |
| POST   | `/api/users/verify`  | Público     | Confirma o código e cria a conta                   |
| GET    | `/api/users`         | Autenticado | Lista os usuários                                  |
| GET    | `/api/users/{id}`    | Autenticado | Busca um usuário pelo id                           |
| POST   | `/api/auth/login`    | Público     | Autentica e retorna o JWT                          |
| GET    | `/api/auth/me`       | Autenticado | Retorna o usuário do token                         |

### Fluxo de cadastro

1. `POST /api/users` com nome, perfil, e-mail e senha: os dados ficam pendentes e um código de 6 dígitos é enviado por e-mail (vale 10 minutos).
2. `POST /api/users/verify` com e-mail e código: a conta é criada. São permitidas até 5 tentativas incorretas; depois disso é preciso refazer o passo 1.
3. `POST /api/auth/login` para obter o token JWT.

Chamar o passo 1 novamente para o mesmo e-mail substitui o cadastro pendente e gera um novo código.

## Arquitetura

O projeto segue Clean Architecture, com o código agrupado por funcionalidade dentro de cada camada.

```
src/
├── HappyChat.API             # Controllers, middlewares e configuração da aplicação
├── HappyChat.Application     # Casos de uso, DTOs e contratos
├── HappyChat.Domain          # Entidades, regras e exceções de domínio
└── HappyChat.Infrastructure  # Implementações (repositórios, JWT, hash de senha, cliente HTTP do email-service)
```

> A persistência atual é em memória: usuários e cadastros pendentes (códigos de verificação) são perdidos ao reiniciar a API.

> O envio de e-mails não é feito pelo HappyChat: ele chama o [email-service](https://github.com/ySkillo/email-service) via HTTP (`POST /api/email/send`) através do `EmailServiceClient`.

## Licença

Distribuído sob a licença descrita em [LICENSE](LICENSE).
