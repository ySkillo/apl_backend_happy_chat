# Happy Chat - Backend

API do Happy Chat, sistema de mensagens em tempo real, construída com ASP.NET Core.

> **Status:** em desenvolvimento.

## Tecnologias

- C# / .NET 8
- ASP.NET Core Web API
- PostgreSQL com Entity Framework Core (provider Npgsql) e migrations
- Autenticação JWT (Bearer)
- Swagger / OpenAPI
- Docker (para subir o banco localmente)
- Integração com o [email-service](https://github.com/ySkillo/email-service) (API externa) para envio de e-mails

## Requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/) (ou um PostgreSQL já instalado na máquina)
- [dotnet-ef](https://learn.microsoft.com/ef/core/cli/dotnet) (ferramenta de migrations):
  ```bash
  dotnet tool install --global dotnet-ef --version 8.*
  ```
- [email-service](https://github.com/ySkillo/email-service) em execução (API externa responsável pelo envio de e-mails)

## Executando localmente

### 1. Subir o banco de dados (PostgreSQL)

```bash
docker run -d --name happychat-db \
  -e POSTGRES_USER=happy \
  -e POSTGRES_PASSWORD=happy123 \
  -e POSTGRES_DB=happychat \
  -p 5433:5432 \
  -v happychat-data:/var/lib/postgresql/data \
  postgres:16
```

> O banco do container fica na porta **5433** do host para não conflitar com um PostgreSQL local, que normalmente usa a 5432. As credenciais acima são apenas para desenvolvimento local; troque-as se o banco for exposto de alguma forma.

Comandos úteis:

```bash
docker ps                     # ver se o container está rodando
docker stop happychat-db      # parar
docker start happychat-db     # iniciar de novo (os dados são mantidos no volume)
```

### 2. Configurar os segredos

A chave do JWT e a connection string **não são versionadas**. Configure-as com `user-secrets`:

```bash
dotnet user-secrets set "Jwt:Key" "<chave-com-no-minimo-32-caracteres>" --project src/HappyChat.API
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5433;Database=happychat;Username=happy;Password=happy123" --project src/HappyChat.API
```

Se for usar um PostgreSQL local em vez do Docker, ajuste `Port`, `Username` e `Password` na connection string.

### 3. Criar as tabelas (migrations)

```bash
dotnet ef database update -p src/HappyChat.Infrastructure -s src/HappyChat.API
```

Isso aplica as migrations da pasta `src/HappyChat.Infrastructure/Migrations` e cria as tabelas `users` e `pending_registrations`.

### 4. Subir o email-service

O envio de e-mails (código de verificação e boas-vindas) é feito pelo [email-service](https://github.com/ySkillo/email-service), uma API externa. Suba-o antes, seguindo o README dele (por padrão em `http://localhost:5290`).

As credenciais SMTP ficam no **email-service**, não no HappyChat. No repositório do email-service, configure-as com `user-secrets`:

```bash
cd src/EmailService.API
dotnet user-secrets init
dotnet user-secrets set "Smtp:User" "seuemail@gmail.com"
dotnet user-secrets set "Smtp:Password" "senha-de-app-do-gmail"
dotnet user-secrets set "Smtp:FromAddress" "seuemail@gmail.com"
```

> No Gmail, use uma [senha de app](https://support.google.com/accounts/answer/185833), não a senha normal da conta.

> Se o email-service estiver fora do ar, o cadastro **não é iniciado**, pois o código de verificação não pode ser enviado (a API responde 503). Já a falha no e-mail de boas-vindas, enviado depois da confirmação, apenas é registrada em log e não impede a criação da conta.

### 5. Rodar a API

```bash
dotnet restore
dotnet build
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

| Chave                       | Descrição                                                 | Obrigatória |
| --------------------------- | --------------------------------------------------------- | ----------- |
| `ConnectionStrings:Default` | Connection string do PostgreSQL                           | Sim         |
| `Jwt:Key`                   | Chave de assinatura (mín. 32 bytes)                       | Sim         |
| `Jwt:Issuer`                | Emissor do token                                          | Sim         |
| `Jwt:Audience`              | Audiência do token                                        | Sim         |
| `Jwt:ExpireMinutes`         | Validade do token, em minutos                             | Não (60)    |
| `EmailService:BaseUrl`      | URL base do email-service (ex.: `http://localhost:5290/`) | Sim         |

`Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpireMinutes` e `EmailService:BaseUrl` já têm valores padrão no `appsettings.json`. `Jwt:Key` e `ConnectionStrings:Default` precisam ser informados por você.

Em desenvolvimento, use `dotnet user-secrets`. Em produção, use variáveis de ambiente, com `__` no lugar de `:`:

```
Jwt__Key=...
ConnectionStrings__Default=...
EmailService__BaseUrl=...
```

## Banco de dados e migrations

O projeto usa PostgreSQL com Entity Framework Core. O `AppDbContext` fica em `HappyChat.Infrastructure/Persistence` e o histórico do schema fica em `HappyChat.Infrastructure/Migrations`, versionado junto com o código.

| Tabela                  | Descrição                                                                 |
| ----------------------- | ------------------------------------------------------------------------- |
| `users`                 | Usuários cadastrados. `Id` é auto incremento; `Email` e `Perfil` são únicos |
| `pending_registrations` | Cadastros aguardando a confirmação do código enviado por e-mail           |

Comandos (execute na raiz do repositório):

```bash
# Criar uma nova migration depois de alterar entidades ou o mapeamento
dotnet ef migrations add NomeDaMigration -p src/HappyChat.Infrastructure -s src/HappyChat.API

# Aplicar as migrations pendentes no banco
dotnet ef database update -p src/HappyChat.Infrastructure -s src/HappyChat.API

# Desfazer a última migration (ainda não aplicada)
dotnet ef migrations remove -p src/HappyChat.Infrastructure -s src/HappyChat.API

# Apagar o banco (apenas em desenvolvimento)
dotnet ef database drop -p src/HappyChat.Infrastructure -s src/HappyChat.API
```

Para consultar os dados direto no banco do container:

```bash
docker exec -it happychat-db psql -U happy -d happychat -c 'SELECT "Id","Perfil","Email","IsActive" FROM users;'
```

## Endpoints

O cadastro acontece em duas etapas: primeiro a API envia um código de verificação para o e-mail informado; a conta só é criada depois que o código é confirmado.

| Método | Rota                | Acesso      | Descrição                                                  |
| ------ | ------------------- | ----------- | ---------------------------------------------------------- |
| POST   | `/api/users`        | Público     | Inicia o cadastro e envia o código de verificação por e-mail |
| POST   | `/api/users/verify` | Público     | Confirma o código, cria a conta e envia o e-mail de boas-vindas |
| GET    | `/api/users`        | Autenticado | Lista os usuários                                          |
| GET    | `/api/users/{id}`   | Autenticado | Busca um usuário pelo id (numérico)                        |
| POST   | `/api/auth/login`   | Público     | Autentica e retorna o JWT                                  |
| GET    | `/api/auth/me`      | Autenticado | Retorna o usuário do token                                 |

Para chamar as rotas autenticadas no Swagger, faça login em `/api/auth/login`, copie o token retornado, clique em **Authorize** e cole apenas o JWT (sem o prefixo `Bearer`).

## Arquitetura

O projeto segue Clean Architecture, com o código agrupado por funcionalidade dentro de cada camada.

```
src/
├── HappyChat.API             # Controllers, middlewares e configuração da aplicação
├── HappyChat.Application     # Casos de uso, DTOs e contratos
├── HappyChat.Domain          # Entidades, regras e exceções de domínio
└── HappyChat.Infrastructure  # Persistência (EF Core, repositórios, migrations), JWT, hash de senha e cliente HTTP do email-service
```

> O envio de e-mails não é feito pelo HappyChat: ele chama o [email-service](https://github.com/ySkillo/email-service) via HTTP (`POST /api/email/send`) através do `EmailServiceClient`.

## Licença

Distribuído sob a licença descrita em [LICENSE](LICENSE).