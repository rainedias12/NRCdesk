using System; // Necessário para usar ArgumentException

namespace Teste1
{
    /// <summary>
    /// Classe estática (Singleton) responsável por armazenar os dados do usuário logado globalmente.
    /// Em POO, classes estáticas são usadas para dados ou funcionalidades que não requerem 
    /// múltiplas instâncias e têm estado global (como esta sessão).
    /// </summary>
    public static class SessaoUsuario
    {
        // Propriedade estática para o ID do usuário.
        // O setter é privado (private set), aplicando o pilar do Encapsulamento:
        // O valor só pode ser alterado por métodos internos da classe (IniciarSessao/EncerrarSessao).
        public static int IdUsuario { get; private set; } = 0; // 0 é o valor padrão que indica 'deslogado'.

        // Propriedades estáticas para armazenar informações do usuário.
        public static string NomeCompleto { get; private set; }
        public static string NomePerfil { get; private set; }

        /// <summary>
        /// Inicializa a sessão do usuário com os dados de login fornecidos.
        /// </summary>
        public static void IniciarSessao(int idUsuario, string nomeCompleto, string nomePerfil)
        {
            // Validação de entrada.
            if (idUsuario <= 0)
            {
                // Lança exceção se o ID for inválido, garantindo a integridade dos dados.
                throw new ArgumentException("O ID do usuário deve ser maior que zero.");
            }
            // Atribui os valores, alterando o estado da classe estática.
            IdUsuario = idUsuario;
            NomeCompleto = nomeCompleto;
            NomePerfil = nomePerfil;
        }

        /// <summary>
        /// Encerra a sessão do usuário, limpando todos os dados.
        /// </summary>
        public static void EncerrarSessao()
        {
            // Reseta todas as propriedades para seus valores iniciais/nulos.
            IdUsuario = 0;
            NomeCompleto = null;
            NomePerfil = null;
        }

        /// <summary>
        /// Propriedade de acesso rápido (somente leitura) que verifica o status de login.
        /// É um exemplo de propriedade calculada ou "expression body member".
        /// </summary>
        public static bool EstaLogado => IdUsuario > 0;
    }
}