using System;
using System.Collections.Generic;
using System.Data; // Necessário para usar o DataTable
using System.Data.SqlClient; // Necessário para interagir com o SQL Server
using System.Linq; // Tipicamente usado para LINQ (não essencial neste trecho)
using System.Text; // Tipicamente usado para manipulação de strings (não essencial neste trecho)
using System.Threading.Tasks; // Tipicamente usado para operações assíncronas (não essencial neste trecho)
using static Teste1.entrar; // Importa estaticamente a classe 'entrar' para acessar 'UsuarioSessao' diretamente.

namespace Teste1
{
    // A classe externa (chamadoDataAccess) é, na verdade, apenas um contêiner (namespace-like)
    internal class chamadoDataAccess
    {
        // Esta classe é a Camada de Acesso a Dados (DAL - Data Access Layer).
        // Seu único propósito é se comunicar com o banco de dados.
        // Isso é um excelente exemplo de Encapsulamento e Separação de Preocupações (POO).
        public class ChamadosDataAccess
        {
            // Constante privada para a string de conexão do banco de dados.
            // 🚨 Alerta de Segurança: Em um ambiente de produção, esta string NUNCA deve ser hardcoded
            // no código-fonte. Deve ser lida de um arquivo de configuração (App.config/Web.config).
            private const string ConnectionString = @"Data Source=DESKTOP-2GBAOHH; Initial Catalog=DesktopNRCdesk;Integrated Security=True;";

            // Método público para obter a lista de chamados.
            // A lógica de qual dado retornar é definida pelo Perfil do usuário logado.
            public DataTable ObterChamadosPorPerfil()
            {
                string sqlQuery; // Variável para armazenar a consulta SQL dinâmica.
                int? idParam = null; // Variável opcional (nullable int) para o ID do usuário, se necessário no WHERE.

                // --- Lógica de Segurança Baseada em Perfil ---

                // Verifica o perfil de administrador.
                if (UsuarioSessao.IsAdmin)
                {
                    // Administrador: Permissão para visualizar TODOS os chamados.
                    sqlQuery = "SELECT * FROM Chamados";
                    // idParam permanece null, pois não há filtro WHERE.
                }
                // Verifica o perfil de técnico.
                else if (UsuarioSessao.IsTecnico)
                {
                    // Técnico: Vê apenas os chamados onde ele é o responsável.
                    sqlQuery = "SELECT * FROM Chamados WHERE ID_Tecnico_Responsavel = @UserId";
                    // Captura o ID do usuário logado para usar no filtro SQL.
                    idParam = UsuarioSessao.IdUsuarioLogado;
                }
                // Perfil padrão (Usuário Comum).
                else // Usuário Comum
                {
                    // Usuário Comum: Vê apenas os chamados que ele próprio solicitou.
                    sqlQuery = "SELECT * FROM Chamados WHERE ID_Usuario_Solicitante = @UserId";
                    // Captura o ID do usuário logado para usar no filtro SQL.
                    idParam = UsuarioSessao.IdUsuarioLogado;
                }

                // --- Execução Segura da Query ---

                // O 'using' garante que o objeto SqlConnection (que implementa IDisposable)
                // seja fechado e liberado corretamente, mesmo que ocorra uma exceção.
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    // O 'using' garante que o objeto SqlCommand seja liberado.
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, cn))
                    {
                        // Se 'idParam' tiver um valor (ou seja, se for Técnico ou Usuário Comum)...
                        if (idParam.HasValue)
                        {
                            // ⭐️ Boa Prática: Adiciona o parâmetro à query SQL.
                            // Isso é CRUCIAL para prevenir ataques de SQL Injection, pois o valor do ID
                            // é tratado como dado, e não como parte do comando SQL.
                            cmd.Parameters.AddWithValue("@UserId", idParam.Value);
                        }

                        // Abre a conexão com o banco de dados.
                        // Poderia ser movido para antes de 'da.Fill(dt)' se a conexão estivesse fechada.
                        // Mas 'SqlDataAdapter.Fill' é inteligente e abre/fecha a conexão se ela não estiver aberta.
                        // É mais comum abri-la explicitamente em DALs mais complexas, mas aqui funciona.

                        // O 'using' garante que o SqlDataAdapter seja liberado.
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // Cria um objeto DataTable em memória para receber os resultados da consulta.
                            DataTable dt = new DataTable();

                            // Preenche o DataTable com os dados retornados pelo SqlCommand.
                            da.Fill(dt);

                            // Retorna o DataTable preenchido.
                            return dt;
                        }
                    } // O SqlCommand é descartado aqui
                } // O SqlConnection é fechado e descartado aqui
            }
        }
    }
}
