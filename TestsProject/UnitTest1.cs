using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDMClasses;
using System.IO;

namespace TestsProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestaSeNomeDoJogadorEhSalvo()
        {
            ScoreManager score = new ScoreManager(new Player("Guest"),"test");
            Assert.AreEqual("Guest", score.Jogador.Nome);
        }
        [TestMethod]
        public void TestaSeRemoveTodosOsNumeros()
        {
            ScoreManager score = new ScoreManager(new Player("Guest"), "test");
            string Test = "1234567890.remove tudo";
            Assert.AreEqual("remove tudo", score.RemoveNumeros(Test));
        }
        [TestMethod]
        public void TestaSePontuacaoEhDada()
        {
            ScoreManager score = new ScoreManager(new Player("Guest"), "test");
            score.AdicionarPontos(100);
            Assert.AreEqual(100, score.Jogador.Pontuacao);
        }
        [TestMethod]
        public void TestaSeHaArquivoDeSave()
        {
            ScoreManager score = new ScoreManager(new Player("Guest"), "test");
            Assert.IsTrue(File.Exists(score.FilePath));
        }
    }
}
