using CompartilhaPublicacaoGrupoFacebook.Interface;
using System;

namespace CompartilhaPublicacaoGrupoFacebook
{
    class Program
    {
        static void Main(string[] args)
        {
            new CompartilhaPublicacao().Iniciar(true, true);
        }
    }
}
