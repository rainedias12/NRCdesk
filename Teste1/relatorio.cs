using grafico1; // Importa o namespace onde a classe RelatorioDados está definida.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq; // Essencial para métodos de extensão como .Count e .ToArray().
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Namespace crucial para usar o componente Chart.

namespace teste1
{
    // Classe do formulário de relatório que contém o gráfico.
    public partial class relatorio : Form
    {
        // Construtor do formulário.
        public relatorio()
        {
            InitializeComponent(); // Inicializa os componentes visuais do formulário.
        }


        // Manipulador de evento para o botão "Gerar".
        private void BtnGerar_Click_1(object sender, EventArgs e)
        {
            GerarGrafico(); // Chama o método para buscar dados e desenhar o gráfico.
        }

        // Método responsável por buscar os dados e configurar o componente Chart.
        private void GerarGrafico()
        {
            // 1. Cria a instância da classe que contém a lógica de acesso a dados (Model/Data Layer).
            var dados = new RelatorioDados();

            // 2. Chama o método de obtenção de dados para o ano de 2025.
            var lista = dados.VendasAnual(2025);

            // Verifica se a lista de dados retornada do banco não está vazia.
            if (lista.Count > 0)
            {
                // 3. Extrai os rótulos (nomes do status) e os valores (contagem) em arrays separados, 
                // para serem usados pelo método DataBindXY.
                var status = dados.GetNomeStatus(lista);
                var valores = dados.GetValoresStatus(lista);

                // Limpa todos os elementos anteriores do gráfico antes de redesenhá-lo.
                chart1.Series.Clear();
                chart1.Titles.Clear();
                chart1.Legends.Clear();

                // Configuração do Título Principal.
                var titulo = new Title
                {
                    Font = new Font("Arial", 16, FontStyle.Bold),
                    // Define o texto do título, usando o ano do primeiro resultado da lista.
                    Text = "Chamados por Status - Ano " + lista[0].Ano
                };
                chart1.Titles.Add(titulo);

                // Configuração da Legenda.
                chart1.Legends.Add(new Legend("Legenda"));
                chart1.Legends[0].Title = "Status";

                // Configuração da Série de Dados (Gráfico de Pizza).
                chart1.Series.Add("Chamados"); // Adiciona uma nova série.
                chart1.Series[0].ChartType = SeriesChartType.Pie; // Define o tipo de gráfico como Pizza.
                chart1.Series[0].Points.DataBindXY(status, valores); // Vincula os rótulos (status) e os valores.
                chart1.Series[0].IsValueShownAsLabel = true; // Exibe o valor de cada fatia como um rótulo.

                // Configuração do Estilo 3D.
                chart1.ChartAreas[0].Area3DStyle.Enable3D = true; // Habilita o efeito 3D.
                chart1.ChartAreas[0].Area3DStyle.Inclination = 30; // Define a inclinação da área 3D.
                chart1.ChartAreas[0].Area3DStyle.Rotation = 60; // Define a rotação da área 3D.

                // Configuração da Paleta de Cores.
                chart1.Palette = ChartColorPalette.SemiTransparent;
            }
            else
            {
                // Executado se a consulta não retornar dados.
                chart1.Series.Clear(); // Limpa as séries existentes.
                chart1.Titles.Clear(); // Limpa títulos anteriores.
                // Adiciona uma mensagem de erro ao título do gráfico.
                chart1.Titles.Add("Nenhum dado encontrado para o ano especificado.");
            }
        }

        // Manipulador de evento disparado quando o formulário é carregado (atualmente vazio).
        private void relatorio_Load(object sender, EventArgs e)
        {

        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        
    }
    }
}