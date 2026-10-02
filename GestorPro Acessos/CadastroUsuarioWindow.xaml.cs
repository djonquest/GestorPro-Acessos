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
    public partial class CadastroUsuarioWindow : Window
    {
        private int? _idUsuarioEdicao = null;
        private string _nomeUsuarioOriginal = "";
        private string _perfilOriginal = "";
        private bool _statusOriginal = true;
        private bool _bloqueadoOriginal = false;

        
        public CadastroUsuarioWindow()
        {
            InitializeComponent();
            txtTituloJanela.Text = "Novo Utilizador";
        }

        
        public CadastroUsuarioWindow(int idUsuario)
        {
            InitializeComponent();
            _idUsuarioEdicao = idUsuario;
            txtTituloJanela.Text = "Editar Utilizador";

            
            panelSenhas.Visibility = Visibility.Collapsed;

            CarregarDadosUsuario(idUsuario);

            
            if (!Sessao.EhAdministrador)
            {
                cmbPerfil.IsEnabled = false;
                cmbStatus.IsEnabled = false;
                chkBloqueado.IsEnabled = false;
            }
        }

        private void CarregarDadosUsuario(int id)
        {
            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();
                  
                    string query = "SELECT nome_completo, nome_usuario, email, tipo_usuario, status_ativo, imagem_perfil, bloqueado FROM usuarios WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtNomeCompleto.Text = reader.GetString(0);
                                txtUsuario.Text = reader.GetString(1);
                                _nomeUsuarioOriginal = txtUsuario.Text;

                                txtEmail.Text = reader.GetString(2);

                                _perfilOriginal = reader.GetString(3);
                                cmbPerfil.SelectedIndex = _perfilOriginal == "Administrador" ? 1 : 0;

                                _statusOriginal = reader.GetBoolean(4);
                                cmbStatus.SelectedIndex = _statusOriginal ? 0 : 1;

                                string avatarGuardado = reader.GetString(5);
                                SelecionarAvatarComboBox(avatarGuardado);

                                _bloqueadoOriginal = reader.GetBoolean(6);
                                if (_bloqueadoOriginal)
                                {
                                    chkBloqueado.IsChecked = true;
                                    chkBloqueado.Visibility = Visibility.Visible;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelecionarAvatarComboBox(string tagImagem)
        {
            foreach (ComboBoxItem item in cmbAvatar.Items)
            {
                if (item.Tag != null && item.Tag.ToString() == tagImagem)
                {
                    cmbAvatar.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNomeCompleto.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string email = txtEmail.Text.Trim();
            string perfil = (cmbPerfil.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool ativo = cmbStatus.SelectedIndex == 0;

            string avatar = "avatar1.png";
            if (cmbAvatar.SelectedItem is ComboBoxItem itemSelecionado && itemSelecionado.Tag != null)
                avatar = itemSelecionado.Tag.ToString();

            
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Preencha todos os campos obrigatórios.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();
                    using (NpgsqlTransaction transacao = conexao.BeginTransaction())
                    {
                        if (_idUsuarioEdicao == null)
                        {
                            
                            string senha = txtSenha.Password;
                            string confirma = txtConfirmaSenha.Password;

                            
                            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                            {
                                MessageBox.Show("Por favor, introduza um endereço de e-mail válido (exemplo: nome@dominio.com).", "Validação de E-mail", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            if (senha.Length < 8)
                            {
                                MessageBox.Show("A palavra-passe deve possuir no mínimo 8 caracteres.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            if (senha != confirma)
                            {
                                MessageBox.Show("As palavras-passe não coincidem.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            string hash = BCrypt.Net.BCrypt.HashPassword(senha);

                            string queryInsert = @"INSERT INTO usuarios (nome_completo, nome_usuario, email, senha, tipo_usuario, status_ativo, imagem_perfil) 
                                                   VALUES (@nome, @usuario, @email, @senha, @perfil, @ativo, @avatar)";

                            using (NpgsqlCommand cmd = new NpgsqlCommand(queryInsert, conexao, transacao))
                            {
                                cmd.Parameters.AddWithValue("@nome", nome);
                                cmd.Parameters.AddWithValue("@usuario", usuario);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@senha", hash);
                                cmd.Parameters.AddWithValue("@perfil", perfil);
                                cmd.Parameters.AddWithValue("@ativo", ativo);
                                cmd.Parameters.AddWithValue("@avatar", avatar);
                                cmd.ExecuteNonQuery();
                            }

                            RegistarAuditoria(conexao, transacao, "Cadastro de usuário", $"Utilizador: {usuario}", null, "Conta criada");
                        }
                        else
                        {
                            
                            if (_perfilOriginal == "Administrador" && (perfil == "Comum" || !ativo))
                            {
                                if (VerificarSeUltimoAdministradorAtivo(conexao, transacao))
                                {
                                    MessageBox.Show("Não é permitido desativar ou remover a permissão do último administrador ativo do sistema.", "Operação Negada", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }

                            
                            bool bloqueado = chkBloqueado.IsChecked == true;
                            int tentativas = bloqueado ? 3 : 0;

                            
                            string queryUpdate = @"UPDATE usuarios SET nome_completo = @nome, nome_usuario = @usuario, email = @email, 
                                                   tipo_usuario = @perfil, status_ativo = @ativo, imagem_perfil = @avatar, 
                                                   bloqueado = @bloqueado, tentativas_invalidas = @tentativas
                                                   WHERE id = @id";

                            using (NpgsqlCommand cmd = new NpgsqlCommand(queryUpdate, conexao, transacao))
                            {
                                cmd.Parameters.AddWithValue("@nome", nome);
                                cmd.Parameters.AddWithValue("@usuario", usuario);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@perfil", perfil);
                                cmd.Parameters.AddWithValue("@ativo", ativo);
                                cmd.Parameters.AddWithValue("@avatar", avatar);
                                cmd.Parameters.AddWithValue("@bloqueado", bloqueado);
                                cmd.Parameters.AddWithValue("@tentativas", tentativas);
                                cmd.Parameters.AddWithValue("@id", _idUsuarioEdicao.Value);
                                cmd.ExecuteNonQuery();
                            }

                            
                            if (_bloqueadoOriginal && !bloqueado)
                            {
                                RegistarAuditoria(conexao, transacao, "Desbloqueio de usuário", $"Utilizador: {usuario}", "Bloqueado", "Desbloqueado");
                            }
                            else
                            {
                             
                                RegistarAuditoria(conexao, transacao, "Alteração de usuário", $"Utilizador: {usuario}", "Dados antigos", "Dados atualizados");
                            }
                        }

                        transacao.Commit();
                        MessageBox.Show("Operação realizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == "23505")
            {
                MessageBox.Show("O nome de utilizador ou e-mail já estão em uso por outra conta.", "Duplicação de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao guardar os dados: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool VerificarSeUltimoAdministradorAtivo(NpgsqlConnection conexao, NpgsqlTransaction transacao)
        {
            string query = "SELECT COUNT(*) FROM usuarios WHERE tipo_usuario = 'Administrador' AND status_ativo = true";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao, transacao))
            {
                int totalAdmins = Convert.ToInt32(cmd.ExecuteScalar());
                return totalAdmins <= 1; 
            }
        }

        private void RegistarAuditoria(NpgsqlConnection conexao, NpgsqlTransaction transacao, string operacao, string afetado, string valorAnterior, string novoValor)
        {
            string query = @"INSERT INTO auditoria (id_usuario_responsavel, tipo_operacao, registro_afetado, valor_anterior, novo_valor) 
                             VALUES (@responsavel, @operacao, @afetado, @anterior, @novo)";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao, transacao))
            {
                cmd.Parameters.AddWithValue("@responsavel", Sessao.UsuarioId);
                cmd.Parameters.AddWithValue("@operacao", operacao);
                cmd.Parameters.AddWithValue("@afetado", afetado);
                cmd.Parameters.AddWithValue("@anterior", (object)valorAnterior ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@novo", (object)novoValor ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}