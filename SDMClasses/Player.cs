using System;
namespace SDMClasses
{

    public class Player
    {
        private string _nome;
        // Versão atualizada da pontuação
        private double _pontuacao;
        // Versão anterior da pontuação
        private double _pontuacaoAnterior;
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
        public double PontuacaoAnterior
        {
            get { return this._pontuacaoAnterior; }
            set { this._pontuacaoAnterior = value; }
        }
        public Player(string Nome)
        {
            this.Nome = Nome;
            Pontuacao = 0.0;
            PontuacaoAnterior = 0.0;
        }
        public Player(string Nome, double PontuacaoAnterior)
        {
            this.Nome = Nome;
            Pontuacao = 0.0;
            this.PontuacaoAnterior = PontuacaoAnterior;
        }
    }
}
