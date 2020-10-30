using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CompartilhaPublicacaoGrupoFacebook.Navegadores
{
    internal class GoogleChrome
    {
        #region Propriedades públicas
        public string ProcessIdDriver { get; set; }
        public string ProcessIdNavegador { get; set; }
        public ChromeDriver Navegador { get; set; }
        public IWebElement Elemento { get; set; }
        #endregion

        #region Propriedades privadas
        private static readonly string path = $"{GetBasePath()}\\Arquivos";
        private ChromeDriverService ChromeDriverService = ChromeDriverService.CreateDefaultService(path);
        private ChromeOptions OptionsChrome;
        #endregion

        #region Construtores
        /// <summary>
        /// Inicializa os componentes do navegador
        /// </summary>
        public GoogleChrome(bool isVisible = false)
        {
            ChromeDriverService.HideCommandPromptWindow = true;
            OptionsChrome = new ChromeOptions();
            OptionsChrome.AddArguments("--no-default-browser-check", "--disable-infobars", "no-sandbox", "--ignore-certificate-errors", "--disable-popup-blocking", "--app=", "--disable-notifications");
            if (isVisible)
            {
                OptionsChrome.AddArgument("--start-maximized");
            }
            else
            {
                OptionsChrome.AddArgument("--headless");
            }
            OptionsChrome.AddUserProfilePreference("credentials_enable_service", false);
            OptionsChrome.AddUserProfilePreference("profile.password_manager_enabled", false);
            OptionsChrome.AddAdditionalCapability("useAutomationExtension", false);
            Navegador = new ChromeDriver(ChromeDriverService, OptionsChrome, TimeSpan.FromSeconds(120));
            ProcessIdDriver = ChromeDriverService.ProcessId.ToString();
        }
        #endregion

        #region Métodos públicos
        /// <summary>
        /// Localiza um elemento HTML no documento
        /// </summary>
        /// <param name="id">ID do elemento a ser localizado</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public IWebElement LocalizaElemento(string id, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement elemento = null;
            int tentativas = 0;

            while (tentativas < maximoTentativas && elemento == null)
            {
                tentativas++;
                try
                {
                    elemento = Navegador.FindElementById(id);
                    if (elemento == null)
                    {
                        Thread.Sleep(1000 * delay);
                    }
                }
                catch
                {
                    Thread.Sleep(1000 * delay);
                }
            }

            if (elemento == null && lancaExcecao)
            {
                throw new Exception(string.Format("Não foi possível localizar o elemento {0}", id));
            }
            return elemento;
        }

        /// <summary>
        /// Localiza um elemento e escreve um conteúdo
        /// </summary>
        /// <param name="id">ID do elemento a ser localizado</param>
        /// <param name="texto">Texto que será digitado no elemento</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public void EscreveElemento(string id, string texto, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement textBox = LocalizaElemento(id, maximoTentativas, delay, lancaExcecao);
            textBox.Clear();
            textBox.SendKeys(texto);
        }

        /// <summary>
        /// Localiza um elemento e escreve um conteúdo
        /// </summary>
        /// <param name="tag">Tag do elemento a ser localizado</param>
        /// <param name="propriedade">Propriedade do elemento que será analisada</param>
        /// <param name="valor">Valor da propriedade do elemento que será analisado</param>
        /// <param name="texto">Texto que será digitado no elemento</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public void EscreveElementoProprieadade(string tag, string propriedade, string valor, string texto, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement textBox = LocalizaElementoPropriedade(tag, propriedade, valor, maximoTentativas, delay, lancaExcecao);
            textBox.Clear();
            textBox.SendKeys(texto);
        }

        /// <summary>
        /// Localiza um elemento e realiza o clique
        /// </summary>
        /// <param name="id">ID do elemento a ser localizado</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public void ClicaElemento(string id, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement elemento = LocalizaElemento(id, maximoTentativas, delay, lancaExcecao);
            elemento.Click();
        }

        /// <summary>
        /// Localiza um elemento pelo Texto
        /// </summary>
        /// <param name="tag">Tag do elemento que será localizado</param>
        /// <param name="texto">Texto interno do elemento</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public IWebElement LocalizaElementoTexto(string tag, string texto, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement elemento = null;
            int tentativas = 0;

            while (tentativas < maximoTentativas && elemento == null)
            {
                tentativas++;
                try
                {
                    foreach (IWebElement e in Navegador.FindElementsByTagName(tag))
                    {
                        if (e.Text != null && e.Text.ToUpper().Equals(texto.ToUpper()))
                        {
                            elemento = e;
                            break;
                        }
                    }
                    if (elemento == null)
                    {
                        Thread.Sleep(1000 * delay);
                    }
                }
                catch
                {
                    Thread.Sleep(1000 * delay);
                }
            }

            if (elemento == null && lancaExcecao)
            {
                throw new Exception(string.Format("Não foi possível localizar o elemento contendo a texto {0}", texto));
            }
            return elemento;
        }

        /// <summary>
        /// Localiza um elemento pelo Texto
        /// </summary>
        /// <param name="tag">Tag do elemento que será localizado</param>
        /// <param name="texto">Texto interno do elemento</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="verificaInnerText">Permite ou não recusar um elemento com inner text nulo ou branco</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public IWebElement LocalizaElementoTexto(string tag, string texto, int maximoTentativas, int delay, bool verificaInnerText, bool lancaExcecao)
        {
            IWebElement elemento = null;
            int tentativas = 0;

            while (tentativas < maximoTentativas && elemento == null)
            {
                tentativas++;
                try
                {
                    foreach (IWebElement e in Navegador.FindElementsByTagName(tag))
                    {
                        if (e.Text != null && e.Text.ToUpper().Equals(texto.ToUpper()))
                        {
                            elemento = e;
                            break;
                        }
                    }
                    if (elemento == null)
                    {
                        Thread.Sleep(1000 * delay);
                    }
                    else if (verificaInnerText && string.IsNullOrEmpty(elemento.Text))
                    {
                        elemento = null;
                        Thread.Sleep(1000 * delay);
                    }
                }
                catch
                {
                    Thread.Sleep(1000 * delay);
                }
            }

            if (elemento == null && lancaExcecao)
            {
                throw new Exception(string.Format("Não foi possível localizar o elemento contendo a texto {0}", texto));
            }
            return elemento;
        }

        /// <summary>
        /// Localiza um elemento pelo texto e realiza o clique
        /// </summary>
        /// <param name="tag">Tag do elemento a ser localizado</param>
        /// <param name="texto">Conteúdo de texto do elemento a ser localizado</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public void ClicaElementoTexto(string tag, string texto, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement elemento = LocalizaElementoTexto(tag, texto, maximoTentativas, delay, lancaExcecao);
            elemento.Click();
        }

        /// <summary>
        /// Clica em um elemento localizado por uma propriedade
        /// </summary>
        /// <param name="tag">Tag do elemento que será clicado</param>
        /// <param name="propriedade">Título da propriedade</param>
        /// <param name="valor">Valor da propriedade</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas de localizar o elemento</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public void ClicaElementoPropriedade(string tag, string propriedade, string valor, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement elemento = LocalizaElementoPropriedade(tag, propriedade, valor, maximoTentativas, delay, lancaExcecao);
            elemento.Click();
        }

        /// <summary>
        /// Localiza um elemento pelo Texto
        /// </summary>
        /// <param name="tag">Tag do elemento que será localizado</param>
        /// <param name="propriedade">Atributo que será analisado</param>
        /// <param name="texto">Texto interno do elemento</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public IWebElement LocalizaElementoPropriedade(string tag, string propriedade, string texto, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement elemento = null;
            int tentativas = 0;

            while (tentativas < maximoTentativas && elemento == null)
            {
                tentativas++;
                try
                {
                    foreach (IWebElement e in Navegador.FindElementsByTagName(tag))
                    {
                        if (e.GetAttribute(propriedade) != null && e.GetAttribute(propriedade).ToUpper().Equals(texto.ToUpper()))
                        {
                            elemento = e;
                            break;
                        }
                    }
                    if (elemento == null)
                    {
                        Thread.Sleep(1000 * delay);
                    }
                }
                catch
                {
                    Thread.Sleep(1000 * delay);
                }
            }

            if (elemento == null && lancaExcecao)
            {
                throw new Exception(string.Format("Não foi possível localizar o elemento contendo a texto {0}", texto));
            }
            return elemento;
        }

        /// <summary>
        /// Localiza um elemento pelo Texto
        /// </summary>
        /// <param name="tag">Tag do elemento que será localizado</param>
        /// <param name="propriedade">Atributo que será analisado</param>
        /// <param name="texto">Texto interno do elemento</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento</param>
        /// <param name="delay">Tempo de espera entre as tentativas</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public List<IWebElement> LocalizaElementosPropriedade(string tag, string propriedade, string texto, int maximoTentativas, int delay, bool lancaExcecao)
        {
            int tentativas = 0;

            List<IWebElement> elementos = new List<IWebElement>();

            while (tentativas < maximoTentativas && elementos.Count == 0)
            {
                tentativas++;
                try
                {
                    foreach (IWebElement e in Navegador.FindElementsByTagName(tag))
                    {
                        if (e.GetAttribute(propriedade) != null && e.GetAttribute(propriedade).ToUpper().Equals(texto.ToUpper()))
                        {
                            elementos.Add(e);
                        }
                    }
                    if (elementos.Count == 0)
                    {
                        Thread.Sleep(1000 * delay);
                    }
                }
                catch
                {
                    Thread.Sleep(1000 * delay);
                }
            }

            if (elementos.Count == 0 && lancaExcecao)
            {
                throw new Exception(string.Format("Não foi possível localizar o elemento contendo a texto {0}", texto));
            }
            return elementos;
        }

        /// <summary>
        /// Executa uma função javascript
        /// </summary>
        /// <param name="funcao">Nome da função</param>
        public void ExecutarScript(string funcao)
        {
            Navegador.ExecuteScript(funcao);
            Thread.Sleep(50);
        }

        /// <summary>
        /// Seleciona uma opção pelo texto
        /// </summary>
        /// <param name="opcao">Texto da opção que será selecionada</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar o elemento a ser clicado</param>
        /// <param name="delay">Tempo de espera entre as tentativas de localizar o elemento</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso o elemento não seja localizado</param>
        public void SelecionaOpcao(string opcao, int maximoTentativas, int delay, bool lancaExcecao)
        {
            IWebElement elemento = LocalizaElementoTexto("option", opcao, 10, 2, true);
            elemento.Click();

        }

        /// <summary>
        /// Método que finaliza a execução do Chrome Driver
        /// </summary>
        public void EncerraDriver()
        {
            try
            {
                System.Diagnostics.Process[] chromeDriverProcesses = System.Diagnostics.Process.GetProcessesByName("chromedriver");
                foreach (var chromeDriverProcess in chromeDriverProcesses)
                {
                    if (chromeDriverProcess.Id.ToString().Equals(ProcessIdDriver))
                    {
                        chromeDriverProcess.Kill();
                        break;
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Encerra o google chrome
        /// </summary>
        public void EncerraNavegador()
        {
            try
            {
                Navegador.Quit();
                //System.Diagnostics.Process[] chromeProcesses = System.Diagnostics.Process.GetProcessesByName("chrome");
                //foreach (var chromeProcess in chromeProcesses)
                //{
                //    chromeProcess.Kill();
                //}
            }
            catch
            {

            }
        }

        /// <summary>
        /// Localiza uma janela pelo título
        /// </summary>
        /// <param name="janela">Título da janela</param>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar a janela</param>
        /// <param name="delay">Tempo de espera entre as tentativas de localizar a janela</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso a janela não seja localizada</param>
        public void LocalizaJanela(string janela, int maximoTentativas, int delay, bool lancaExcecao)
        {
            int tentativas = 0;
            bool localizado = false;
            while (!localizado && tentativas < maximoTentativas)
            {
                try
                {
                    Navegador.SwitchTo().Window(janela);
                    localizado = true;
                }
                catch
                {
                    Thread.Sleep(1000 * delay);
                }
                tentativas++;
            }
            if (!localizado && lancaExcecao)
            {
                throw new Exception("A janela não pode ser localizada");
            }
        }

        /// <summary>
        /// Localiza a janela ativa
        /// </summary>
        /// <param name="maximoTentativas">Quantidade máxima de tentativas de localizar a janela</param>
        /// <param name="delay">Tempo de espera entre as tentativas de localizar a janela</param>
        /// <param name="lancaExcecao">Permite ou não lançar exceção caso a janela não seja localizada</param>
        public void LocalizaJanelaAtiva(int maximoTentativas, int delay, bool lancaExcecao)
        {
            int tentativas = 0;
            bool localizado = false;
            while (!localizado && tentativas < maximoTentativas)
            {
                try
                {
                    Navegador.SwitchTo().DefaultContent();
                    localizado = true;
                }
                catch
                {
                    Thread.Sleep(1000 * delay);
                }
                tentativas++;
            }
            if (!localizado && lancaExcecao)
            {
                throw new Exception("A janela não pode ser localizada");
            }
        }
        #endregion

        private static string GetBasePath()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }
    }
}
