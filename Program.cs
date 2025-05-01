using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ElPaisScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando scraper para El País...");

            // Configurar opciones de Chrome
            var chromeOptions = new ChromeOptions();
            // La siguiente línea ejecuta Google Chrome en modo headless (sin interfaz visual)
            chromeOptions.AddArgument("--headless");

            try
            {
                // Iniciar el driver de Chrome
                using (var driver = new ChromeDriver(chromeOptions))
                {
                    // Navegar a la página de El País
                    Console.WriteLine("Navegando a elpais.com...");
                    driver.Navigate().GoToUrl("https://elpais.com");

                    // Esperar a que la página cargue completamente
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    // Buscar y hacer clic en el botón "Suscríbete"
                    Console.WriteLine("Buscando el botón 'Suscríbete'...");

                    // Nota: El selector exacto puede variar, así que incluyo algunas alternativas comunes
                    IWebElement suscribeButton = null;

                    try
                    {
                        // Intentar diferentes selectores posibles para el botón
                        suscribeButton = wait.Until(d => {
                            try { return d.FindElement(By.XPath("//a[contains(text(), 'Suscríbete') or contains(text(), 'Suscribete')]")); }
                            catch { return null; }
                        });

                        if (suscribeButton == null)
                        {
                            suscribeButton = wait.Until(d => {
                                try { return d.FindElement(By.CssSelector("a.subscribe-button, a.suscribete-button, a[href*='suscribete']")); }
                                catch { return null; }
                            });
                        }

                        if (suscribeButton != null)
                        {
                            Console.WriteLine("Haciendo clic en el botón 'Suscríbete'...");
                            suscribeButton.Click();

                            // Dar tiempo para que la página se cargue después de hacer clic
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Console.WriteLine("No se encontró el botón 'Suscríbete'. Continuando con la página actual.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al buscar el botón de suscripción: {ex.Message}");
                        Console.WriteLine("Continuando con la página actual.");
                    }

                    // Obtener el primer tag <a> de la página
                    Console.WriteLine("Obteniendo el primer tag <a> de la página...");
                    IWebElement firstATag = wait.Until(d => d.FindElement(By.TagName("a")));

                    // Obtener el HTML del primer tag <a>
                    string firstATagHtml = firstATag.GetAttribute("outerHTML");
                    Console.WriteLine("HTML del primer tag <a> obtenido:");
                    Console.WriteLine(firstATagHtml);

                    // Guardar el HTML en un archivo de texto
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "elpaisTag.txt");
                    File.WriteAllText(filePath, firstATagHtml);
                    Console.WriteLine($"HTML guardado en: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}