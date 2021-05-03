using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadFaturas.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ReadFaturas.Web.Controllers
{
    [Route("api/Read")]
    [ApiController]
    public class FaturaController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> ExportCsvHelper(string fileUrl)
        {
            List<FaturaCliente> faturas = new List<FaturaCliente>();
            string file = @fileUrl;

            FaturaCliente cliente = new FaturaCliente();

            var textFile = cliente.GetText(file);

            var teste = textFile.ToList();
            teste.RemoveAt(0);


            foreach (var item in teste)
            {
                var dados = item.Split(";");

                if (cliente.ValidaCep(dados[1].Replace(" ", "")))
                {

                    int numPag;
                    var tryConvert = Int32.TryParse(dados[7], out numPag);

                    if (numPag % 2 == 1)
                    {
                        numPag += 1;
                    }

                    FaturaCliente clienteaux = new FaturaCliente
                    {
                        NomeCliente = dados[0],
                        EnderecoCompleto = dados[2] + '-' + dados[3] + '-' + dados[4] + '-' + dados[5] + '-' + dados[1],
                        ValorFatura = dados[6],
                        NumeroPaginas = numPag
                    };

                    faturas.Add(clienteaux);
                }

            }
            string strFilePath6 = ".\\arquivos\\faturaspg6.csv";
            string strFilePath12 = ".\\arquivos\\faturaspg12.csv";
            string strFilePath = ".\\arquivos\\faturaspg.csv";
            string strFilePathZero = ".\\arquivos\\faturasZero.csv";

            string strSeperator = ";";

            var faturasPg6 = faturas.AsQueryable().Where(x => x.NumeroPaginas <= 6 && x.ValorFatura != "0").ToList();
            var faturasPg12 = faturas.AsQueryable().Where(x => x.NumeroPaginas > 6 && x.NumeroPaginas <= 12 && x.ValorFatura != "0").ToList();
            var faturasPg = faturas.AsQueryable().Where(x => x.NumeroPaginas > 12 && x.ValorFatura != "0").ToList();
            var faturazero = faturas.AsQueryable().Where(x =>  x.ValorFatura == "0").ToList();

            await cliente.WriteFile(faturasPg6, strFilePath6);
            await cliente.WriteFile(faturasPg12, strFilePath12);
            await cliente.WriteFile(faturasPg, strFilePath);
            await cliente.WriteFile(faturazero, strFilePathZero);


            return Ok("Arquivos gerados");
        }

        
    }
}
