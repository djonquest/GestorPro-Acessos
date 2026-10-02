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
    
    public class RegistroAuditoria
    {
        public DateTime DataHora { get; set; }
        public string DataHoraTexto => DataHora.ToString("dd/MM/yyyy HH:mm:ss");
        public string NomeResponsavel { get; set; }
        public string TipoOperacao { get; set; }
        public string RegistroAfetado { get; set; }
        public string NovoValor { get; set; }
    }

    public class RegistroEvento
    {
        public DateTime DataHora { get; set; }
        public string DataHoraTexto => DataHora.ToString("dd/MM/yyyy HH:mm:ss");
        public string NomeUsuarioTentativa { get; set; }
        public string TipoEvento { get; set; }
        public string ResultadoOperacao { get; set; }
    }

    public partial class AuditoriaWindow : Window
    {
        public AuditoriaWindow()
        {
            InitializeComponent();
            CarregarAuditoria();
            CarregarEventos();
        }

        private void CarregarAuditoria()
        {
            List<RegistroAuditoria> lista = new List<RegistroAuditoria>();
            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();
                    
                    string query = @"SELECT a.data_hora, COALESCE(u.nome_completo, 'Sistema/Excluído'), a.tipo_operacao, a.registro_afetado, a.novo_valor 
                                     FROM auditoria a 
                                     LEFT JOIN usuarios u ON a.id_usuario_responsavel = u.id 
                                     ORDER BY a.data_hora DESC";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new RegistroAuditoria
                                {
                                    
                                    DataHora = DateTime.SpecifyKind(reader.GetDateTime(0), DateTimeKind.Utc).ToLocalTime(),
                                    NomeResponsavel = reader.GetString(1),
                                    TipoOperacao = reader.GetString(2),
                                    RegistroAfetado = reader.IsDBNull(3) ? "-" : reader.GetString(3),
                                    NovoValor = reader.IsDBNull(4) ? "-" : reader.GetString(4)
                                });
                            }
                        }
                    }
                }
                dgAuditoria.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar auditoria: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CarregarEventos()
        {
            List<RegistroEvento> lista = new List<RegistroEvento>();
            try
            {
                using (NpgsqlConnection conexao = new NpgsqlConnection(ConexaoDB.StringConexao))
                {
                    conexao.Open();
                    string query = @"SELECT data_hora, nome_usuario_tentativa, tipo_evento, resultado_operacao 
                                     FROM eventos_autenticacao 
                                     ORDER BY data_hora DESC";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexao))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new RegistroEvento
                                {
                                    
                                    DataHora = DateTime.SpecifyKind(reader.GetDateTime(0), DateTimeKind.Utc).ToLocalTime(),
                                    NomeUsuarioTentativa = reader.GetString(1),
                                    TipoEvento = reader.GetString(2),
                                    ResultadoOperacao = reader.GetString(3)
                                });
                            }
                        }
                    }
                }
                dgEventos.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar eventos: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}