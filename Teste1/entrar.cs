using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // Necessário para interagir com o SQL Server (conexão, comandos, leitura).
using System.Security.Cryptography; // Necessário para usar a classe SHA256 e gerar o hash criptográfico.

namespace Teste1
{
    // Classe do formulário de login (janela "entrar").
    public partial class entrar : Form
    {
        // Classe estática (global) para armazenar os dados do usuário após o login.
        // O uso de "static" permite acesso de qualquer lugar da aplicação.
        public static class UsuarioSessao
        {
            // Propriedade de leitura (private set) para o ID do usuário logado.
            public static int IdUsuario { get; private set; }
            // Nome do perfil (Admin, Técnico, etc.).
            public static string NomePerfil { get; private set; }
            // Flags de permissão de acesso. O 'internal set' restringe a alteração ao assembly atual.
            public static bool IsAdmin { get; internal set; }
            public static bool IsTecnico { get; internal set; }
            // ID do usuário logado (pode ser redundante com IdUsuario, mas mantido). O '?' indica que é anulável (nullable).
            public static int? IdUsuarioLogado { get; internal set; }

            // Método para configurar as informações essenciais do usuário após o login bem-sucedido.
            public static void DefinirUsuarioLogado(int idUsuario, string nomePerfil)
            {
                IdUsuario = idUsuario;
                NomePerfil = nomePerfil;
                // Nota: As flags IsAdmin/IsTecnico precisariam ser definidas aqui ou em outra chamada.
            }

            // Método para resetar as variáveis de sessão (usado no logout).
            public static void LimparSessao()
            {
                IdUsuario = 0;
                NomePerfil = null;
            }
        }

        // Constante que armazena a string de conexão com o banco de dados SQL Server.
        // ATENÇÃO: Contém detalhes do servidor e é vulnerável se o aplicativo não estiver protegido.
        private const string ConnectionString = @"Data Source=DESKTOP-2GBAOHH; Initial Catalog=DesktopNRCdesk;Integrated Security=True;";

        // Construtor do formulário de entrada.
        public entrar()
        {
            // Inicializa os componentes visuais do Windows Forms (gerado automaticamente).
            InitializeComponent();
            // Chama um método auxiliar para configurar as dicas de ferramentas (tooltips).
            ConfigurarToolTip();
            // Define o foco inicial do cursor no campo de e-mail.
            TxtEmail.Select();
        }

        // ----------------------------------------------------
        // MÉTODO DE HASH (Deve ser idêntico em todo o projeto)
        // ----------------------------------------------------
        // Função para converter uma string (senha em texto puro) em seu valor hash SHA256.
        private string GerarHashSha256(string texto)
        {
            // Cria uma instância de SHA256. O 'using' garante que o recurso seja liberado após o uso.
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Converte a string de entrada para um array de bytes e calcula o hash.
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                // Objeto otimizado para construir a string final do hash.
                StringBuilder builder = new StringBuilder();
                // Itera sobre cada byte do hash e o converte para uma representação hexadecimal de 2 dígitos ("x2").
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                // Retorna o hash final, que terá 64 caracteres.
                return builder.ToString();
            }
        }

        // ----------------------------------------------------
        // ROTINA CRÍTICA DE ATUALIZAÇÃO DO BANCO DE DADOS (NOVO)
        // ----------------------------------------------------
        // Método para converter todas as senhas armazenadas em texto simples para o formato hash SHA256.
        // Esta rotina DEVE ser removida após sua execução única no ambiente de produção.
        private void AtualizarTodasAsSenhasParaHash()
        {
            // Query para selecionar IDs e Senhas onde o campo "Senha" tem menos de 64 caracteres (indicando texto puro).
            string querySelect = "SELECT ID_Usuario, Senha FROM Usuarios WHERE LEN(Senha) < 64";
            // Query de UPDATE para gravar o novo hash, usando parâmetros de segurança.
            string queryUpdate = "UPDATE Usuarios SET Senha = @NovaSenhaHash WHERE ID_Usuario = @IdUsuario";

            // Cria a conexão com o banco. O 'using' garante o fechamento da conexão.
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                try
                {
                    conexao.Open();
                    // Estrutura de tupla (id, senha) para armazenar os dados lidos antes de realizar o update.
                    List<(int id, string senhaAntiga)> usuariosParaAtualizar = new List<(int, string)>();

                    // 1. LER AS SENHAS EM TEXTO PURO
                    // Cria o comando SELECT.
                    using (SqlCommand cmdSelect = new SqlCommand(querySelect, conexao))
                    {
                        // Abre o DataReader para ler os resultados.
                        using (SqlDataReader reader = cmdSelect.ExecuteReader())
                        {
                            // Itera sobre os registros encontrados.
                            while (reader.Read())
                            {
                                // Lê o valor da senha e do ID e adiciona à lista temporária.
                                string senha = reader.GetString(reader.GetOrdinal("Senha"));
                                usuariosParaAtualizar.Add((
                                  reader.GetInt32(reader.GetOrdinal("ID_Usuario")),
                                  senha
                                ));
                            }
                        } // Fecha DataReader
                    } // Fecha SqlCommand (Select)

                    // Verifica se há senhas para atualizar. Se não houver, exibe mensagem e sai.
                    if (usuariosParaAtualizar.Count == 0)
                    {
                        MessageBox.Show("Nenhuma senha em texto puro encontrada. O banco já está atualizado.", "Processo Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // 2. GERAR O HASH E ATUALIZAR CADA SENHA
                    // Itera sobre a lista de usuários com senhas antigas (texto puro).
                    foreach (var usuario in usuariosParaAtualizar)
                    {
                        // Gera o hash SHA256 da senha antiga.
                        string novaSenhaHash = GerarHashSha256(usuario.senhaAntiga);

                        // Cria o comando UPDATE para cada usuário.
                        using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conexao))
                        {
                            // Adiciona o hash gerado como parâmetro.
                            cmdUpdate.Parameters.AddWithValue("@NovaSenhaHash", novaSenhaHash);
                            // Adiciona o ID do usuário para a cláusula WHERE.
                            cmdUpdate.Parameters.AddWithValue("@IdUsuario", usuario.id);
                            // Executa a atualização no banco de dados.
                            cmdUpdate.ExecuteNonQuery();
                        } // Fecha SqlCommand (Update)
                    } // Fim do loop foreach

                    // Exibe mensagem de sucesso após a conclusão de todos os updates.
                    MessageBox.Show($"Sucesso! {usuariosParaAtualizar.Count} senhas foram convertidas para SHA256. Você deve remover a chamada desta rotina agora!", "Atualização Crítica Concluída", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (Exception ex)
                {
                    // Tratamento de erro robusto para falhas de conexão ou SQL.
                    MessageBox.Show("Erro durante a atualização do banco de dados:\n" + ex.Message, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } // Fecha SqlConnection
        }

        // ----------------------------------------------------
        // EVENTOS
        // ----------------------------------------------------

        // Manipulador de evento disparado quando o formulário é carregado (aberto).
        private void entrar_Load(object sender, EventArgs e)
        {
            // Garante que o foco permaneça no campo de e-mail ao carregar.
            TxtEmail.Select();

            // 🚨 PASSO CRÍTICO: CHAME O MÉTODO DE ATUALIZAÇÃO AQUI 🚨
            // EXECUTE ISTO APENAS UMA VEZ e COMENTE/REMOVA DEPOIS!
            // AtualizarTodasAsSenhasParaHash(); // Esta linha é o PONTO DE CHAMADA que deve ser removido.
        }

        // Método auxiliar para configurar as dicas de ferramentas (tooltips) para os controles.
        private void ConfigurarToolTip()
        {
            toolTip1.SetToolTip(TxtEmail, "Email do Usuário");
            toolTip1.SetToolTip(TxtSenha, "Senha do Usuário");
            toolTip1.SetToolTip(BtnEntrar, "Clique aqui para entrar");
            toolTip1.SetToolTip(LblCriarConta, "Clique aqui para Criar uma Conta");
        }

        // Manipulador de evento para o link/botão "Criar Conta".
        private void LblCriarConta_Click_1(object sender, EventArgs e)
        {
            // Cria uma nova instância do formulário de cadastro.
            cadastrar entrarMain = new cadastrar();
            // Exibe o formulário de cadastro.
            entrarMain.Show();
        }

        // Manipulador de evento para o botão "Entrar". Contém a lógica de autenticação.
        private void BtnEntrar_Click(object sender, EventArgs e)
        {
            // 💡 1. LOGIN COM HASH (JÁ ESTAVA CORRETO)
            string senhaDigitada = TxtSenha.Text;
            // Validação inicial de campos vazios.
            if (string.IsNullOrWhiteSpace(TxtEmail.Text) || string.IsNullOrWhiteSpace(senhaDigitada))
            {
                MessageBox.Show("Preencha Email e Senha.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gera o hash da senha digitada pelo usuário.
            string senhaHasheada = GerarHashSha256(senhaDigitada);


            // Query SQL parametrizada para buscar o ID e Perfil do usuário.
            // Compara o email e o HASH da senha (segurança).
            string query = @"
                SELECT 
                    U.ID_Usuario, 
                    P.Nome_Perfil 
                FROM Usuarios U
                JOIN PerfilAcesso P ON U.ID_Perfil = P.ID_Perfil
                WHERE U.Email = @Email AND U.Senha = @Senha";

            try
            {
                // Cria a conexão e garante o descarte.
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Open();

                    // Cria o comando SQL e garante o descarte.
                    using (SqlCommand cmd = new SqlCommand(query, conexao))
                    {
                        // Adiciona o parâmetro @Email (com trim para remover espaços).
                        cmd.Parameters.AddWithValue("@Email", TxtEmail.Text.Trim());
                        // 💡 2. ENVIA O HASH PARA A COMPARAÇÃO NO BANCO DE DADOS
                        cmd.Parameters.AddWithValue("@Senha", senhaHasheada);

                        // Executa a consulta e obtém um leitor de dados.
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Se o leitor encontrar uma linha, o login foi bem-sucedido.
                            if (reader.Read())
                            {
                                // Obtém os dados de ID e Perfil.
                                int idUsuario = reader.GetInt32(reader.GetOrdinal("ID_Usuario"));
                                string nomePerfil = reader.GetString(reader.GetOrdinal("Nome_Perfil"));

                                // Define a sessão do usuário.
                                UsuarioSessao.DefinirUsuarioLogado(idUsuario, nomePerfil);

                                MessageBox.Show($"Acesso Liberado! Perfil: {nomePerfil}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Abre o formulário principal após o login.
                                tabstatus entrarMain = new tabstatus();
                                entrarMain.Show();
                                // Esconde o formulário de login.
                                this.Hide();
                            }
                            else
                            {
                                // Login falhou (email/senha incorretos ou usuário não encontrado).
                                MessageBox.Show("Email/Senha Incorreta!", "Erro de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        } // Fecha SqlDataReader
                    } // Fecha SqlCommand
                } // Fecha SqlConnection
            }
            catch (Exception ex)
            {
                // Tratamento de erro geral para problemas de conexão ou SQL.
                MessageBox.Show("Erro no banco de dados: " + ex.Message, "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}