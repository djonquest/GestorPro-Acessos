using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GestorPro_Acessos
{
    public partial class LoginWindow : Window
    {
        private const int LIMITE_TENTATIVAS = 3;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            txtMensagemErro.Visibility = Visibility.Collapsed;
            string usuarioInput = txtUsuario.Text.Trim();
            string senhaInput = txtSenha.Password;

            if (string.IsNullOrEmpty(usuarioInput) || string.IsNullOrEmpty(senhaInput))
            {
                ExibirErroGenerico();
                return;
            }

            try
            {
                
                int userId = 0;
                string hashGuardado = "";
                bool ativo = false;
                bool bloqueado = false;
                int tentativas = 0;
                string tipoUsuario = "";
                bool usuarioEncontrado = false;

                
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();
                    string query = "SELECT id, senha, status_ativo, bloqueado, tentativas_invalidas, tipo_usuario FROM usuarios WHERE nome_usuario = @usuario";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuarioInput);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userId = reader.GetInt32(0);
                                hashGuardado = reader.GetString(1);
                                ativo = reader.GetBoolean(2);
                                bloqueado = reader.GetBoolean(3);
                                tentativas = reader.GetInt32(4);
                                tipoUsuario = reader.GetString(5);
                                usuarioEncontrado = true;
                            }
                        }
                    }
                } 

                if (!usuarioEncontrado)
                {
                    RegistarEventoIsolado(usuarioInput, null, "Tentativa de login inválida", "Utilizador inexistente");
                    ExibirErroGenerico();
                    return;
                }

                if (bloqueado)
                {
                    RegistarEventoIsolado(usuarioInput, userId, "Tentativa de login inválida", "Conta bloqueada temporariamente");
                    txtMensagemErro.Text = "Conta bloqueada devido a múltiplas tentativas inválidas.";
                    txtMensagemErro.Visibility = Visibility.Visible;
                    return;
                }

                if (!ativo)
                {
                    RegistarEventoIsolado(usuarioInput, userId, "Tentativa de login inválida", "Conta inativa");
                    txtMensagemErro.Text = "A sua conta está inativa. Contacte um administrador.";
                    txtMensagemErro.Visibility = Visibility.Visible;
                    return;
                }

                
                bool senhaCorreta = BCrypt.Net.BCrypt.Verify(senhaInput, hashGuardado);

                
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();

                    if (senhaCorreta)
                    {
                        AtualizarDadosLoginSucesso(conexao, userId);
                        RegistarEvento(conexao, usuarioInput, userId, "Login realizado com sucesso", "Acesso permitido");

                        Sessao.UsuarioId = userId;
                        Sessao.TipoUsuario = tipoUsuario;

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        tentativas++;
                        bool bloquearAgora = tentativas >= LIMITE_TENTATIVAS;

                        AtualizarTentativasFalhadas(conexao, userId, tentativas, bloquearAgora);

                        string tipoEvento = bloquearAgora ? "Bloqueio temporário de conta" : "Tentativa de login inválida";
                        string resultadoEvento = bloquearAgora ? "Múltiplas falhas - Conta bloqueada" : "Palavra-passe incorreta";
                        RegistarEvento(conexao, usuarioInput, userId, tipoEvento, resultadoEvento);

                        if (bloquearAgora)
                        {
                            txtMensagemErro.Text = "Conta bloqueada devido a múltiplas tentativas inválidas.";
                            txtMensagemErro.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            ExibirErroGenerico();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao ligar à base de dados: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExibirErroGenerico()
        {
            txtMensagemErro.Text = "Usuário ou senha inválidos.";
            txtMensagemErro.Visibility = Visibility.Visible;
        }

        private void AtualizarDadosLoginSucesso(NpgsqlConnection conexao, int userId)
        {
            string query = "UPDATE usuarios SET tentativas_invalidas = 0, ultimo_login = CURRENT_TIMESTAMP WHERE id = @id";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        private void AtualizarTentativasFalhadas(NpgsqlConnection conexao, int userId, int tentativas, bool bloquear)
        {
            string query = "UPDATE usuarios SET tentativas_invalidas = @tentativas, bloqueado = @bloquear WHERE id = @id";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
            {
                cmd.Parameters.AddWithValue("@tentativas", tentativas);
                cmd.Parameters.AddWithValue("@bloquear", bloquear);
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        private void RegistarEvento(NpgsqlConnection conexao, string usuarioTentativa, int? userId, string tipoEvento, string resultado)
        {
            string query = @"INSERT INTO eventos_autenticacao (nome_usuario_tentativa, id_usuario_relacionado, tipo_evento, resultado_operacao) 
                             VALUES (@usuario, @idRelacionado, @tipo, @resultado)";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
            {
                cmd.Parameters.AddWithValue("@usuario", usuarioTentativa);
                cmd.Parameters.AddWithValue("@idRelacionado", userId.HasValue ? (object)userId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@tipo", tipoEvento);
                cmd.Parameters.AddWithValue("@resultado", resultado);
                cmd.ExecuteNonQuery();
            }
        }

        
        private void RegistarEventoIsolado(string usuarioTentativa, int? userId, string tipoEvento, string resultado)
        {
            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();
                    RegistarEvento(conexao, usuarioTentativa, userId, tipoEvento, resultado);
                }
            }
            catch { /* Evita que uma falha no log afete o ecrã visual */ }
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }
    }
}
