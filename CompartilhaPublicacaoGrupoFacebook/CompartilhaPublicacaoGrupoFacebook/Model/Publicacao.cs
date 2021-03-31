using System;

namespace CompartilhaPublicacaoGrupoFacebook.Model
{
    public class Publicacao
    {
        public string Grupo { get; set; }
        public bool Sucesso { get; set; }
        public DateTime DataPublicacao { get; set; }
        public string Mensagem { get; set; }

        public override string ToString()
        {
            return $"{DataPublicacao.ToString("dd/MM/yyyy HH:mm:ss")};{Grupo};{Sucesso};{Mensagem};";
        }
    }
}
