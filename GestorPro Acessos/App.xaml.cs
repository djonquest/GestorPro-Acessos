using Npgsql;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GestorPro_Acessos
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            int totalUsuarios = 0;

            try
            {
                
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();

                    
                    string query = "SELECT COUNT(*) FROM usuarios";
                    using (NpgsqlCommand comando = new NpgsqlCommand(query, conexao))
                    {
                        totalUsuarios = Convert.ToInt32(comando.ExecuteScalar());
                    }
                }


                if (totalUsuarios == 0)
                {
                    PrimeiroAdminWindow adminWindow = new PrimeiroAdminWindow();
                    adminWindow.Show();
                }
                else
                {
                    
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao ligar à base de dados: " + ex.Message,
                                "Erro de Ligação", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}
