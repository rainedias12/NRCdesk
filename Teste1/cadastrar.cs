using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // Necessário para a comunicação com o SQL Server.
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography; // Necessário para usar SHA256 na criptografia da senha.

namespace Teste1
{
    // Classe do formulário de cadastro.
    public partial class cadastrar : Form
    {
        // Construtor do formulário.
        public cadastrar()
        {
            // Inicializa os componentes visuais.
            InitializeComponent();
            // Chama um método auxiliar para configurar as dicas de ferramentas (tooltips).
            ConfigurarToolTip();
        }

        // Instância de conexão com o banco de dados.
        // PRÁTICA RECOMENDADA: É melhor declarar e instanciar a conexão DENTRO do método 'registrar' e envolvê-la com 'using'
        // para garantir que ela seja fechada e liberada corretamente, mesmo em caso de erro.
        SqlConnection cn = new SqlConnection(@"Data Source=DESKTOP-2GBAOHH; Initial Catalog=DesktopNRCdesk;Integrated Security=True;");

        // ----------------------------------------------------
        // NOVO MÉTODO: Geração de Hash (Criptografia Unidirecional)
        // ----------------------------------------------------
        /// <summary>
        /// Gera o hash SHA256 de uma string (ideal para senhas).
        /// </summary>
        /// <param name="texto">A string (senha) a ser hasheada.</param>
        /// <returns>A string hasheada em formato hexadecimal.</returns>
        private string GerarHashSha256(string texto)
        {
            // O uso de 'using' garante que o objeto SHA256 (recurso) seja descartado corretamente.
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Converte a string de entrada para um array de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));

                // Converte o array de bytes para uma string hexadecimal
                StringBuilder builder = new StringBuilder();
                // Converte cada byte em sua representação hexadecimal de 2 dígitos.
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        // Manipulador de evento para o clique no botão "Registrar".
        private void BtnRegistrar_Click_1(object sender, EventArgs e)
        {
            // Chama o método principal de registro.
            // PRÁTICA RECOMENDADA: O método 'registrar' retorna um booleano. 
            // É ideal usar essa informação para fechar o formulário SOMENTE se o registro for 'true'.
            if (registrar())
            {
                this.Close();
            }

        }

        // Método principal para coletar, validar e inserir os dados do novo usuário.
        private bool registrar()
        {
            // 💡 1. VALIDAÇÃO E OBTENÇÃO DOS DADOS
            string nomeCompleto = TxtNomeCompleto.Text;
            string email = TxtEmail.Text;
            string senha = TxtSenha.Text;
            // ATENÇÃO: Confirmação de senha deve usar um campo dedicado (TxtConfirmarSenha, conforme assumido).
            string confirmaSenha = TxtConfirmarSenha.Text; // 👈 O nome do controle deve corresponder ao Designer.

            // Validação de campos obrigatórios (verifica se há campos vazios).
            if (string.IsNullOrWhiteSpace(nomeCompleto) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(confirmaSenha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validação da Senha (compara a senha digitada com a confirmação).
            if (senha != confirmaSenha)
            {
                MessageBox.Show("A Senha e a Confirmação de Senha não coincidem.", "Erro de Senha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // PRÁTICA RECOMENDADA: Adicionar verificação de formato de e-mail e complexidade de senha aqui.

            // 💡 2. CRIPTOGRAFIA (HASH) DA SENHA
            // Gera o hash SHA256 da senha em texto puro antes de enviá-lo ao banco.
            string senhaHasheada = GerarHashSha256(senha);

            try
            {
                // Abre a conexão com o banco de dados.
                cn.Open();

                // Query SQL para inserção de dados. Uso de parâmetros (@Nome_Completo, etc.) é EXCELENTE
                // para prevenir ataques de SQL Injection.
                string sqlQuery = "INSERT INTO Usuarios (Nome_Completo, Email, Senha, ID_Perfil, ID_Setor) " +
                 "VALUES (@Nome_Completo, @Email, @Senha, @ID_Perfil, @ID_Setor)";

                // Cria o comando SQL e garante o descarte com 'using'.
                using (SqlCommand cmd = new SqlCommand(sqlQuery, cn))
                {
                    // Adiciona os valores dos campos como parâmetros SQL.
                    cmd.Parameters.AddWithValue("@Nome_Completo", nomeCompleto);
                    cmd.Parameters.AddWithValue("@Email", email);

                    // 💡 SALVANDO A SENHA HASHEADA (Ponto de segurança crucial)
                    cmd.Parameters.AddWithValue("@Senha", senhaHasheada);

                    // ID_Perfil e ID_Setor são inseridos com valores fixos (3 e 1).
                    // PRÁTICA RECOMENDADA: Em vez de valores fixos, usar um ComboBox para o Setor ou
                    // uma constante/enum para o Perfil Padrão de novo cadastro.
                    cmd.Parameters.AddWithValue("@ID_Perfil", 3);
                    cmd.Parameters.AddWithValue("@ID_Setor", 1);

                    // Executa a consulta (INSERT).
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Dados salvos com sucesso", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true; // Retorna sucesso.
                } // Fim do 'using' do SqlCommand
            }
            catch (Exception ex)
            {
                // Tratamento de erro robusto.
                MessageBox.Show("Não foi possível inserir os dados!\n\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Retorna falha.
            }
            finally
            {
                // Bloco 'finally' garante que a conexão será fechada, independentemente de sucesso ou falha.
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }

        // Método auxiliar para configurar as dicas de ferramentas (tooltips) para os controles.
        private void ConfigurarToolTip()
        {
            toolTip1.SetToolTip(TxtNomeCompleto, "Digite seu Nome Completo");
            toolTip1.SetToolTip(TxtEmail, "Digite seu Email");
            toolTip1.SetToolTip(TxtSenha, "Digite uma senha forte");
            toolTip1.SetToolTip(TxtConfirmarSenha, "Confirmar senha");
        }

        // Manipulador de evento para a opção "Sair" do menu (ToolStripMenuItem).
        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Fecha o formulário atual de cadastro.
            this.Close();
        }
    }
}