using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq; // Necessário para usar métodos de extensão, como .Cast<object>().ToArray()
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Teste1.entrar; // Importa a classe estática UsuarioSessao do formulário 'entrar'.

// O using 'teste1' é redundante se o namespace já é Teste1, mas mantido se for uma referência externa.
// using teste1; 

namespace Teste1
{
    // Formulário para Enviar ou Editar um Ticket/Chamado.
    public partial class enviar_ticket : Form
    {
        private int id; // Variável para armazenar o ID do ticket sendo editado (0 se for novo).
        // Instância de conexão com o banco de dados.
        // PRÁTICA RECOMENDADA: É melhor usar 'using' para a conexão DENTRO dos métodos de DB para garantir o fechamento.
        private SqlConnection cn = new SqlConnection(
      @"Data Source=DESKTOP-2GBAOHH; Initial Catalog=DesktopNRCdesk;Integrated Security=True;");

        // --- VARIÁVEL NECESSÁRIA PARA IDENTIFICAR QUEM ABRE O CHAMADO ---
        // A variável de sessão 'UsuarioSessao.IdUsuario' será usada para obter o ID.
        // private const int ID_USUARIO_LOGADO = 1;


        // Construtor padrão (usado para CRIAR um novo ticket).
        public enviar_ticket()  // <-- construtor sem parâmetro
        {
            InitializeComponent();
            this.id = 0; // novo ticket
            // Chama métodos para popular os ComboBoxes.
            ConfigurarComboBoxSetor();
            ConfigurarComboBoxPrioridade();
            // Chama um método auxiliar para configurar as dicas de ferramentas (tooltips).
            ConfigurarToolTip();
        }

        // Construtor alternativo (usado para EDITAR um ticket existente).
        public enviar_ticket(int id)
        {
            InitializeComponent();
            ConfigurarComboBoxSetor();
            ConfigurarComboBoxPrioridade();
            this.id = id;
            // Se um ID válido for fornecido, busca os dados do ticket para preencher o formulário.
            if (id > 0)
            {
                GetTicket(id);
            }
        }

        /// <summary>
        /// Popula o ComboBox TxtNome (Setor/Categoria) com as opções de departamentos.
        /// </summary>
        private void ConfigurarComboBoxSetor()
        {
            // Array de strings com as opções de Categoria/Setor.
            string[] setores = new string[]
      {
        "Tecnologia da Informação",
        "Recursos Humanos",
        "Financeiro",
        "Marketing",
        "Comercial"
      };

            // Assumindo que TxtNome é o ComboBox para o Setor.
            TxtNome.Items.Clear();
            // Adiciona os itens ao ComboBox (Cast necessário pois AddRange espera um array de objetos).
            TxtNome.Items.AddRange(setores.Cast<object>().ToArray());

            // Uso de 'is ComboBox cbx' para garantir que o controle seja um ComboBox e definir propriedades.
            if (TxtNome is ComboBox cbx)
            {
                // Impede que o usuário digite no ComboBox, obrigando a seleção de uma opção válida.
                cbx.DropDownStyle = ComboBoxStyle.DropDownList;
                if (cbx.Items.Count > 0)
                {
                    cbx.SelectedIndex = 0; // Seleciona o primeiro item como padrão.
                }
            }
        }

        /// <summary>
        /// Popula o ComboBox CbxPrioridade com as opções de prioridade.
        /// </summary>
        private void ConfigurarComboBoxPrioridade()
        {
            // Array de strings com as opções de Prioridade.
            string[] prioridades = new string[]
      {
        "Baixa",
        "Média",
        "Alta"
      };

            // Popula o ComboBox de Prioridade.
            CbxPrioridade.Items.Clear();
            CbxPrioridade.Items.AddRange(prioridades.Cast<object>().ToArray());

            if (CbxPrioridade is ComboBox cbx)
            {
                cbx.DropDownStyle = ComboBoxStyle.DropDownList;
                if (cbx.Items.Count > 0)
                {
                    // Define 'Média' (índice 1) como padrão.
                    cbx.SelectedIndex = 1;
                }
            }
        }


        // Método para buscar e carregar os dados de um ticket existente (para edição).
        private void GetTicket(int id)
        {
            try
            {
                cn.Open();
                // Query para buscar todas as colunas de um chamado específico usando o ID_Chamado.
                string sql = "SELECT * FROM Chamados WHERE ID_Chamado = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    // Uso de parâmetro para segurança (evitar SQL Injection).
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) // Se um ticket for encontrado.
                        {
                            // Preenche os campos do formulário com os dados do ticket.
                            TxtAssunto.Text = dr["Titulo"].ToString();
                            TxtDescricao.Text = dr["Descricao"].ToString();

                            // Seleciona o item correto nos ComboBoxes usando o valor do banco.
                            CbxPrioridade.Text = dr["Prioridade"].ToString();
                            TxtNome.Text = dr["Categoria"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Tratamento de erro ao buscar dados.
                MessageBox.Show("Erro ao buscar o ticket:\n\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Garante que a conexão seja fechada.
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }

        /// <summary>
        /// Cria (INSERT) ou atualiza (UPDATE) um ticket no banco de dados.
        /// Retorna true em caso de sucesso ou false em caso de falha.
        /// </summary>
        private bool criarTicket()
        {
            // Validação de campos obrigatórios. Verifica Assunto, Descrição e se o Setor foi selecionado.
            if (string.IsNullOrWhiteSpace(TxtAssunto.Text) || string.IsNullOrWhiteSpace(TxtDescricao.Text) || TxtNome.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, preencha o Assunto, a Descrição e selecione um Setor.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // 💡 OBTENDO O ID DO USUÁRIO LOGADO AQUI
                // Usa a variável de sessão estática definida no formulário 'entrar' para obter o ID do usuário.
                int idUsuarioLogado = UsuarioSessao.IdUsuario;

                cn.Open();
                string sql;
                string categoriaSelecionada = TxtNome.Text;
                string prioridadeSelecionada = CbxPrioridade.Text;

                // Lógica de decisão: 0 = Novo Ticket (INSERT), > 0 = Edição (UPDATE).
                if (this.id == 0)
                {
                    // Comando INSERT: O campo ID_Usuario_Solicitante recebe o ID do usuário logado.
                    sql = "INSERT INTO Chamados (ID_Usuario_Solicitante, Titulo, Descricao, Prioridade, Categoria, Status) " +
             "VALUES (@ID_Usuario_Solicitante, @Titulo, @Descricao, @Prioridade, @Categoria, @Status)";
                }
                else
                {
                    // Comando UPDATE: Atualiza os campos editáveis. Não atualiza ID_Usuario_Solicitante.
                    sql = "UPDATE Chamados SET " +
             "Titulo = @Titulo, " +
             "Descricao = @Descricao, " +
             "Prioridade = @Prioridade, " +
             "Categoria = @Categoria " +
             "WHERE ID_Chamado = @Id";
                }

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    // Adiciona os parâmetros comuns a INSERT e UPDATE.
                    cmd.Parameters.AddWithValue("@Titulo", TxtAssunto.Text);
                    cmd.Parameters.AddWithValue("@Descricao", TxtDescricao.Text);
                    cmd.Parameters.AddWithValue("@Prioridade", prioridadeSelecionada);
                    cmd.Parameters.AddWithValue("@Categoria", categoriaSelecionada);

                    // Parâmetros específicos para INSERÇÃO.
                    if (this.id == 0)
                    {
                        // Define o solicitante com o ID da sessão.
                        cmd.Parameters.AddWithValue("@ID_Usuario_Solicitante", idUsuarioLogado);
                        // Define o status inicial como "Aberto".
                        cmd.Parameters.AddWithValue("@Status", "Aberto");
                    }

                    // Parâmetro específico para ATUALIZAÇÃO (cláusula WHERE).
                    if (this.id > 0)
                        cmd.Parameters.AddWithValue("@Id", this.id);

                    // Executa a consulta (INSERT ou UPDATE).
                    cmd.ExecuteNonQuery();

                    // Exibe mensagem de sucesso com base na ação realizada.
                    string acao = (this.id == 0) ? "criado" : "atualizado";
                    MessageBox.Show($"Chamado {acao} com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Tratamento de erro robusto.
                MessageBox.Show("Erro ao salvar o chamado:\n\n" + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Garante que a conexão seja fechada.
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }
        // Método auxiliar para configurar as dicas de ferramentas (tooltips) para os controles.
        private void ConfigurarToolTip()
        {
            toolTip1.SetToolTip(TxtNome, "Adicione o Setor");
            toolTip1.SetToolTip(TxtAssunto, "Escreva o Assunto do Chamado");
            toolTip1.SetToolTip(TxtDescricao, "Descreva brevemente o assunto do Chamado");
        }

        // Manipulador de evento para o botão Enviar/Salvar.
        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            // Chama 'criarTicket' e fecha o formulário se a operação for bem-sucedida.
            if (criarTicket())
            {
                this.Close();
            }
        }

        // Manipulador de evento para o botão Cancelar.
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close(); // Fecha o formulário sem salvar.
        }

        // Manipuladores de eventos de menu (itens comentados, mas mantidos).
        private void todosOsTicketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Lógica para abrir a tela de todos os tickets (tabstatus).
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); // Fecha o formulário.
        }

        private void criarTicketToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Lógica para abrir a tela de relatório.
        }
    }
}
