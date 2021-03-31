using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CompartilhaPublicacaoGrupoFacebook.Util
{
    public class CSVUtil
    {
        public Queue<string> CarregaGrupos(string arquivo)
        {
            try
            {
                var grupos = new Queue<string>();
                using (StreamReader reader = new StreamReader(arquivo, Encoding.GetEncoding("iso-8859-1")))
                {
                    while (!reader.EndOfStream)
                    {
                        grupos.Enqueue(reader.ReadLine().Replace("\"", ""));
                    }
                    reader.Close();
                    reader.Dispose();
                }
                return grupos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void EscreverCSV<T>(List<T> lista, string caminho, bool acrescentar)
        {
            var csv = String.Join(
              ";",
              Array.ConvertAll(
                 lista.ToArray(),
                 element => element.ToString()
              )
            );

            using (StreamWriter writer = new StreamWriter(path: caminho, encoding: Encoding.UTF8, append: acrescentar))
            {
                foreach (var item in lista)
                {
                    var linhaArray = item.GetType()
                    .GetProperties()
                    .Select(p =>
                    {
                        object value = p.GetValue(item, null);
                        return value == null ? null : value.ToString();
                    })
                    .ToArray();

                    string linhaString = "";
                    foreach (var arrayItem in linhaArray)
                    {
                        linhaString += arrayItem + ";";
                    }

                    writer.WriteLine(linhaString);
                }

                writer.Close();
                writer.Dispose();
            }
        }
    }
}
