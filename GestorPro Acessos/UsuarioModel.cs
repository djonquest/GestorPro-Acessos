using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GestorPro_Acessos
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string NomeUsuario { get; set; }
        public string Email { get; set; }
        public string TipoUsuario { get; set; }
        public bool StatusAtivo { get; set; }
        public string ImagemPerfil { get; set; }
        public DateTime? UltimoLogin { get; set; }

        

        public string ImagemCaminho => $"pack://application:,,,/Images/{ImagemPerfil}";

        public string StatusTexto => StatusAtivo ? "Ativo" : "Inativo";

        public string CorStatus => StatusAtivo ? "#44BD32" : "#E84118";

        public string UltimoLoginTexto => UltimoLogin.HasValue
            ? UltimoLogin.Value.ToString("dd/MM/yyyy HH:mm")
            : "Nenhum acesso registado.";


        public Visibility BotoesAcaoVisiveis => Sessao.EhAdministrador ? Visibility.Visible : Visibility.Collapsed;
    }
}
