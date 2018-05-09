namespace SDMClasses
{
    public class Player
    {
        private string _nome;
        // Versão atualizada da pontuação
        private double _pontuacao;
        public string Nome
        {
            get { return this._nome; }
            set { this._nome = value; }
        }
        public double Pontuacao
        {
            get { return this._pontuacao; }
            set { this._pontuacao = value; }
        }
        public Player(string Nome)
        {
            this.Nome = Nome;
            Pontuacao = 0.0;
        }
        public Player(string Nome, double PontuacaoAnterior)
        {
            this.Nome = Nome;
            Pontuacao = 0.0;
        }
    }
}
