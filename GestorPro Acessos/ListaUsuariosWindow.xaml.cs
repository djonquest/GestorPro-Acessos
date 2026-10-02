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
    public partial class ListaUsuariosWindow : Window
    {
        public ListaUsuariosWindow()
        {
            InitializeComponent();
            CarregarUsuarios();

            
            if (!Sessao.EhAdministrador)
            {
                btnAdicionarUsuario.Visibility = Visibility.Collapsed;
            }
        }

        private void CarregarUsuarios(string filtroPesquisa = "", string filtroPerfil = "Todos", string filtroStatus = "Todos")
        {
            List<UsuarioModel> lista = new List<UsuarioModel>();

            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();

                    string query = "SELECT id, nome_completo, nome_usuario, email, tipo_usuario, status_ativo, imagem_perfil, ultimo_login FROM usuarios WHERE 1=1";

                    
                    if (!string.IsNullOrWhiteSpace(filtroPesquisa))
                    {
                        query += " AND (nome_completo ILIKE @pesquisa OR nome_usuario ILIKE @pesquisa OR email ILIKE @pesquisa)";
                    }

                    if (filtroPerfil != "Todos")
                    {
                        query += " AND tipo_usuario = @perfil";
                    }

                    if (filtroStatus != "Todos")
                    {
                        bool isAtivo = filtroStatus == "Ativo";
                        query += " AND status_ativo = @status";
                    }

                    query += " ORDER BY nome_completo ASC";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
                    {
                        if (!string.IsNullOrWhiteSpace(filtroPesquisa))
                            cmd.Parameters.AddWithValue("@pesquisa", "%" + filtroPesquisa + "%");

                        if (filtroPerfil != "Todos")
                            cmd.Parameters.AddWithValue("@perfil", filtroPerfil);

                        if (filtroStatus != "Todos")
                            cmd.Parameters.AddWithValue("@status", filtroStatus == "Ativo");

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuarioModel usuario = new UsuarioModel
                                {
                                    Id = reader.GetInt32(0),
                                    NomeCompleto = reader.GetString(1),
                                    NomeUsuario = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    TipoUsuario = reader.GetString(4),
                                    StatusAtivo = reader.GetBoolean(5),
                                    ImagemPerfil = reader.GetString(6),
                                    UltimoLogin = reader.IsDBNull(7) ? (DateTime?)null : DateTime.SpecifyKind(reader.GetDateTime(7), DateTimeKind.Utc).ToLocalTime()
                                };
                                lista.Add(usuario);
                            }
                        }
                    }
                }

                lstUsuarios.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao listar utilizadores: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            string pesquisa = txtPesquisa.Text.Trim();
            string perfil = (cmbFiltroPerfil.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";
            string status = (cmbFiltroStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";

            CarregarUsuarios(pesquisa, perfil, status);
        }

        private void BtnAdicionarUsuario_Click(object sender, RoutedEventArgs e)
        {
            CadastroUsuarioWindow cadastro = new CadastroUsuarioWindow();
            cadastro.ShowDialog();
            CarregarUsuarios(); 
        }

        private void BtnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int idSelecionado = Convert.ToInt32(btn.Tag);
                CadastroUsuarioWindow edicao = new CadastroUsuarioWindow(idSelecionado);
                edicao.ShowDialog();
                CarregarUsuarios();
            }
        }

        private void BtnExcluirUsuario_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int idSelecionado = Convert.ToInt32(btn.Tag);

                if (idSelecionado == Sessao.UsuarioId)
                {
                    MessageBox.Show("Não é permitido excluir a própria conta.", "Operação Negada", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult confirmacao = MessageBox.Show("Deseja realmente excluir este utilizador?", "Confirmação de Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmacao == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                        {
                            conexao.Open();
                            using (NpgsqlTransaction transacao = conexao.BeginTransaction())
                            {
                                
                                string queryCheckAdmin = "SELECT tipo_usuario, status_ativo FROM usuarios WHERE id = @id";
                                string tipo = "";
                                bool ativo = false;
                                string nomeDeletado = "";

                                using (NpgsqlCommand cmdCheck = new NpgsqlCommand("SELECT tipo_usuario, status_ativo, nome_usuario FROM usuarios WHERE id = @id", conexao, transacao))
                                {
                                    cmdCheck.Parameters.AddWithValue("@id", idSelecionado);
                                    using (NpgsqlDataReader reader = cmdCheck.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            tipo = reader.GetString(0);
                                            ativo = reader.GetBoolean(1);
                                            nomeDeletado = reader.GetString(2);
                                        }
                                    }
                                }

                                if (tipo == "Administrador" && ativo)
                                {
                                    string queryCountAdmins = "SELECT COUNT(*) FROM usuarios WHERE tipo_usuario = 'Administrador' AND status_ativo = true";
                                    using (NpgsqlCommand cmdCount = new NpgsqlCommand(queryCountAdmins, conexao, transacao))
                                    {
                                        int totalAdmins = Convert.ToInt32(cmdCount.ExecuteScalar());
                                        if (totalAdmins <= 1)
                                        {
                                            MessageBox.Show("Não é possível excluir o último administrador ativo do sistema.", "Operação Negada", MessageBoxButton.OK, MessageBoxImage.Error);
                                            return;
                                        }
                                    }
                                }

                               
                                string queryAuditoria = @"INSERT INTO auditoria (id_usuario_responsavel, tipo_operacao, registro_afetado, novo_valor) 
                                                        VALUES (@responsavel, 'Exclusão de usuário', @afetado, 'Conta excluída permanentemente')";
                                using (NpgsqlCommand cmdAuditoria = new NpgsqlCommand(queryAuditoria, conexao, transacao))
                                {
                                    cmdAuditoria.Parameters.AddWithValue("@responsavel", Sessao.UsuarioId);
                                    cmdAuditoria.Parameters.AddWithValue("@afetado", $"Utilizador ID: {idSelecionado} ({nomeDeletado})");
                                    cmdAuditoria.ExecuteNonQuery();
                                }

                                
                                using (NpgsqlCommand cmdDelEventos = new NpgsqlCommand("DELETE FROM eventos_autenticacao WHERE id_usuario_relacionado = @id", conexao, transacao))
                                {
                                    cmdDelEventos.Parameters.AddWithValue("@id", idSelecionado);
                                    cmdDelEventos.ExecuteNonQuery();
                                }

                                
                                using (NpgsqlCommand cmdUpdateAuditoria = new NpgsqlCommand("UPDATE auditoria SET id_usuario_responsavel = NULL WHERE id_usuario_responsavel = @id", conexao, transacao))
                                {
                                    cmdUpdateAuditoria.Parameters.AddWithValue("@id", idSelecionado);
                                    cmdUpdateAuditoria.ExecuteNonQuery();
                                }
                                

                                
                                using (NpgsqlCommand cmdDelUsuario = new NpgsqlCommand("DELETE FROM usuarios WHERE id = @id", conexao, transacao))
                                {
                                    cmdDelUsuario.Parameters.AddWithValue("@id", idSelecionado);
                                    cmdDelUsuario.ExecuteNonQuery();
                                }

                                transacao.Commit();
                                MessageBox.Show("Utilizador excluído com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                                CarregarUsuarios();

                                
                            }
                        }
                    }
                    catch (PostgresException pgEx) when (pgEx.SqlState == "23503") // Erro de Foreign Key
                    {
                        MessageBox.Show("Este utilizador possui registos de auditoria associados e não pode ser excluído diretamente. Recomendamos que altere o estado para 'Inativo'.", "Restrição do Sistema", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao excluir utilizador: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnAlterarSenha_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int idSelecionado = Convert.ToInt32(btn.Tag);
                RedefinirSenhaWindow redefinir = new RedefinirSenhaWindow(idSelecionado);
                redefinir.ShowDialog();
            }
        }

        
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow main = new MainWindow();
            main.Show();

            
            this.Close();
        }
    }
}