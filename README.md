# 🛡️ GestorPro - Sistema de Controlo de Acessos

Um sistema de gestão de utilizadores e controlo de acessos desenvolvido em **C# (WPF)** com base de dados **PostgreSQL**. Este projeto foi concebido com foco em segurança, integridade de dados e rastreabilidade total (auditoria), aplicando boas práticas de engenharia de software.

## ✨ Funcionalidades Principais

* **Autenticação Segura:** Proteção de senhas com Hash criptográfico utilizando a biblioteca `BCrypt`.
* **Controlo de Perfis (RBAC):** Diferenciação de permissões entre `Administrador` (acesso total a gestão) e `Comum` (acesso restrito).
* **Bloqueio de Segurança:** Sistema de proteção contra força bruta que bloqueia temporariamente a conta após 3 tentativas de login falhadas, com opção de desbloqueio manual pelo Administrador.
* **Auditoria Completa (Logs):** Rastreabilidade rigorosa de ações críticas (criação, edição e exclusão de utilizadores). Nenhuma exclusão pode violar o histórico de auditoria.
* **Eventos de Autenticação:** Registo detalhado de todas as tentativas de login (sucesso, falha, utilizador inexistente).
* **Gestão de Fusos Horários (Timezones):** Base de dados configurada estritamente em `UTC (00:00)` para garantir integridade global, com conversão dinâmica para a hora local do utilizador na interface gráfica (WPF).
* **Proteção de Regras de Negócio:** Transações seguras (BeginTransaction/Commit) e bloqueio de exclusão do último Administrador ativo para evitar perda de acesso ao sistema.

## 🛠️ Tecnologias Utilizadas

* **Linguagem:** C# (.NET)
* **Interface Gráfica:** WPF (Windows Presentation Foundation) / XAML
* **Base de Dados:** PostgreSQL
* **Driver de Conexão:** Npgsql (ADO.NET)
* **Segurança:** BCrypt.Net-Next

## 🗄️ Estrutura da Base de Dados

O sistema opera com um banco de dados dedicado (`gestorpro`) e possui três tabelas principais fortemente relacionadas:

1. `usuarios`: Armazena os dados, credenciais encriptadas e estado de bloqueio.
2. `auditoria`: Regista todas as alterações de estado feitas por administradores.
3. `eventos_autenticacao`: Monitoriza o tráfego e tentativas de acesso.

## 🚀 Como Executar o Projeto (Setup)

### Pré-requisitos
* Visual Studio (com a carga de trabalho de desenvolvimento Desktop .NET instalada)
* PostgreSQL instalado localmente (ou hospedado na nuvem)
* PgAdmin ou DBeaver para gestão do banco de dados

