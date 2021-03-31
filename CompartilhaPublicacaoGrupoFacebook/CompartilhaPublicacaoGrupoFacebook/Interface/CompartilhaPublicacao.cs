using CompartilhaPublicacaoGrupoFacebook.Model;
using CompartilhaPublicacaoGrupoFacebook.Navegadores;
using CompartilhaPublicacaoGrupoFacebook.Util;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CompartilhaPublicacaoGrupoFacebook.Interface
{
    public class CompartilhaPublicacao
    {
        private GoogleChrome Chrome = null;
        private List<Publicacao> resultados = new List<Publicacao>();
        Queue<string> grupos;


        public void Iniciar(bool loop = false, bool exibirNavegador = false)
        {
            string usuario;
            string senha;
            string nomePerfil;
            string urlPublicacao;
            string navegador;

            Console.WriteLine("### COMPARTILHAMENTO EM GRUPOS FACEBOOK ###");
            Console.Write("Login: ");
            usuario = Console.ReadLine();

            Console.Write("Senha: ");
            senha = Console.ReadLine();

            Console.Write("Nome do perfil: ");
            nomePerfil = Console.ReadLine();

            Console.Write("URL: ");
            urlPublicacao = Console.ReadLine();

            Console.Write("Exibir navegador? SIM ou NÃO ");
            navegador = Console.ReadLine();

            while (!navegador.ToUpper().Equals("SIM") && !navegador.ToUpper().Equals("NÃO"))
            {
                Console.Write("Exibir navegador? SIM ou NÃO ");
                navegador = Console.ReadLine();
            }

            exibirNavegador = navegador.ToUpper().Equals("SIM");

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
            
            int step = 0;
            try
            {
                this.grupos = new CSVUtil().CarregaGrupos("arquivos\\grupos.csv");
                string grupo = "";
                while (step <= 5)
                {
                    try
                    {
                        switch (step)
                        {
                            case 0:
                                Chrome = new GoogleChrome(exibirNavegador);
                                LoginFacebook(usuario, senha);
                                break;
                            case 1:
                                AcessaTelaPagina(urlPublicacao);
                                break;
                            case 2:
                                IniciaCompartilhamento();
                                break;
                            case 3:
                                SelecionaPerfil(nomePerfil);
                                break;
                            case 4:
                                if (!grupos.Any())
                                {
                                    step = 6;
                                    goto case 6;
                                }

                                grupo = grupos.Dequeue();
                                BuscaGrupo(grupo);

                                if (!AcessaTelaCompartilharGrupo(grupo))
                                {
                                    resultados.Add(new Publicacao() { Sucesso = false, Grupo = grupo, DataPublicacao = DateTime.Now, Mensagem = "Grupo não localizado" });
                                    step = 1;
                                    goto case 1;
                                }

                                break;
                               
                            case 5:
                                Publicar();
                                resultados.Add(new Publicacao() { Sucesso = true, Grupo = grupo, DataPublicacao = DateTime.Now, Mensagem = "Publicação realizada"});
                                step = 1;
                                goto case 1;
                            case 6:
                                EscreveLog("Processo finalizado");
                                break;
                        }
                        step++;
                    }
                    catch (Exception ex)
                    {
                        EscreveLog($"Falha ao realizar publicação", true);
                        resultados.Add(new Publicacao() { Sucesso = false, Grupo = grupo, DataPublicacao = DateTime.Now, Mensagem = ex.Message });
                        Console.WriteLine($"Falha ao realizar publicação no grupo {grupo}: {ex.Message}");
                        Chrome = new GoogleChrome(exibirNavegador);
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
                //Chrome.EncerraNavegador();
            }
            CSVUtil.EscreverCSV(resultados, $"resultado_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv", false);
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
                Chrome.LocalizaElementoTexto("span", "Compartilhar", 10, 2, true);
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
                Chrome.ClicaElementoPropriedade("label", "aria-label", "Compartilhar como", 10, 2, true);

                var perfis = Chrome.LocalizaElementosPropriedade("div", "role", "menuitemradio", 10, 2, true);
                foreach (var item in perfis)
                {
                    if (item.Text.Equals(nomePerfil))
                    {
                        item.Click();
                        break;
                    }
                }
                //var spanNomePerfil = Chrome.LocalizaElementoTexto("span", nomePerfil, 10, 2, true);
                //((IJavaScriptExecutor)Chrome.Navegador).ExecuteScript("arguments[0].scrollIntoView(true);", spanNomePerfil);
                //spanNomePerfil.Click();
                //Chrome.ClicaElementoTexto("span", nomePerfil, 10, 2, true);
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

        private void BuscaGrupo(string grupo)
        {
            try
            {
                EscreveLog("Realizando a busca do grupo");
                Thread.Sleep(2000);
                Chrome.EscreveElementoProprieadade("input", "aria-label", "Procurar grupos", grupo, 10, 2, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool AcessaTelaCompartilharGrupo(string grupo)
        {
            try
            {
                bool localizado = false;
                var spanGrupo = Chrome.LocalizaElementoTexto("span", grupo, 3, 1, false);
                if (spanGrupo != null)
                {
                    localizado = true;
                    spanGrupo.Click();
                }
                else
                {
                    EscreveLog($"Grupo não localizado: {grupo}");
                    Chrome.EscreveElementoProprieadade("input", "aria-label", "Procurar grupos", "", 10, 2, true);
                }
                return localizado;
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

        private void Publicar()
        {
            try
            {
                EscreveLog($"Efetivando a publicação");
                Thread.Sleep(2000);
                Chrome.ClicaElementoPropriedade("div", "aria-label", "Publicar", 10, 2, false);
                Thread.Sleep(5000);
                EscreveLog($"Publicação realizada com sucesso");
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

                var divMae = Chrome.LocalizaElementoPropriedade("div", "aria-label", "Compartilhar em um grupo", 10, 2, true);

                Chrome.ExecutarScript(JavaScript_ScrollListaDeGrupos(), divMae);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string JavaScript_ScrollListaDeGrupos()
        {
            return @"
                    console.log(""arguments[0]"");
                    var divs = arguments[0].getElementsByTagName(""div"");
                    for (i = 0; i < divs.length; i++){
                    if(divs.item(i).getAttribute(""aria-busy"") != null && divs.item(i).getAttribute(""aria-busy"") == ""false""){
                        divs.item(i).scroll(0, divs.item(i).scrollTop + 1000);
                            break;
                        }
                    }";
        }
    }
}
