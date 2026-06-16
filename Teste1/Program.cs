using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms; // Necessário para acessar a classe Application e Forms.
using teste1; // Assumindo que este é o namespace onde está a classe 'entrar'.

namespace Teste1
{
    // A classe 'Program' é um contêiner estático que define o ponto de partida do aplicativo.
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        // [STAThread] é um atributo obrigatório para aplicações Windows Forms/WPF.
        // Ele garante que o thread principal seja um Single-Threaded Apartment, 
        // necessário para a maioria dos componentes UI (User Interface).
        [STAThread]
        static void Main()
        {
            // 1. Habilita os estilos visuais (temas) do sistema operacional, 
            // tornando os controles visuais (botões, caixas de texto) modernos.
            Application.EnableVisualStyles();

            // 2. Define o modo padrão de renderização de texto para ser compatível
            // com versões anteriores do .NET Framework.
            Application.SetCompatibleTextRenderingDefault(false);

            // 3. O coração do aplicativo: Inicia o loop de mensagens da aplicação 
            // e carrega a primeira instância do formulário ('entrar').
            // O aplicativo permanecerá em execução até que este formulário seja fechado.
            Application.Run(new entrar());
        }
    }
}