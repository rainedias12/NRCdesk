using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient; // Necessário para a conexão com o SQL Server
using System.Linq; // Essencial para o uso das funções LINQ (Select, ToArray)

namespace grafico1
{
    // Esta classe atua como um DTO (Data Transfer Object) ou Modelo.
    // Ela define a estrutura de dados que será usada para relatórios ou gráficos.
    // Isso é um pilar da Abstração e Encapsulamento em POO.
    internal class RelatorioDados
    {
        // Propriedades (Getters e Setters) da classe DTO.
        // Representam uma linha de dados no relatório.
        public string Status { get; set; } // Representa o status do chamado (eixo X do gráfico)
        public double Valor { get; set; } // Representa a contagem de chamados (eixo Y do gráfico)
        public short Ano { get; set; } // Ano de referência do relatório

        // Construtor padrão (vazio).
        public RelatorioDados() { }

        // Construtor sobrecarregado (Polimorfismo).
        // Usado para instanciar e preencher o objeto DTO diretamente com os dados lidos do banco.
        public RelatorioDados(short ano, string status, decimal valor)
        {
            Ano = ano;
            Status = status;
            // Converte o valor de 'decimal' (do banco) para 'double' (da propriedade).
            Valor = (double)valor;
        }

        // Método de Acesso a Dados (DAL - Data Access Logic) para obter dados do banco.
        // Ele reside dentro do DTO, o que é comum em modelos pequenos, mas idealmente seria em uma DAL separada.
        public List<RelatorioDados> VendasAnual(short ano)
        {
            // Inicializa a lista que armazenará os objetos DTO preenchidos.
            var lista = new List<RelatorioDados>();

            // String de conexão hardcoded. Boa prática seria movê-la para um arquivo de configuração.
            string conexaoString = @"Data Source=DESKTOP-2GBAOHH;Initial Catalog=DesktopNRCdesk;Integrated Security=True;";

            // Usa o bloco 'using' para garantir que o objeto SqlConnection seja fechado e liberado.
            using (SqlConnection conexao = new SqlConnection(conexaoString))
            {
                // Comando SQL para agregar o total de chamados por Status em um ano específico.
                string sql = @"
                SELECT 
                    Status, 
                    COUNT(ID_Chamado) AS TotalChamados -- Agrega a contagem de chamados
                FROM 
                    dbo.Chamados
                WHERE 
                    YEAR(Data_Abertura) = @Ano -- Filtra pelo ano fornecido como parâmetro
                GROUP BY 
                    Status"; // Agrupa para consolidar o total por cada status

                // Cria o objeto SqlCommand para executar a query.
                SqlCommand cmd = new SqlCommand(sql, conexao);
                // Adiciona o parâmetro para evitar SQL Injection (Boa Prática de segurança).
                cmd.Parameters.AddWithValue("@Ano", ano);

                // Abre explicitamente a conexão com o banco de dados.
                conexao.Open();
                // Executa a consulta e obtém um SqlDataReader para ler os resultados linha por linha.
                SqlDataReader dr = cmd.ExecuteReader();

                // Loop para ler cada linha (registro) retornado pelo banco de dados.
                while (dr.Read())
                {
                    // Instancia um novo objeto RelatorioDados usando o construtor sobrecarregado
                    // e adiciona-o à lista.
                    lista.Add(new RelatorioDados(
                        ano, // Passa o ano de filtro
                        dr["Status"].ToString(), // Converte o status para string
                        Convert.ToDecimal(dr["TotalChamados"]) // Converte a contagem para Decimal
                    ));
                }
                // Fecha o leitor de dados, liberando os recursos.
                dr.Close();
            } // A conexão é fechada automaticamente aqui pelo bloco 'using'.

            // Retorna a lista de DTOs preenchidos.
            return lista;
        }

        // 💡 CORREÇÃO 1: Tipo de retorno alterado para string[]
        // Usa LINQ (Language Integrated Query) para extrair e projetar dados.
        internal string[] GetNomeStatus(List<RelatorioDados> lista)
        {
            // O .Select(x => x.Status) projeta (extrai) apenas a propriedade Status de cada objeto na lista.
            // O .ToArray() converte a coleção resultante em um array de strings.
            return lista.Select(x => x.Status).ToArray();
        }

        // 💡 CORREÇÃO 2: Tipo de retorno alterado para double[]
        // Usa LINQ para extrair e projetar os valores.
        internal double[] GetValoresStatus(List<RelatorioDados> lista)
        {
            // O .Select(x => x.Valor) projeta (extrai) apenas a propriedade Valor de cada objeto na lista.
            // O .ToArray() converte a coleção resultante em um array de doubles.
            return lista.Select(x => x.Valor).ToArray();
        }
    }
}