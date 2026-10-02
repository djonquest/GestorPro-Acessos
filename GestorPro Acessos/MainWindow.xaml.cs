using Npgsql;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestorPro_Acessos
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CarregarDadosUsuario();
            AplicarPermissoes();
        }

        private void CarregarDadosUsuario()
        {
            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();
                    string query = "SELECT nome_completo, nome_usuario, tipo_usuario, ultimo_login, imagem_perfil FROM usuarios WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
                    {
                        cmd.Parameters.AddWithValue("@id", Sessao.UsuarioId);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtNome.Text = reader.GetString(0);
                                txtUsuario.Text = "@" + reader.GetString(1);
                                txtTipoUsuario.Text = reader.GetString(2);

                                
                                if (!reader.IsDBNull(3))
                                {
                                    DateTime ultimoLogin = reader.GetDateTime(3);
                                    txtUltimoLogin.Text = ultimoLogin.ToString("dd/MM/yyyy 'às' HH:mm");
                                }
                                else
                                {
                                    txtUltimoLogin.Text = "Nenhum acesso registado.";
                                }

                                string nomeAvatar = reader.GetString(4);
                                CarregarAvatar(nomeAvatar);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados do perfil: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CarregarAvatar(string nomeArquivo)
        {
            try
            {
                
                string uri = $"pack://application:,,,/Images/{nomeArquivo}";
                imgPerfil.ImageSource = new BitmapImage(new Uri(uri));
            }
            catch
            {
                // Em caso de falha no carregamento da imagem, mantém o fundo padrão
            }
        }

        private void AplicarPermissoes()
        {
            // Oculta funcionalidades exclusivas se o utilizador for comum
            if (!Sessao.EhAdministrador)
            {
                btnGerenciarUsuarios.Visibility = Visibility.Collapsed;
                btnAuditoria.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnGerenciarUsuarios_Click(object sender, RoutedEventArgs e)
        {
            ListaUsuariosWindow listaUsuarios = new ListaUsuariosWindow();
            listaUsuarios.Show();
            this.Close();
        }

        private void BtnAuditoria_Click(object sender, RoutedEventArgs e)
        {
            AuditoriaWindow auditoriaWin = new AuditoriaWindow();
            auditoriaWin.ShowDialog();
        }

        private void BtnAlterarMinhaSenha_Click(object sender, RoutedEventArgs e)
        {
            
            RedefinirSenhaWindow redefinir = new RedefinirSenhaWindow(Sessao.UsuarioId);
            redefinir.ShowDialog();
        }

        private void BtnMeuPerfil_Click(object sender, RoutedEventArgs e)
        {
            
            CadastroUsuarioWindow edicaoPerfil = new CadastroUsuarioWindow(Sessao.UsuarioId);
            edicaoPerfil.ShowDialog();

            
            CarregarDadosUsuario();
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            
            Sessao.UsuarioId = 0;
            Sessao.TipoUsuario = null;

            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}