# 💇 Salão de Beleza — Sistema Web

> Sistema web para gerenciamento de salão de beleza com cadastro de clientes, profissionais, serviços, produtos e agendamentos, com login e controle de acesso.

---

## 📋 Sobre o Projeto

Projeto escolar desenvolvido em **ASP.NET Core MVC (C#)** com banco de dados **MySQL**, seguindo o padrão **MVC (Model-View-Controller)**. O sistema permite o gerenciamento completo de um salão de beleza, incluindo autenticação de usuários com diferentes níveis de permissão, cadastro de todas as entidades do negócio, upload de fotos e exclusão lógica de registros.

---

## 🚀 Tecnologias Utilizadas

| Tecnologia | Versão |
|---|---|
| ASP.NET Core MVC | .NET 8.0 |
| C# | 12 |
| MySQL | 8.x |
| Bootstrap | 5.x |
| jQuery | 3.x |
| BCrypt.Net-Next | 4.1.0 |
| MySql.Data | 9.6.0 |

---

## 🗄️ Banco de Dados

O banco de dados `bdsalaodebeleza` é composto pelas seguintes tabelas:

| Tabela | Descrição |
|---|---|
| `profissional` | Profissionais do salão (cabeleireiros, manicures, etc.) |
| `servico` | Serviços oferecidos (corte, coloração, escova, etc.) |
| `cliente` | Clientes cadastrados |
| `produto` | Produtos em estoque (shampoo, tinta, etc.) |
| `agendamento` | Agendamentos vinculando cliente, profissional e serviço |
| `usuarios` | Usuários do sistema com hash de senha e perfil de acesso |

> Todas as tabelas possuem o campo `ativo` para exclusão lógica — os dados **nunca são apagados fisicamente** do banco.

O arquivo `bdsalaodebelezaFInalizado.sql` na raiz do projeto contém toda a estrutura e dados iniciais para configurar o banco.

---

## 🔐 Sistema de Login e Perfis de Acesso

O sistema possui autenticação via sessão com **3 perfis de usuário**:

| Perfil | Permissões |
|---|---|
| **Admin** | Acesso total: cadastrar, editar, excluir e gerenciar usuários |
| **Gerente** | Pode cadastrar e editar, mas não exclui nem gerencia usuários |
| **Recepcionista** | Acesso somente para visualização e cadastro de clientes/agendamentos |

As senhas são armazenadas com **hash BCrypt** — nunca em texto puro.  
A sessão expira automaticamente após **8 horas** de inatividade.

### Credenciais padrão

| Usuário | Senha | Perfil |
|---|---|---|
| `admin` | `admin123` | Admin |
| `gerente` | `admin123` | Gerente |

> ⚠️ Altere as senhas após o primeiro acesso.

---

## 📂 Estrutura do Projeto

```
SalaoDeBeleza/
├── Controllers/
│   ├── AdminController.cs          # Login, logout e gerenciamento de usuários
│   ├── HomeController.cs           # Painel inicial com totais
│   ├── ProfissionalController.cs   # CRUD de profissionais + upload de foto
│   ├── ServicoController.cs        # CRUD de serviços
│   ├── ClienteController.cs        # CRUD de clientes
│   ├── ProdutoController.cs        # CRUD de produtos
│   └── AgendamentoController.cs    # CRUD de agendamentos
├── Models/
│   ├── Profissional.cs
│   ├── Servico.cs
│   ├── Cliente.cs
│   ├── Produto.cs
│   ├── Agendamento.cs
│   └── Usuario.cs
├── Views/
│   ├── Admin/         # Login, novo usuário, listagem de usuários, acesso negado
│   ├── Profissional/  # Index, Criar, Editar
│   ├── Servico/       # Index, Criar, Editar
│   ├── Cliente/       # Index, Criar, Editar
│   ├── Produto/       # Index, Criar, Editar
│   ├── Agendamento/   # Index, Criar, Editar
│   ├── Home/          # Painel principal
│   └── Shared/        # Layout, Error
├── SessionAuthorize/
│   └── SessionAuthorizeAttribute.cs  # Filtro de autenticação e autorização
├── Data/
│   └── Database.cs                   # Conexão com o MySQL
├── wwwroot/
│   └── img/fotos/                    # Fotos dos profissionais (upload)
├── bdsalaodebelezaFInalizado.sql
└── ProjetoOlimpicos.csproj
```

---

## ⚙️ Como Executar o Projeto

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [MySQL 8.x](https://dev.mysql.com/downloads/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou VS Code

### Passo a passo

**1. Clone o repositório**
```bash
git clone https://github.com/seu-usuario/salao-de-beleza.git
cd salao-de-beleza
```

**2. Configure o banco de dados**

Abra o MySQL e execute o script:
```sql
source bdsalaodebelezaFInalizado.sql
```

**3. Verifique a string de conexão**

Em `Data/Database.cs`, confirme as credenciais do MySQL:
```csharp
private readonly string connectionString =
    "server=localhost;port=3306;database=bdsalaodebeleza;user=root;password=12345678;";
```

**4. Execute o projeto**
```bash
dotnet run
```
Ou abra `ProjetoOlimpicos.sln` no Visual Studio e pressione **F5**.

**5. Acesse no navegador**
```
https://localhost:7000
```

---

## ✅ Funcionalidades

- [x] Sistema de login com sessão e BCrypt
- [x] Controle de acesso por perfil (Admin / Gerente / Recepcionista)
- [x] Cadastro, listagem e edição de Profissionais
- [x] Upload de foto dos profissionais
- [x] Cadastro, listagem e edição de Serviços
- [x] Cadastro, listagem e edição de Clientes
- [x] Cadastro, listagem e edição de Produtos
- [x] Cadastro, listagem e edição de Agendamentos (com FK para cliente, profissional e serviço)
- [x] Exclusão lógica em todas as tabelas (campo `ativo`, sem apagar do banco)
- [x] Painel inicial com totais de registros ativos

---

## 👨‍💻 Autor

Desenvolvido como projeto escolar — Categoria: Serviços (#13 Salão de Beleza).
