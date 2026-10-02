using System;
using System.Collections.Generic;
using System.Text;

namespace GestorPro_Acessos
{
    public static class Sessao
    {
        
        public static int UsuarioId { get; set; }
        public static string TipoUsuario { get; set; }

        public static bool EhAdministrador => TipoUsuario == "Administrador";
    }
}
