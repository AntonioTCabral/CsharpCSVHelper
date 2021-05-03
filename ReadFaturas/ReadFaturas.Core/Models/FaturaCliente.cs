using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadFaturas.Core.Models
{
    public class FaturaCliente
    {
        public string NomeCliente { get; set; }
        public string EnderecoCompleto { get; set; }
        public string ValorFatura { get; set; }
        public int NumeroPaginas { get; set; }

        public string[] GetText(string url)
        {
            var textFile = File.ReadAllLines(url);

            return textFile;
        }

        public void WriteAllText(string strFilePath, string sbOutput)
        {
            File.WriteAllText(strFilePath, sbOutput);
        }

        public async Task<bool> WriteFile(List<FaturaCliente> faturas, string url)
        {
            try
            {
                await using var stw = new StreamWriter(url);
                await using var csv = new CsvWriter(stw, CultureInfo.InvariantCulture);

                await csv.WriteRecordsAsync(faturas);

                await csv.FlushAsync();
                await stw.FlushAsync();

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool ValidaCep(string cep)
        {
            if (cep.Length == 8 && cep != "00000000")
            {
                cep = cep.Substring(0, 5) + "-" + cep.Substring(5, 3);
            }
            return Regex.IsMatch(cep, ("[0-9]{5}-[0-9]{3}"));
        }
    }


}
