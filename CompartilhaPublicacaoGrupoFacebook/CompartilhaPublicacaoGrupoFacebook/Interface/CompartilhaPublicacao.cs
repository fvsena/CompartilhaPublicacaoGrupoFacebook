using CompartilhaPublicacaoGrupoFacebook.Navegadores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CompartilhaPublicacaoGrupoFacebook.Interface
{
    public class CompartilhaPublicacao
    {
        private GoogleChrome Chrome = null;
        public void Iniciar(bool loop = false, bool exibirNavegador = false)
        {
            string usuario;
            string senha;
            string nomePerfil;
            string urlPublicacao;

            Console.WriteLine("### COMPARTILHAMENTO EM GRUPOS FACEBOOK ###");
            Console.Write("Login: ");
            usuario = Console.ReadLine();

            Console.Write("Senha: ");
            senha = Console.ReadLine();

            Console.Write("Nome do perfil: ");
            nomePerfil = Console.ReadLine();

            Console.Write("URL: ");
            urlPublicacao = Console.ReadLine();

            //INICIA PUBLICACAO
            PROCESSO:
            ProcessoPublicacao(usuario, senha, urlPublicacao, nomePerfil, exibirNavegador);

            if (loop)
            {
                goto PROCESSO;
            }
            
        }

        private void ProcessoPublicacao(string usuario, string senha, string urlPublicacao, string nomePerfil, bool exibirNavegador)
        {
            EscreveLog($"Iniciando processo de compartilhamento em grupos");
            Chrome = new GoogleChrome(exibirNavegador);
            int step = 0;
            try
            {
                string grupo = "";
                while (step <= 5)
                {
                    try
                    {
                        switch (step)
                        {
                            case 0:
                                LoginFacebook(usuario, senha);
                                break;
                            case 1:
                                AcessaTelaPagina(urlPublicacao);
                                break;
                            case 2:
                                SelecionaPerfil(nomePerfil);
                                break;
                            case 3:
                                IniciaCompartilhamento();
                                break;
                            case 4:
                                if (!Publicar())
                                {
                                    ScrollListaDeGrupos();
                                    if (!Publicar())
                                    {
                                        goto case 5;
                                    }
                                }
                                step = 4;
                                goto case 4;
                            case 5:
                                EscreveLog("Processo finalizado");
                                break;
                        }
                        step++;
                    }
                    catch (Exception ex)
                    {
                        EscreveLog($"Falha ao realizar publicação", true);
                        Console.WriteLine($"Falha ao realizar publicação no grupo {grupo}: {ex.Message}");
                        step = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                EscreveLog(ex.Message);
            }
            finally
            {
                Chrome.EncerraDriver();
                Chrome.EncerraNavegador();
            }
        }

        private void EscreveLog(string texto, bool escreveTxt = false)
        {
            var txt = $"{DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss")} - {texto}";
            Console.WriteLine(txt);
            if (escreveTxt)
            {
                using (StreamWriter writer = new StreamWriter("log.txt", true))
                {
                    writer.WriteLine(txt);
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        private void LoginFacebook(string login, string senha)
        {
            try
            {
                EscreveLog($"Acessando a tela de login");
                Chrome.Navegador.Navigate().GoToUrl("https://www.facebook.com");
                Chrome.EscreveElemento("email", login, 10, 2, true);
                Chrome.EscreveElemento("pass", senha, 10, 2, true);

                var btnEntrar = Chrome.LocalizaElementoPropriedade("input", "value", "Entrar", 2, 2, false);
                if (btnEntrar == null) btnEntrar = Chrome.LocalizaElementoPropriedade("button", "type", "submit", 2, 2, false);
                if (btnEntrar == null) throw new Exception("Não foi possível localizar o botão Entrar");
                btnEntrar.Click();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AcessaTelaPagina(string urlPublicacao)
        {
            try
            {
                EscreveLog("Acessando link da publicação");
                Thread.Sleep(5000);
                Chrome.Navegador.Navigate().GoToUrl(urlPublicacao);
                Chrome.LocalizaElementoPropriedade("button", "aria-label", "Seletor de voz", 25, 2, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SelecionaPerfil(string nomePerfil)
        {
            try
            {
                EscreveLog("Selecionando o perfil pessoal para publicação");
                Thread.Sleep(5000);
                Chrome.ClicaElementoPropriedade("button", "aria-label", "Seletor de voz", 10, 2, true);
                Chrome.ClicaElementoTexto("span", nomePerfil, 10, 2, true);
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void IniciaCompartilhamento()
        {
            try
            {
                EscreveLog("Clicando em Compartilhar em um Grupo");
                Thread.Sleep(2000);
                Chrome.ClicaElementoTexto("span", "Compartilhar", 10, 2, true);
                Chrome.ClicaElementoTexto("span", "Compartilhar em um grupo", 10, 2, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ExisteBotaoCompartilhar()
        {
            try
            {
                EscreveLog($"Verificando se existe o botão compartilhar");
                Thread.Sleep(2000);
                var elemento = Chrome.LocalizaElementoPropriedade("div", "aria-label", "Compartilhar", 10, 2, false);
                return elemento != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Publicar()
        {
            try
            {
                EscreveLog($"Verificando se existe o botão compartilhar");
                Thread.Sleep(2000);
                var elemento = Chrome.LocalizaElementoPropriedade("div", "aria-label", "Compartilhar", 10, 2, false);

                if (elemento == null)
                {
                    return false;
                }

                EscreveLog($"Efetivando a publicação");
                Thread.Sleep(2000);
                elemento.Click();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ScrollListaDeGrupos()
        {
            try
            {
                EscreveLog($"Descendo a lista de grupos");
                Chrome.Navegador.ExecuteScript(JavaScript_ScrollListaDeGrupos(), null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string JavaScript_ScrollListaDeGrupos()
        {
            return @"var element = document.evaluate(""/html/body/div[1]/div/div[1]/div[1]/div[4]/div/div/div[1]/div/div[2]/div/div/div/div[3]/div[3]/div/div[2]/div"", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
                    element.scrollTo(0, element.scrollHeight+100);";
        }
    }
}
