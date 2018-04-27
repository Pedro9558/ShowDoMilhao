using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDMClasses
{
    public class Pergunta
    {
        private int _id;
        private string _questao;
        private string _resposta;
        private int _indexCorreto;
        private string[] _alternativas;
        public string Questao
        {
            get { return this._questao; }
        }
        public int ID
        {
            get { return this._id; }
        }
        public string Resposta
        {
            get { return this._resposta; }
            set { this._resposta = value; }
        }
        public string[] Alternativas
        {
            get { return this._alternativas; }
        }
        public int IndexCorreto
        {
            get { return this._indexCorreto; }
        }
        public Pergunta(string Q, string[] Alt, int IndexDaCorreta)
        {
            this._id++;
            this._alternativas = Alt;
            this._questao = Q;
            this.Resposta = Alternativas[IndexDaCorreta];
            this._indexCorreto = IndexDaCorreta;
        }
        public bool CheckAnswer(string R)
        {
            return R.Contains(Resposta);
        }
    }
}
