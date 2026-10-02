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
    public partial class RedefinirSenhaWindow : Window
    {
        private int _idUsuarioAlvo;

        public RedefinirSenhaWindow(int idUsuarioAlvo)
        {
            InitializeComponent();
            _idUsuarioAlvo = idUsuarioAlvo;
        }

        private void BtnSalvarSenha_Click(object sender, RoutedEventArgs e)
        {
            string novaSenha = txtNovaSenha.Password;
            string confirmaSenha = txtConfirmaSenha.Password;

            if (string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmaSenha))
            {
                MessageBox.Show("Preencha ambos os campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (novaSenha.Length < 8)
            {
                MessageBox.Show("A palavra-passe deve possuir no mínimo 8 caracteres.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (novaSenha != confirmaSenha)
            {
                MessageBox.Show("As palavras-passe não coincidem.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string novoHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);

            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();
                    using (NpgsqlTransaction transacao = conexao.BeginTransaction())
                    {
                        
                        string queryUpdate = "UPDATE usuarios SET senha = @senha WHERE id = @id";
                        using (NpgsqlCommand cmdUpdate = new NpgsqlCommand(queryUpdate, conexao, transacao))
                        {
                            cmdUpdate.Parameters.AddWithValue("@senha", novoHash);
                            cmdUpdate.Parameters.AddWithValue("@id", _idUsuarioAlvo);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        
                        string queryAuditoria = @"INSERT INTO auditoria (id_usuario_responsavel, tipo_operacao, registro_afetado, novo_valor) 
                                                  VALUES (@responsavel, 'Redefinição de senha', @afetado, 'Nova senha atribuída')";
                        using (NpgsqlCommand cmdAuditoria = new NpgsqlCommand(queryAuditoria, conexao, transacao))
                        {
                            cmdAuditoria.Parameters.AddWithValue("@responsavel", Sessao.UsuarioId);
                            cmdAuditoria.Parameters.AddWithValue("@afetado", $"Utilizador ID: {_idUsuarioAlvo}");
                            cmdAuditoria.ExecuteNonQuery();
                        }

                        transacao.Commit();
                    }
                }

                MessageBox.Show("Palavra-passe redefinida com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao redefinir a palavra-passe: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
