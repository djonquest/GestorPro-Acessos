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
    public partial class PrimeiroAdminWindow : Window
    {
        public PrimeiroAdminWindow()
        {
            InitializeComponent();
        }

        private void BtnCriarAdmin_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(txtNomeCompleto.Text) ||
                string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtSenha.Password) ||
                string.IsNullOrWhiteSpace(txtConfirmaSenha.Password))
            {
                MessageBox.Show("Todos os campos são obrigatórios.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Por favor, introduza um endereço de e-mail válido (exemplo: nome@dominio.com).", "Validação de E-mail", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            if (txtSenha.Password.Length < 8)
            {
                MessageBox.Show("A palavra-passe deve possuir no mínimo 8 caracteres.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            if (txtSenha.Password != txtConfirmaSenha.Password)
            {
                MessageBox.Show("A palavra-passe e a confirmação devem ser iguais.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            string avatarEscolhido = "avatar1.png";
            if (cmbAvatar.SelectedItem is ComboBoxItem itemSelecionado && itemSelecionado.Tag != null)
            {
                avatarEscolhido = itemSelecionado.Tag.ToString();
            }

            
            string hashSenha = BCrypt.Net.BCrypt.HashPassword(txtSenha.Password);

            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();

                    
                    using (NpgsqlTransaction transacao = conexao.BeginTransaction())
                    {
                        
                        string queryInsert = @"INSERT INTO usuarios 
                            (nome_completo, nome_usuario, email, senha, tipo_usuario, imagem_perfil, status_ativo, bloqueado) 
                            VALUES (@nome, @usuario, @email, @senha, 'Administrador', @avatar, true, false) RETURNING id;";

                        int novoAdminId = 0;
                        using (NpgsqlCommand cmdInsert = new NpgsqlCommand(queryInsert, conexao, transacao))
                        {
                            cmdInsert.Parameters.AddWithValue("@nome", txtNomeCompleto.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@usuario", txtUsuario.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@senha", hashSenha);
                            cmdInsert.Parameters.AddWithValue("@avatar", avatarEscolhido);

                            novoAdminId = Convert.ToInt32(cmdInsert.ExecuteScalar());
                        }

                        
                        string queryAuditoria = @"INSERT INTO auditoria 
                            (id_usuario_responsavel, tipo_operacao, registro_afetado, novo_valor) 
                            VALUES (@responsavel, 'Cadastro de usuário', @afetado, @detalhes);";

                        using (NpgsqlCommand cmdAuditoria = new NpgsqlCommand(queryAuditoria, conexao, transacao))
                        {
                            cmdAuditoria.Parameters.AddWithValue("@responsavel", novoAdminId);
                            cmdAuditoria.Parameters.AddWithValue("@afetado", "Utilizador: " + txtUsuario.Text.Trim());
                            cmdAuditoria.Parameters.AddWithValue("@detalhes", "Criação do primeiro administrador do sistema.");
                            cmdAuditoria.ExecuteNonQuery();
                        }

                        transacao.Commit();
                    }
                }

                MessageBox.Show("Primeiro administrador criado com sucesso! Faça login para aceder ao sistema.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == "23505") 
            {
                MessageBox.Show("O nome de utilizador ou o e-mail inseridos já estão registados no sistema.", "Erro de Duplicação", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao comunicar com a base de dados: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
