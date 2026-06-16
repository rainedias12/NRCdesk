using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using teste1; // Assumindo que o namespace 'teste1' contém classes de apoio.
using Teste1; // Namespace principal do projeto.
using static Teste1.entrar; // Importa a classe estática 'UsuarioSessao' para acessar o ID e perfil do usuário logado.

namespace Teste1
{
    // Classe do formulário que exibe o status dos tickets (DataGrid).
    public partial class tabstatus : Form
    {
        // String de conexão com o banco de dados. PRÁTICA RECOMENDADA: Usar arquivos de configuração.
        private const string ConnectionString = @"Data Source=DESKTOP-2GBAOHH; Initial Catalog=DesktopNRCdesk;Integrated Security=True;";
        // Variável de conexão (embora a prática 'using' seja usada nos métodos, ela existe aqui).
        private SqlConnection cn;
        // Variáveis de controle de UI (possivelmente não usadas ou mal definidas no código fornecido).
        private object dgvTickets;
        private Form frmAtivo;
        private object panelForm;

        // Construtor do formulário.
        public tabstatus()
        {
            InitializeComponent(); // Inicializa os componentes do Windows Forms.
            // Chama o método para pré-configurar o DataGridView antes de carregar os dados.
            ConfigurarGrid();
        }

        // Método para exibir formulários filhos dentro de um painel (Parent-Child Form).
        private void FormShow(Form tabstatus)
        {
            // Fecha o formulário filho ativo anterior, se houver.
            if (frmAtivo != null)
                frmAtivo.Close();

            frmAtivo = tabstatus;
            tabstatus.TopLevel = false; // Define como formulário filho.
            // ATENÇÃO: A linha abaixo 'tabstatus.Controls.Add(tabstatus)' provavelmente é um erro de referência.
            // O correto seria adicionar o formulário ao container pai (e.g., panelForm.Controls.Add(tabstatus)).
            tabstatus.Controls.Add(tabstatus);
            tabstatus.BringToFront();
            tabstatus.Show();
        }



        // Evento disparado no carregamento inicial do formulário.
        private void tabstatus_Load(object sender, EventArgs e)
        {
            // Carrega os tickets usando o filtro padrão baseado no perfil do usuário logado (sem prioridade, idUsuarioMinimo=0).
            CarregarTickets();
        }

        /// <summary>
                /// Carrega e exibe os tickets no DataGridView, aplicando filtros opcionais.
                /// </summary>
        private void CarregarTickets(string prioridadeFiltro = null, int idUsuarioMinimo = 0) // idUsuarioMinimo é um filtro condicional.
        {
            try
            {
                // Usa a instrução 'using' para garantir que a conexão seja fechada automaticamente.
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();

                    // 1. Monta a query base, que inclui o filtro de perfil (Usuário Comum, Técnico, Admin).
                    string sqlQuery = MontarQueryPorPerfil(out bool isAdmin);

                    // 2. Adiciona o filtro condicional (se 'idUsuarioMinimo' for maior que zero).
                    if (idUsuarioMinimo > 0)
                    {
                        // Adiciona uma condição WHERE/AND para filtrar pelo ID do solicitante (>= 1, se não for 0).
                        sqlQuery += " AND ID_Usuario_Solicitante >= @IDUsuarioMinimo";
                    }

                    // 3. Adiciona filtro de prioridade, se o parâmetro 'prioridadeFiltro' for fornecido.
                    if (!string.IsNullOrWhiteSpace(prioridadeFiltro))
                    {
                        sqlQuery += " AND Prioridade = @Prioridade";
                    }

                    // 4. Adiciona ordenação dos resultados, garantindo tickets mais recentes no topo.
                    sqlQuery += " ORDER BY Data_Abertura DESC";

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, cn))
                    {
                        // 5. Define PARÂMETROS de perfil (necessários para as cláusulas WHERE de Técnico e Usuário Comum).
                        if (entrar.UsuarioSessao.NomePerfil == "Técnico")
                            cmd.Parameters.AddWithValue("@ID_Tecnico", entrar.UsuarioSessao.IdUsuario);
                        else if (entrar.UsuarioSessao.NomePerfil == "Usuário Comum")
                            cmd.Parameters.AddWithValue("@ID_Usuario", entrar.UsuarioSessao.IdUsuario);

                        // 6. Define o parâmetro ID_UsuarioMinimo, se for aplicado (condicional).
                        if (idUsuarioMinimo > 0)
                        {
                            cmd.Parameters.AddWithValue("@IDUsuarioMinimo", idUsuarioMinimo);
                        }

                        // 7. Define parâmetro de prioridade, se for aplicado (condicional).
                        if (!string.IsNullOrWhiteSpace(prioridadeFiltro))
                        {
                            cmd.Parameters.AddWithValue("@Prioridade", prioridadeFiltro);
                        }

                        // 8. Preenche o DataGridView usando SqlDataAdapter.
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt); // Preenche o DataTable com os resultados da consulta.
                            dataGridView3.DataSource = dt; // Define o DataTable como fonte de dados do DataGridView.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Tratamento de erro robusto em caso de falha na conexão ou consulta.
                MessageBox.Show("Erro ao carregar tickets: " + ex.Message);
            }
        }

        // ==========================================================
        // MÉTODO DE AUXÍLIO: MontarQueryPorPerfil
        // ==========================================================
        /// <summary>
        /// Constrói a string de consulta SQL base, aplicando a regra de visibilidade por perfil de usuário.
        /// </summary>
        private string MontarQueryPorPerfil(out bool isAdmin)
        {
            string perfil = entrar.UsuarioSessao.NomePerfil;
            isAdmin = false;

            // Query base de seleção de colunas relevantes.
            string baseSelect = "SELECT ID_Chamado, Titulo, Categoria, Prioridade, Status, Data_Abertura, " +
                "ID_Usuario_Solicitante, ID_Tecnico_Responsavel " +
                "FROM Chamados";

            switch (perfil)
            {
                case "Administrador":
                    isAdmin = true;
                    // O Admin vê TUDO. 'WHERE 1=1' é uma técnica para garantir que filtros adicionais (Prioridade, idUsuarioMinimo) 
                    // possam ser adicionados usando apenas 'AND'.
                    return baseSelect + " WHERE 1=1";

                case "Técnico":
                    // O Técnico vê apenas os tickets atribuídos a ele.
                    return baseSelect + " WHERE ID_Tecnico_Responsavel = @ID_Tecnico";

                case "Usuário Comum":
                    // O Usuário Comum vê apenas os tickets que ele mesmo abriu.
                    return baseSelect + " WHERE ID_Usuario_Solicitante = @ID_Usuario";

                default:
                    // Tratamento para perfis não mapeados, assume 'vê tudo' com filtro genérico.
                    return baseSelect + " WHERE 1=1";
            }
        }

        // ==========================================================
        // CONFIGURAÇÕES E FORMATAÇÕES DO GRID
        // ==========================================================
        // Método para definir o layout das colunas antes do carregamento dos dados.
        private void ConfigurarGrid()
        {
            // Desativa a geração automática de colunas para definir manualmente.
            dataGridView3.AutoGenerateColumns = false;
            dataGridView3.ReadOnly = true; // Impede a edição direta no grid.
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Seleciona a linha inteira.
            dataGridView3.AllowUserToAddRows = false; // Impede que o usuário adicione novas linhas.

            // Definição e mapeamento manual de colunas (DataPropertyName liga a coluna ao nome do campo no DataTable).
            dataGridView3.Columns.Add("ID_Chamado", "ID");
            dataGridView3.Columns["ID_Chamado"].DataPropertyName = "ID_Chamado";

            dataGridView3.Columns.Add("Titulo", "Título");
            dataGridView3.Columns["Titulo"].DataPropertyName = "Titulo";

            dataGridView3.Columns.Add("Categoria", "Categoria");
            dataGridView3.Columns["Categoria"].DataPropertyName = "Categoria";

            dataGridView3.Columns.Add("Prioridade", "Prioridade");
            dataGridView3.Columns["Prioridade"].DataPropertyName = "Prioridade";

            dataGridView3.Columns.Add("Status", "Status");
            dataGridView3.Columns["Status"].DataPropertyName = "Status";

            dataGridView3.Columns.Add("Data_Abertura", "Abertura");
            dataGridView3.Columns["Data_Abertura"].DataPropertyName = "Data_Abertura";

            // Configura colunas de IDs que serão mapeadas, mas não exibidas.
            dataGridView3.Columns.Add("ID_Usuario_Solicitante", "ID Solicitante");
            dataGridView3.Columns["ID_Usuario_Solicitante"].DataPropertyName = "ID_Usuario_Solicitante";
            dataGridView3.Columns["ID_Usuario_Solicitante"].Visible = false; // Oculta a coluna.

            dataGridView3.Columns.Add("ID_Tecnico_Responsavel", "ID Técnico");
            dataGridView3.Columns["ID_Tecnico_Responsavel"].DataPropertyName = "ID_Tecnico_Responsavel";
            dataGridView3.Columns["ID_Tecnico_Responsavel"].Visible = false; // Oculta a coluna.
        }

        // Método para formatar a aparência das colunas (tamanho, alinhamento, formato de data).
        private void FormatarColunasGrid()
        {
            // Verifica a existência da coluna e aplica a formatação.
            if (dataGridView3.Columns.Contains("ID_Chamado"))
            {
                dataGridView3.Columns["ID_Chamado"].Width = 50;
                dataGridView3.Columns["ID_Chamado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dataGridView3.Columns.Contains("Titulo"))
            {
                // Define a coluna Título para preencher o espaço restante.
                dataGridView3.Columns["Titulo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (dataGridView3.Columns.Contains("Data_Abertura"))
            {
                // Formata a exibição da data de abertura.
                dataGridView3.Columns["Data_Abertura"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }
        }


        // ==========================================================
        // EVENTO DE CLIQUE: BtnTodosOsTickets
        // ==========================================================
        private void BtnTodosOsTickets_Click_1(object sender, EventArgs e)
        {
            // O valor 0 garante que o filtro condicional idUsuarioMinimo no CarregarTickets não seja aplicado.
            int idUsuarioFiltro = 0;

            // Verifica o perfil para exibir uma mensagem informativa.
            if (entrar.UsuarioSessao.NomePerfil == "Usuário Comum")
            {
                // O filtro por ID do solicitante já é aplicado dentro de MontarQueryPorPerfil.
                MessageBox.Show($"Você está visualizando todos os seus {entrar.UsuarioSessao.NomePerfil} tickets.",
                "Filtro Padrão Aplicado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Mensagem para perfis Admin/Técnico.
                MessageBox.Show($"Você está visualizando os tickets atribuídos ao perfil {entrar.UsuarioSessao.NomePerfil}.",
                "Filtro Padrão Aplicado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Chama o carregamento sem filtro de prioridade e com idUsuarioMinimo=0, 
            // confiando na lógica de perfil para o filtro principal.
            CarregarTickets(idUsuarioMinimo: idUsuarioFiltro);
        }


        // ==========================================================
        // EVENTO DE CLIQUE: BtnCriarTicket (Criação de novo ticket)
        // ==========================================================
        private void BtnCriarTicket_Click(object sender, EventArgs e)
        {
            // Restringe a criação de tickets a Usuários Comuns e Administradores.
            if (entrar.UsuarioSessao.NomePerfil == "Usuário Comum" || entrar.UsuarioSessao.NomePerfil == "Administrador")
            {
                // Instancia o formulário de envio de ticket (novo, pois não passa ID).
                enviar_ticket novaJanela = new enviar_ticket();
                // Abre o formulário como modal.
                // Se o resultado for OK (sucesso ao salvar) ou Cancel (fechado), recarrega os dados.
                if (novaJanela.ShowDialog() == DialogResult.OK || novaJanela.DialogResult == DialogResult.Cancel)
                {
                    CarregarTickets(); // Recarrega o DataGridView para exibir o novo ticket.
                }
            }
            else
            {
                // Mensagem para Técnicos, que não podem criar novos tickets.
                MessageBox.Show("Apenas usuários comuns ou administradores podem criar tickets.",
          "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

       

        // ==========================================================
        // EVENTO DE CLIQUE: BtnBuscar (Filtro por prioridade)
        // ==========================================================
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            // Obtém o valor do TextBox de filtro (assumindo que TxtFiltrarPor existe).
            string prioridade = TxtFiltrarPor.Text;

            // Validação simples do campo de filtro.
            if (string.IsNullOrWhiteSpace(prioridade))
            {
                MessageBox.Show("Por favor, digite a prioridade para filtrar (Baixa, Média, Alta).", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Chama o método de carregamento, aplicando o filtro de prioridade sobre o filtro de perfil já existente.
            CarregarTickets(prioridade);
        }

        // ==========================================================
        // MÉTODOS DE MENU REDUNDANTES OU DE SAÍDA
        // ==========================================================

        private void criarTicketToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Chama o manipulador do botão, geralmente ligado a um item de menu "Criar Ticket".
            BtnCriarTicket_Click(sender, e);
        }

        

        private void criarTicketToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            // Manipulador de evento vazio, provavelmente duplicado ou não utilizado.
        }

        // ==========================================================
        // EVENTO DE CLIQUE: relatórioToolStripMenuItem (Menu)
        //==========================================================

        private void BtnRelatorio2_Click(object sender, EventArgs e)
        {

            // Permite acesso ao relatório apenas para Administradores.
            if (UsuarioSessao.NomePerfil == "Administrador")
            {
                // Assumindo que a classe 'relatorio' existe e está acessível.
                relatorio relatorioForm = new relatorio();
                relatorioForm.Show();
            }
            else
            {
                MessageBox.Show("Apenas administradores podem acessar os relatórios.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void BtnSair2_Click(object sender, EventArgs e)
        {
            this.Close(); // Fecha o formulário (Saída).
        }

        
    }
}
