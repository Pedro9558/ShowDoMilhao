using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace SDMClasses
{
    public class ScoreManager
    {
        // Caminho para o arquivo: C:/Users/(Nome do usuário)/%appdata%/Roaming
        private string _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        // Instancia do jogador
        private Player _jogador;
        // Caminho do arquivo dos placares
        private string _filePath;
        // Getters e setters
        public string AppDataPath
        {
            get { return this._appDataPath; }
        }
        public Player Jogador
        {
            get { return this._jogador; }
            set { this._jogador = value; }
        }
        public string FilePath
        {
            get { return this._filePath; }
        }
        public ScoreManager(Player P, string FileName)
        {
            this.Jogador = P;
            this._filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Combinando o caminho do roaming com uma pasta SDM, ficaria assim: C:/Users/(Nome do usuário)/%appdata%/roaming/SDM
            string aux = Path.Combine(FilePath, "SDM");
            // Se o diretório acima não existir, crie um novo
            if (!Directory.Exists(aux))
                Directory.CreateDirectory(aux);
            // Prepara o arquivo: C:/Users/(Nome do usuário)/%appdata%/roaming/SDM/(Nome do arquivo).ssdm para ser lido ou criado"
            this._filePath = aux + "/"+ FileName + ".ssdm";
            this.CreateScoreFile();
        }
        /// <summary>
        /// Cria o arquivo de placares inicial caso o mesmo não exista
        /// </summary>
        /// <param name="FilePath"></param>
        private void CreateScoreFile()
        {
            if(!File.Exists(FilePath))
            {
                using(StreamWriter st = File.CreateText(FilePath))
                {
                    st.WriteLine("[Placares]");
                }
            }
        }
        /// <summary>
        /// Salva a pontuação do jogador no arquivo de placar
        /// </summary>
        public void SalvarPontuacao()
        {
            string Aux = File.ReadAllText(FilePath);
            File.WriteAllText(FilePath, Aux);
            using (StreamWriter st = File.AppendText(FilePath))
            {
                st.WriteLine(Jogador.Nome + "=" + Jogador.Pontuacao);
            }
        }
        public double AdicionarPontos(double Pontos)
        {
            if(!TemJogador())
            {
                throw new NullReferenceException("Sem jogador adicionado");
            }
            if(Pontos > 0)
            {
                Jogador.Pontuacao += Pontos;
                return Jogador.Pontuacao;
            }
            return 0;
        }
        private bool TemJogador()
        {
            return this.Jogador != null;
        }
        [Obsolete("Usar metódo GetPlacarOrdenado")]
        public string GetAllScores()
        {
            if(!File.Exists(FilePath))
            {
                throw new NullReferenceException("Nenhum arquivo de placar encontrado");
            }
            return File.ReadAllText(FilePath);
        }
        /// <summary>
        /// Retorna um vetor ordenado do placar
        /// </summary>
        /// <returns></returns>
        public string GetPlacarOrdenado()
        {
            if(!File.Exists(FilePath))
            {
                throw new NullReferenceException("Nenhum arquivo de placar encontrado");
            }
            string[] Linhas = File.ReadAllLines(FilePath);
            // Cria um construtor de strings para construir a string
            StringBuilder PlacarOrdenado = new StringBuilder();
            PlacarOrdenado.Append("[Placar]").Append("\n");
            // Lê todos os registros do placar ordenado e adiciona a string
            foreach (KeyValuePair<string, string> dict in OrdenarPlacar(Linhas))
            {
                PlacarOrdenado.Append(dict.Key).Append(" = ").Append(dict.Value).Append("\n");
            }
            return PlacarOrdenado.ToString();
        }
        /// <summary>
        /// Retorna um dicionario ordenado do placar, em ordem decrescente
        /// </summary>
        /// <param name="placar"></param>
        /// <returns></returns>
        private Dictionary<string, string> OrdenarPlacar(string[] placar)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            int Contador = 1;
            // Pega todo conteudo do placar e adiciona no dicionario
            foreach(string Line in placar)
            {
                // Menos essa linha
                if(!Line.Equals("[Placares]", StringComparison.OrdinalIgnoreCase))
                {
                    string[] Temp = Line.Split('=');
                    dictionary.Add(Contador+"."+Temp[0], Temp[1].Replace(" ", ""));
                    Contador++;
                }
            }
            // Converte o dicionario em uma lista
            var List = dictionary.ToList();
            // Aranja a lista para ficar em ordem decrescente
            List.Sort((value1, value2) => value1.Value.CompareTo(value2.Value));
            // Converte de volta em um dicionario
            dictionary = List.ToDictionary(pair => pair.Key, pair => pair.Value);
            return dictionary;
        }
    }
}
