using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

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
        // Lista de todos os perfis contidos no placar
        private List<Player> _jogadores;
        // Variavel usada para habilitar os registros no placar
        private bool _registrar;
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
        public List<Player> Jogadores
        {
            get { return this._jogadores; }
        }
        public bool Registrar
        {
            get { return this._registrar; }
            set { this._registrar = value; }
        }
        public ScoreManager(Player P, string FileName)
        {
            this.Jogador = P;
            if (FileName != null)
            {
                _jogadores = new List<Player>();
                Registrar = true;
                this._filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                // Combinando o caminho do roaming com uma pasta SDM, ficaria assim: C:/Users/(Nome do usuário)/%appdata%/roaming/SDM
                string aux = Path.Combine(FilePath, "SDM");
                // Se o diretório acima não existir, crie um novo
                if (!Directory.Exists(aux))
                    Directory.CreateDirectory(aux);
                // Prepara o arquivo: C:/Users/(Nome do usuário)/%appdata%/roaming/SDM/(Nome do arquivo).ssdm para ser lido ou criado"
                this._filePath = aux + "/" + FileName + ".ssdm";
                this.CreateScoreFile();
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
        /// <summary>
        /// Cria o arquivo de placares inicial caso o mesmo não exista
        /// </summary>
        /// <param name="FilePath"></param>
        private void CreateScoreFile()
        {
            // Se o arquivo não existe, cria-lo, caso contrário, leia-o em busca de perfis
            if(!File.Exists(FilePath))
            {
                using(StreamWriter st = File.CreateText(FilePath))
                {
                    st.WriteLine("[Placares]");
                }
            }
            else
            {
                string Dado = "";
                using(StreamReader sr = new StreamReader(File.OpenRead(FilePath)))
                {
                    Dado = sr.ReadLine();
                    while(Dado != null)
                    {
                        Dado = sr.ReadLine();
                        if(Dado != null && !Dado.Equals("[Placares]"))
                        {
                            string[] Split = Dado.Split('=');
                            try
                            {
                                Jogadores.Add(new Player(Split[0], Double.Parse(Split[1])));
                            }catch(FormatException e)
                            {
                                if((MessageBox.Show("Foram encontrados alguns dados corrompidos no placar! Deseja chamar o Liminha para reparar o placar? Se escolher \"não\" o placar não irá funcionar!")) == DialogResult.Yes)
                                {
                                    RepararArquivoDePlacar();
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show("Placar desativado pelo motivo de haver dados corrompidos!");
                                    Registrar = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Tenta recuperar o arquivo de placar, reparando ou deletando dados corrompidos
        /// </summary>
        private void RepararArquivoDePlacar()
        {
            if (!File.Exists(FilePath))
            {
                var Dado = "";
                var Pontos = 0.0;
                var DadosCorrompidos = 0;
                // Limpa a lista e começa do zero
                Jogadores.Clear();
                // Lê todos os dados do arquivo
                using (StreamReader sr = new StreamReader(File.OpenRead(FilePath)))
                {
                    Dado = sr.ReadLine();
                    var DadoCorrompido = false;
                    while (Dado != null)
                    {
                        // Tenta checar por pontuações corrompidas
                        try
                        {
                            Pontos = Double.Parse(Dado.Split('=')[1]);
                        }
                        catch (FormatException e)
                        {
                            // Tenta corrigir pontuações corrompidas
                            try
                            {
                                Pontos = RemoveLetras(Dado.Split('=')[1]);
                            }catch(FormatException e2 )
                            {
                                // Caso não dê para corrigir, trata-o como dado corrompido
                                DadoCorrompido = true;
                                DadosCorrompidos++;
                            }
                        }
                        // Se o dado não estiver mais corrompido adiciona-o a lista
                        if(!DadoCorrompido)
                        {
                            Jogadores.Add(new Player(Dado.Split('=')[0], Pontos));
                        }
                    }
                }
                // Recria o arquivo de placar
                using (StreamWriter st = File.AppendText(FilePath))
                {
                    st.WriteLine("[Placares]");
                    // Testa se há algum dado salvo
                    if(Jogadores.Count > 0)
                    {
                        // Reescreve os dados salvos
                        foreach(var Jogador in Jogadores)
                        {
                            st.WriteLine(Jogador.Nome + "=" + Jogador.Pontuacao);
                        }
                        MessageBox.Show("Placar recuperado com êxito! Dados Perdidos: " + DadosCorrompidos + "!", "Placar Recuperado!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Erro exibido quando todos os dados foram perdidos
                        MessageBox.Show("Falha ao recuperar os dados! Todos os dados estavam corrompidos, portanto o arquivo de placar foi resetado!", "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        /// <summary>
        /// Tenta remover letras de um número
        /// </summary>
        /// <returns>O número limpo, sem letras</returns>
        public double RemoveLetras(string Numero)
        {
            var Caracteres = new string[] { "A", "B", "C", "D", "E", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "Ç", "~", "´", "-", "_", "!", "?", "`", "{", "[", "}", "]", "(", ")", ":", ";", "$", "*", "'", "\"", "&", "#" };
            var NumeroNovo = Numero.ToLower();
            foreach(string Caractere in Caracteres)
            {
                NumeroNovo = NumeroNovo.Replace(Caractere.ToLower(), "");
            }
            return double.Parse(NumeroNovo);
        }
        /// <summary>
        /// Salva a pontuação do jogador no arquivo de placar
        /// </summary>
        public void SalvarPontuacao()
        {
            if (Registrar)
            {
                string Aux = File.ReadAllText(FilePath);
                File.WriteAllText(FilePath, Aux);
                using (StreamWriter st = File.AppendText(FilePath))
                {
                    st.WriteLine(Jogador.Nome + "=" + Jogador.Pontuacao);
                }
                Jogadores.Add(new Player(Jogador.Nome, Jogador.Pontuacao));
            }
        }
        public double AdicionarPontos(double Pontos)
        {
            if (Registrar)
            {
                if (!TemJogador())
                {
                    throw new NullReferenceException("Sem jogador adicionado");
                }
                if (Pontos > 0)
                {
                    Jogador.Pontuacao += Pontos;
                    return Jogador.Pontuacao;
                }
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
        /// Retorna um placar ordenado no formato de lista
        /// </summary>
        /// <returns></returns>
        public List<Player> GetPlacarOrdenado()
        {
            if(!File.Exists(FilePath))
            {
                throw new NullReferenceException("Nenhum arquivo de placar encontrado");
            }
            return this.OrdenarPlacar();
        }
        private List<Player> OrdenarPlacar()
        {
            return (Jogadores.Count > 1) ? Jogadores.OrderByDescending(o => o.Pontuacao).ToList() : Jogadores;
        }
        [Obsolete("Não necessário mais")]
        /// <summary>
        /// Remove os números bugados do placar
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public string RemoveNumeros(string Text)
        {
            return Text.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").Replace(".", ""); ;
        }
        [Obsolete("Contém bugs que não podem ser corrigidos")]
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
            List.Sort((value1, value2) => value2.Value.CompareTo(value1.Value));
            // Converte de volta em um dicionario
            dictionary = List.ToDictionary(pair => pair.Key, pair => pair.Value);
            return dictionary;
        }
    }
}
