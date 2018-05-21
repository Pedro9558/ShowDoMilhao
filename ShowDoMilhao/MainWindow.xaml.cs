using System;
using System.Windows;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using SDMClasses;

namespace ShowDoMilhao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Vetor de perguntas
        private Pergunta[] _perguntas = new Pergunta[120];
        // Vetor com as perguntas selecionadas
        private Pergunta[] _perguntasSelecionadas = new Pergunta[10];
        private Random R = new Random();
        // Temporaziadores para criação de ações de segundo plano
        private Timer T = new Timer();
        private Timer TTwo = new Timer();
        private Timer TThree = new Timer();
        // Stream relativa às falas do Silvio Santos
        private Stream SilvioSantos = Properties.Resources.sdm_intro;
        // Referência ao arquivo da música do show do milhão
        private Stream Musica = Properties.Resources.sdm;
        // Reprodutor de audio
        private SoundPlayer Player;
        private int _threadDelay;
        // Garante que o usuário não responda mais de uma vez
        private bool _respondeu;
        // Variavel Auxiliar para a animações
        private int _auxForAnimation;

        // Criador do jogo
        private string _criador;
        // Variavel reponsável pela ação "Aperte qualquer tecla continuar"
        private bool _canPressAnyKey;
        // Guarda a alternativa a escolha
        private byte _alternativaEscolhida;
        private ScoreManager _placar;
        // Jogador
        private Player _guest;
        // Número da pergunta
        private int _numeroDaPergunta = 1;
        // Getters e setters
        public int ThreadDelay
        {
            get { return this._threadDelay; }
            set { this._threadDelay = value; }
        }
        public string Criador
        {
            get { return "Pedro9558"; }
        }
        public int AuxForAnimation
        {
            get { return this._auxForAnimation; }
            set { this._auxForAnimation = value; }
        }
        public bool CanPressAnyKey
        {
            get { return this._canPressAnyKey; }
            set { this._canPressAnyKey = value; }
        }
        public bool Respondeu
        {
            get { return this._respondeu; }
            set { this._respondeu = value; }
        }
        public ScoreManager Placar
        {
            get { return this._placar; }
            set { this._placar = value; }
        }
        public Pergunta[] Perguntas
        {
            get { return this._perguntas; }
        }
        public Pergunta[] PerguntasSelecionadas
        {
            get { return this._perguntasSelecionadas; }
        }
        public Player Guest
        {
            get { return this._guest; }
            set { this._guest = value; }
        }
        public int NumeroDaPergunta
        {
            get { return this._numeroDaPergunta; }
        }
        public byte AlternativaEscolha
        {
            get { return this._alternativaEscolhida; }
            set { this._alternativaEscolhida = value; }
        }
        public MainWindow()
        {
            T.Tick += new EventHandler(AnimacaoInicial);
            this.ThreadDelay = 10;
            T.Interval = ThreadDelay;
            InitializeComponent();
            Player = new SoundPlayer(SilvioSantos);
            T.InitializeLifetimeService();
            T.Start();
            this.Copyright.Content = "Criado por "+Criador+". Vídeo, Audios e Imagens by SBT Copyright © 2018 - Sistema Brasileiro de Televisão";
            TThree.Tick += new EventHandler(AnimacaoPergunta);
            TThree.Interval = 8000;
            TTwo.Interval = 2000;
            TTwo.Tick += new EventHandler(EncerraJogo);
        }

        /// <summary>
        /// Faz a animação inicial da tela
        /// <param name="e"></param>
        /// <param name="sender"></param>
        /// </summary>
        private void AnimacaoInicial(object sender, EventArgs e)
        {
            switch(AuxForAnimation)
            {
                case 0:
                    // Altera a opacidade do texto inicial aos poucos até ele ficar visivel
                    if (Introducao.Opacity < 1)
                    {
                        Introducao.Opacity = Introducao.Opacity + 0.01;
                    }
                    else
                    {
                        T.Interval = 3000;
                        AuxForAnimation++;
                    }
                    break;
                case 1:
                    // Altera a opacidade do texto inicial aos poucos até ele ficar invisivel
                    if (Introducao.Opacity > 0)
                    {
                        T.Interval = T.Interval != ThreadDelay ? ThreadDelay : T.Interval; 
                        Introducao.Opacity = Introducao.Opacity - 0.01;
                    }
                    else
                    {
                        T.Interval = 1500;
                        AuxForAnimation++;
                    }
                    break;
                case 2:
                    // Reproduz vídeo intro
                    introVideo.Visibility = Visibility.Visible;
                    introVideo.Play();
                    CanPressAnyKey = true;
                    T.Interval = 41600;
                    AuxForAnimation++;
                    break;
                case 3:
                    // Termina Reprodução do vídeo
                    introVideo.Visibility = Visibility.Hidden;
                    introVideo.Stop();
                    CanPressAnyKey = false;
                    AuxForAnimation++;
                    Player.Play();
                    T.Interval = 3000;
                    break;
                case 4:
                    // Toca música inicial de fundo
                    AuxForAnimation++;
                    Player.Stream = Musica;
                    Player.PlayLooping();
                    break;
                case 5:
                    // Exibe a logo do show do milhão
                    T.Interval = ThreadDelay;
                    if(SDMLogo.Opacity < 1)
                    {
                        SDMLogo.Opacity = SDMLogo.Opacity + 0.01;
                    }
                    else
                    {
                        T.Interval = 3000;
                        AuxForAnimation++;
                    }
                    break;
                case 6:
                    // Animação de pedir para o usuário apertar qualquer tecla
                    T.Interval = 300;
                    CanPressAnyKey = true;
                    Titulo.Visibility = (Titulo.Visibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden;
                    break;
                case 7:
                    // Executado após o usuário apertar a tecla, logo desaparece e é perguntado o nome do usuário
                    T.Interval = ThreadDelay;
                    CanPressAnyKey = false;
                    Titulo.Visibility = Visibility.Hidden;
                    if (SDMLogo.Opacity > 0)
                    {
                        SDMLogo.Opacity = SDMLogo.Opacity - 0.01;
                    }else
                    {
                        Titulo.Content = "Digite o seu nome abaixo:";
                        Titulo.Margin = new Thickness(106, 186, 0, 0);
                        AuxForAnimation++;
                    }
                    break;
                case 8:
                    // Mostra o menu de colocar o nome do participante
                    Titulo.Visibility = Visibility.Visible;
                    OKButton.Visibility = Visibility.Visible;
                    InputNome.Visibility = Visibility.Visible;
                    break;
                case 9:
                    // Constroi o menu inicial do jogo
                    OKButton.Visibility = Visibility.Hidden;
                    InputNome.Visibility = Visibility.Hidden;
                    MiniLogo.Visibility = Visibility.Visible;
                    BIniciar.Visibility = Visibility.Visible;
                    BPlacar.Visibility = Visibility.Visible;
                    BSair.Visibility = Visibility.Visible;
                    SilvioImage.Visibility = Visibility.Visible;
                    // Cria o perfil do Jogador, caso não exista
                    if(Guest == null)
                    {
                        Guest = new Player(InputNome.Text);
                        Titulo.Content = "Bem-vindo(a) " + Guest.Nome + "!";
                    }
                    else
                    {
                        Titulo.Visibility = Visibility.Visible;
                        Titulo.Content = "Mais uma, " + Guest.Nome + "?"; 
                    }
                    Titulo.Margin = new Thickness(152, 67, 0, 0);
                    this.Width = 625.0;
                    this.Height = 450.0;
                    // Cria placar para o jogador caso não tenha sido criado
                    if (Placar == null)
                    {
                        Placar = new ScoreManager(Guest, "SDMPlacar");
                    }
                    this.AtualizarPlacar();
                    AuxForAnimation++;
                    break;
            }
        }
        /// <summary>
        /// Handler caso alguma tecla do teclado seja apertada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Testa se o usuário já pode apertar qualquer tecla
            if(CanPressAnyKey)
            {
                switch (AuxForAnimation)
                {
                    case 3:
                        T.Interval = 1;
                        break;
                    case 6:
                        AuxForAnimation++;
                        break;
                }
            }
        }
        /// <summary>
        /// Uma pequena animação do mostrador de perguntas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimacaoPergunta(object sender, EventArgs e)
        {
            // A animação ocorre conforme a visibilidade do indicador do número da pergunta
            if(IndicadorPergunta.Visibility == Visibility.Hidden)
            {
                // Esconde tudo da pergunta anterior
                Pergunta.Visibility = Visibility.Hidden;
                AlternativaA.Visibility = Visibility.Hidden;
                AlternativaB.Visibility = Visibility.Hidden;
                AlternativaC.Visibility = Visibility.Hidden;
                AlternativaD.Visibility = Visibility.Hidden;
                Pontos.Visibility = Visibility.Hidden;
                ImagemPergunta.Visibility = Visibility.Hidden;
                IndicadorPergunta.Content = "Pergunta " + NumeroDaPergunta;
                IndicadorPergunta.Margin = new Thickness(189, 161, 0, 0);
                IndicadorPergunta.Visibility = Visibility.Visible;
                // Executa um audio personalizado dependendo do número da pergunta
                switch(NumeroDaPergunta)
                {
                    case 1:
                        TThree.Interval = 8000;
                        SilvioSantos = Properties.Resources.sdm_primeira_pergunta;
                        break;
                    case 2:
                        TThree.Interval = 5000;
                        SilvioSantos = Properties.Resources.sdm_segunda_pergunta;
                        break;
                    case 3:
                        TThree.Interval = 5000;
                        SilvioSantos = Properties.Resources.sdm_terceira_pergunta;
                        break;
                    case 4:
                        TThree.Interval = 5000;
                        SilvioSantos = Properties.Resources.sdm_quarta_pergunta;
                        break;
                    case 5:
                        TThree.Interval = 6000;
                        SilvioSantos = Properties.Resources.sdm_quinta_pergunta;
                        break;
                    case 6:
                        TThree.Interval = 9000;
                        SilvioSantos = Properties.Resources.sdm_sexta_pergunta;
                        break;
                    case 7:
                        TThree.Interval = 6000;
                        SilvioSantos = Properties.Resources.sdm_setima_pergunta;
                        break;
                    case 8:
                        TThree.Interval = 5000;
                        SilvioSantos = Properties.Resources.sdm_oitava_pergunta;
                        break;
                    case 9:
                        TThree.Interval = 5000;
                        SilvioSantos = Properties.Resources.sdm_nona_pergunta;
                        break;
                    case 10:
                        TThree.Interval = 7000;
                        SilvioSantos = Properties.Resources.sdm_ultima_pergunta;
                        break;
                }
                Player.Stream = SilvioSantos;
                Player.Play();
            }
            else
            {
                // Constroi a pergunta e exibe-a para o jogador
                Respondeu = false;
                this.ConstroiPergunta();
                IndicadorPergunta.Visibility = Visibility.Hidden;
                Pergunta.Visibility = Visibility.Visible;
                AlternativaA.Visibility = Visibility.Visible;
                AlternativaB.Visibility = Visibility.Visible;
                AlternativaC.Visibility = Visibility.Visible;
                AlternativaD.Visibility = Visibility.Visible;
                Pontos.Visibility = Visibility.Visible;
                TThree.Stop();
            }
        }
        /// <summary>
        /// Checa o nome do usuário para ver se é um nome válido
        /// Handler do botão "Pronto!"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckName(object sender, RoutedEventArgs e)
        {
            if (InputNome.Text == null || InputNome.Text.Equals(""))
            {
                System.Windows.MessageBox.Show("Por favor, coloque um nome!", "Erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                System.Windows.MessageBox.Show("Bem-vindo(a) " + InputNome.Text+"!");
                AuxForAnimation++;
            }
        }
        /// <summary>
        /// Constroi a proxima pergunta
        /// </summary>
        private void ConstroiPergunta()
        {
            AlternativaA.Background = new SolidColorBrush(Colors.Transparent);
            AlternativaB.Background = new SolidColorBrush(Colors.Transparent);
            AlternativaC.Background = new SolidColorBrush(Colors.Transparent);
            AlternativaD.Background = new SolidColorBrush(Colors.Transparent);
            Pontos.Content = "Placar: " + Guest.Pontuacao;
            this.Width = 750.0;
            // Dependendo de qual questão está, a tela mostra a questão conforme o número
            switch (NumeroDaPergunta)
            {
                case 1:
                    Pergunta.Content = "1." + PerguntasSelecionadas[0].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[0].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[0].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[0].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[0].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if(PerguntasSelecionadas[0].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[0].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 2:
                    Pergunta.Content = "2." + PerguntasSelecionadas[1].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[1].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[1].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[1].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[1].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[1].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[1].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 3:
                    Pergunta.Content = "3." + PerguntasSelecionadas[2].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[2].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[2].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[2].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[2].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[2].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[2].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 4:
                    Pergunta.Content = "4." + PerguntasSelecionadas[3].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[3].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[3].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[3].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[3].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[3].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[3].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 5:
                    Pergunta.Content = "5." + PerguntasSelecionadas[4].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[4].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[4].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[4].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[4].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[4].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[4].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 6:
                    Pergunta.Content = "6." + PerguntasSelecionadas[5].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[5].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[5].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[5].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[5].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[5].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[5].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 7:
                    Pergunta.Content = "7." + PerguntasSelecionadas[6].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[6].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[6].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[6].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[6].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[6].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[6].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 8:
                    Pergunta.Content = "8." + PerguntasSelecionadas[7].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[7].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[7].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[7].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[7].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[7].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[7].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 9:
                    Pergunta.Content = "9." + PerguntasSelecionadas[8].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[8].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[8].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[8].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[8].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[8].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[8].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
                case 10:
                    Pergunta.Content = "10." + PerguntasSelecionadas[9].Questao;
                    AlternativaA.Content = "A)" + PerguntasSelecionadas[9].Alternativas[0];
                    AlternativaB.Content = "B)" + PerguntasSelecionadas[9].Alternativas[1];
                    AlternativaC.Content = "C)" + PerguntasSelecionadas[9].Alternativas[2];
                    AlternativaD.Content = "D)" + PerguntasSelecionadas[9].Alternativas[3];
                    // Testa se há uma imagem na questão
                    if (PerguntasSelecionadas[9].LocalizacaoDaImagemDaPergunta != null)
                    {
                        ImagemPergunta.Visibility = Visibility.Visible;
                        ImagemPergunta.Source = new BitmapImage(PerguntasSelecionadas[9].LocalizacaoDaImagemDaPergunta);
                    }
                    break;
            }
        }
        /// <summary>
        /// Checa a resposta que o usuário deu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckAnswer(object sender, MouseButtonEventArgs e)
        {
            var Temp = 0;
            if (!Respondeu)
            {
                Temp = R.Next(1, 4);
                System.Windows.Controls.Label label = sender as System.Windows.Controls.Label;
                label.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                switch(Temp)
                {
                    case 1:
                        SilvioSantos = Properties.Resources.sdm_confirmar;
                        break;
                    case 2:
                        SilvioSantos = Properties.Resources.sdm_confirmar2;
                        break;
                    case 3:
                        SilvioSantos = Properties.Resources.sdm_confirmar3;
                        break;
                }
                Player.Stream = SilvioSantos;
                Player.Play();
                // Pede uma confirmação do usuário se ele quer escolher essa resposta
                if ((System.Windows.MessageBox.Show("Você tem certeza de sua resposta?", "Silvio Santos", MessageBoxButton.YesNo)) == MessageBoxResult.Yes)
                {
                    Temp = R.Next(1, 3);
                    Respondeu = true;
                    // Testa se o usuário acertou a pergunta
                    if (PerguntasSelecionadas[NumeroDaPergunta - 1].CheckAnswer(label.Content.ToString()))
                    {
                        switch(Temp)
                        {
                            case 1:
                                SilvioSantos = Properties.Resources.sdm_acertou;
                                break;
                            case 2:
                                SilvioSantos = Properties.Resources.sdm_acertou2;
                                break;
                        }
                        Player.Stream = SilvioSantos;
                        Player.Play();
                        label.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF0FB0C"));
                        label.Background = new SolidColorBrush(Colors.Green);
                        // Determina quantos pontos serão dados dependendo do número da pergunta
                        switch(NumeroDaPergunta)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                Placar.AdicionarPontos(1000);
                                break;
                            case 6:
                                Placar.AdicionarPontos(5000);
                                break;
                            case 7:
                            case 8:
                                Placar.AdicionarPontos(10000);
                                break;
                            case 9:
                                Placar.AdicionarPontos(20000);
                                break;
                            case 10:
                                Placar.AdicionarPontos(50000);
                                break;
                        }
                        _numeroDaPergunta++;
                        if (NumeroDaPergunta < 11)
                        {
                            TThree.Interval = 2000;
                            TThree.Start();
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Parabéns!!! Você acertou todas as perguntas!", "Parabéns! :-D");
                            TTwo.Interval = 4000;
                            TTwo.Start();
                        }
                    }
                    else
                    {
                        switch (Temp)
                        {
                            case 1:
                                SilvioSantos = Properties.Resources.sdm_errou;
                                break;
                            case 2:
                                SilvioSantos = Properties.Resources.sdm_errou2;
                                break;
                        }
                        Player.Stream = SilvioSantos;
                        Player.Play();
                        label.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF0FB0C"));
                        label.Background = new SolidColorBrush(Colors.Red);
                        // Mostra a resposta correta
                        switch (PerguntasSelecionadas[NumeroDaPergunta - 1].IndexCorreto)
                        {
                            case 0:
                                AlternativaA.Background = new SolidColorBrush(Colors.Green);
                                break;
                            case 1:
                                AlternativaB.Background = new SolidColorBrush(Colors.Green);
                                break;
                            case 2:
                                AlternativaC.Background = new SolidColorBrush(Colors.Green);
                                break;
                            case 3:
                                AlternativaD.Background = new SolidColorBrush(Colors.Green);
                                break;
                        }
                        TTwo.Interval = 4000;
                        TTwo.Start();
                    }
                }
                else
                {
                    label.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF0FB0C"));
                }
            }
        }
        /// <summary>
        /// Cria Perguntas pro questionário
        /// </summary>
        private void CriaPerguntas()
        {
            Perguntas[0] = new Pergunta("Em um triângulo retângulo, como é chamado a linha oposta ao ângulo reto?",new string[] { "Hipotenusa","Cateto Oposto","Cateto Adjacente","Cateto Reto" }, 0);
            Perguntas[1] = new Pergunta("Qual o nome do primeiro elemento quimico da tabela periodica?",new string[] { "Oxigênio","Carbono","Hidrogênio","Zinco" }, 2);
            Perguntas[2] = new Pergunta("Qual dessas moedas é a mais valiosa?",new string[] { "Rand","Real","Iene","Shekel" },1);
            Perguntas[3] = new Pergunta("Qual dessas palavras está escrita corretamente?", new string[] { "Meteorologia", "Espectativa", "Antiinflamatório", "Cabelereiro" }, 0);
            Perguntas[4] = new Pergunta("Quais países da américa tem o mesmo nome do que a capital?", new string[] { "Cuba e Venezuela", "México e Cuba", "Guiana e Guiana Francesa", "México e Guatemala" }, 3);
            Perguntas[5] = new Pergunta("Qual é a símbolo do enxofre?", new string[] { "E", "S", "X", "En" }, 1);
            Perguntas[6] = new Pergunta("Considerando que um carro percorre 1km em 50 segundos, Qual o valor de sua velocidade média?", new string[] { "60km/h","72km/h","80km/h","96km/h"}, 1);
            Perguntas[7] = new Pergunta("Quantos noves tem de 0 a 100?", new string[] { "18", "19", "20" , "21" }, 2);
            Perguntas[8] = new Pergunta("Qual destes órgãos faz parte do sistema digestivo?", new string[] { "Bexiga", "Pulmão", "Tireoide", "Duodeno" }, 3);
            Perguntas[9] = new Pergunta("Que animal é o Pateta?", new string[] { "Lontra", "Burro", "Cachorro", "Marmota" }, 2);
            Perguntas[10] = new Pergunta("A Osteoporose está ligada com a deficiência de qual vitamina?", new string[] { "B", "A", "K", "D" }, 3);
            Perguntas[11] = new Pergunta("Qual tecla é utilizada para atualizar uma página?", new string[] { "F5", "F12" , "F8" , "F9"}, 0);
            Perguntas[12] = new Pergunta("Qual desses elementos químicos é um gás nobre?", new string[] { "Kingium", "Xenônio", "Oxigênio", "Hidrogênio" }, 1);
            Perguntas[13] = new Pergunta("Das substâncias do cigarro, Qual dessas é a mais nociva?", new string[] { "Monóxido de Carbono", "Plutônio", "Nicotina", "Cianeto de Hidrogênio" }, 2);
            Perguntas[14] = new Pergunta("Qual destas alternativas é uma mudança do novo acordo ortográfico 2016?", new string[] { "Queda do acento diferencial", "Queda do cedilha", "Mudanças no \"porque\"", "Mudanças nas conjuções" }, 0);
            Perguntas[15] = new Pergunta("Qual dos estados abaixo tem sua capital com o mesmo nome?",new string[] { "Ceará", "Distrito Federal", "São Paulo" , "Paraná" }, 2);
            Perguntas[16] = new Pergunta("Em uma operação matemática, qual operação é resolvida primeiro?", new string[] { "Adição", "Subtração", "Divisão" , "Multiplicação" }, 3);
            Perguntas[17] = new Pergunta("O poeta romântico Álvares de Azevedo foi um poeta que se destacou em qual geração do Romantismo?", new string[] { "Primeira Geração - Nacionalista", "Segunda Geração - Mal do Século", "Terceira Geração - Condoreirista", "Quarta Geração - Simbolista" }, 1);
            Perguntas[18] = new Pergunta("A Claustrofobia é o medo de...", new string[] { "Lugares Abertos", "Lugares Fechados", "Enchente", "Avião" }, 1);
            Perguntas[19] = new Pergunta("Qual o valor da força gravitacional na lua?", new string[] { "10 m/s²", "1,6 m/s²", "3,2 m/s²", "17,2 m/s²" }, 1);
            Perguntas[20] = new Pergunta("Qual o animal é simbolizado pelo signo de Câncer?", new string[] { "Caranguejo" , "Carneiro" , "Leão" , "Lagarta" }, 0);
            Perguntas[21] = new Pergunta("Qual país sediou a primeira copa do mundo?", new string[] { "Uruguai", "Paraguai", "Argentina", "Brasil" }, 0);
            Perguntas[22] = new Pergunta("Que monstro da mitologia grega transformava qualquer um que olhasse para ele em pedra?", new string[] { "Medusa", "Cérbero" , "Hidra de Lerna", "Quimera" }, 0);
            Perguntas[23] = new Pergunta("É um país de Europa, No passado invadiu o Brasil, Terra das... Enfim, que país é esse?", new string[] { "Espanha", "França", "Itália", "Holanda" }, 3);
            Perguntas[24] = new Pergunta("Em um retângulo de base 5cm e altura de 3cm, qual o valor de sua área?", new string[] { "8 cm²", "15 cm²", "30 cm²", "34 cm²" }, 1);
            Perguntas[25] = new Pergunta("O que tem dentro das corcovas dos camelos?", new string[] { "Água", "Ar", "Gordura", "Osso" }, 2);
            Perguntas[26] = new Pergunta("Nos tempos antigos, o bronze era confudido com um outro metal muito parecido, só que de menor valor que o bronze. Que metal era esse?", new string[] { "Latão", "Cobre", "Zinco", "Estanho" }, 0);
            Perguntas[27] = new Pergunta("Qual o planeta mais quente do sistema solar?", new string[] { "Mercúrio", "Júpiter", "Vênus", "Marte" }, 2);
            Perguntas[28] = new Pergunta("Charles Darwin desenvolveu a Teoria da Evolução após navegar em qual navio que tinha nome de cachorro?", new string[] { "Poodle", "Chihuahua", "Bulldog", "Beagle" }, 3);
            Perguntas[29] = new Pergunta("Quantos decimetros cúbicos equivalem 1 litro?", new string[] { "1 dm³", "10 dm³", "100 dm³", "1000 dm³" }, 0);
            Perguntas[30] = new Pergunta("Qual é o único país cuja a bandeira não é retângular?", new string[] { "Nepal", "Senegal", "Filipinas", "Brunei" }, 0);
            Perguntas[31] = new Pergunta("Para se proteger de raios, você pode:", new string[] { "Se abrigar debaixo de uma árvore", "Abraçar os joelhos", "Ficar encostado em um carro", "Ir para um lugar aberto onde nada atraia raios" }, 1);
            Perguntas[32] = new Pergunta("Qual destes animais surgiu primeiro no planeta?", new string[] { "Tubarões", "Crocodilos", "Cobras", "Árvores" }, 0);
            Perguntas[33] = new Pergunta("Que presidente lançou um plano de metas cujo o lema era \"50 anos em 5\"?", new string[] { "Getúlio Vargas", "Fernando Collor", "Juscelino Kubitschek", "José Sarney" }, 2);
            Perguntas[34] = new Pergunta("O Ex-Presidente Carlos Luz detém o menor tempo de posse na presidência, que foi de:", new string[] { "4 dias", "1 semana", "2 semenas", "3 meses" }, 0);
            Perguntas[35] = new Pergunta("Do que eram feitos os travesseiros do Antigo Egito?", new string[] { "Peles de Animais", "Feixes de Trigo", "Sacos de Areia", "Pedra" }, 3);
            Perguntas[36] = new Pergunta("O que acontece com uma batata quando exposta no sol por muito tempo?", new string[] { "Ela fica seca e desmancha", "Ela fica verde e venenosa", "As raízes começam a brotar", "Ela fica dura e cinza feito pedra" }, 1);
            Perguntas[37] = new Pergunta("Por qual desses materiais o som viaja mais rápido?", new string[] { "Água", "Ar", "Gás Hélio", "Ferro" }, 3);
            Perguntas[38] = new Pergunta("O que é mais quente?", new string[] { "Um raio", "A superfície do Sol", "Lava de vulcão", "O núcleo da Terra" }, 0);
            Perguntas[39] = new Pergunta("Qual a temperatura aproximada em Celsius do chamado \"Zero absoluto\"?", new string[] { "-100°C", "-125°C", "-273°C", "-459°C" }, 2);
            Perguntas[40] = new Pergunta("Qual destes animais é o parente mais próximo do T-Rex?", new string[] { "Jacaré", "Dragão de Komodo", "Galinha", "Canguru" }, 2);
            Perguntas[41] = new Pergunta("BRIC é uma sigla que se refere a quais países?", new string[] { "Brasil, Rússia, Índia e China", "Brasil, Rússia, Inglaterra e China", "Bélgica, Rússia, Itália e China", "Bélgica, Rússia, Inglaterra e China" }, 0);
            Perguntas[42] = new Pergunta("Qual é o elemento que causou o acidente radioativo de Goiânia em 1987?", new string[] { "Carbono-14", "Urânio-235", "Polônio-210", "Césio-137" }, 3);
            Perguntas[43] = new Pergunta("Qual é o maior órgão do corpo humano?", new string[] { "Fígado", "Intestino", "Pulmão", "Pele" }, 3);
            Perguntas[44] = new Pergunta("Qual o elemento químico mais abundante no sol?", new string[] { "Hélio", "Hidrogênio", "Oxigênio", "Nitrogênio" }, 1);
            Perguntas[45] = new Pergunta("Com que frequência, em média, ocorre um eclipse total do sol?", new string[] { "18 meses", "12 anos", "28 anos", "57 anos" }, 0);
            Perguntas[46] = new Pergunta("Qual destas pessoas nunca foi indicada para um nobel da paz?", new string[] { "Hitler", "Kim Jong-il", "Stalin", "Mussolini" }, 1);
            Perguntas[47] = new Pergunta("Qual é o país que tem o maior número de pirâmides?", new string[] { "Sudão", "Egito", "México", "Índia" }, 0);
            Perguntas[48] = new Pergunta("O herói Thor de os Vingadores é uma referência a um deus de qual mitologia?", new string[] { "Grega", "Romana", "Nórdica", "Asteca" }, 2);
            Perguntas[49] = new Pergunta("Legalmente, um homem pode se casar com a irmã da sua viúva?", new string[] { "Sim", "Não", "Em alguns países, incluindo o Brasil", "Em alguns países, mas não no Brasil" }, 1);
            Perguntas[50] = new Pergunta("Como era chamada a máquina que criptografava as mensagens militares do nazistas durante a 2ª Guerra Mundial?", new string[] { "Heimlich", "Krebs", "Enigma", "Verboten" }, 2);
            Perguntas[51] = new Pergunta("Em um tocador de disco de vinil, do que são feitas as agulhas mais caras?", new string[] { "Ouro", "Tungstênio", "Cobre", "Diamante" }, 3);
            Perguntas[52] = new Pergunta("Qual foi o presidente que deu o Golpe do Estado Novo?", new string[] { "José Linhares", "João Goulart", "Getúlio Vargas", "Café Filho" }, 2);
            Perguntas[53] = new Pergunta("Qual o poder criado por D.Pedro II na Constituição de 1824?", new string[] { "Legislativo", "Executivo", "Judiciário", "Moderador" }, 3);
            Perguntas[54] = new Pergunta("Qual foi o Tratado assinado em 1810 por Dom João VI?", new string[] { "Tratado de Tordesilhas", "Tratado de Madrid", "Tratado de Comércio e Navegação", "Tratado de Porto Seguro" }, 2);
            Perguntas[55] = new Pergunta("Qual planta foi atingida por uma praga que causou a Grande Fome da Irlanda?", new string[] { "Beterraba", "Batata", "Milho", "Trigo" }, 1);
            Perguntas[56] = new Pergunta("Qual o maior deserto do mundo?", new string[] { "Saara", "Arábia", "Gobi", "Antártida" }, 3);
            Perguntas[57] = new Pergunta("Qual é a cor que fica do lado de fora do arco-íris?", new string[] { "Violeta", "Verde", "Amarelo", "Vermelho" }, 3);
            Perguntas[58] = new Pergunta("O que pesa mais?", new string[] { "1kg de Penas", "1kg de Ferro", "1kg de Madeira", "Mesmo Peso" }, 3);
            Perguntas[59] = new Pergunta("Qual é a unidade de medida do peso?", new string[] { "Kg", "N", "L", "M" }, 1);
            Perguntas[60] = new Pergunta("\"Só sei que nada sei\". Esta frase foi dita por qual filósofo?", new string[] { "Descartes", "Sócrates", "Platão", "Nietzsche" }, 1);
            Perguntas[61] = new Pergunta("Qual é o menor país do mundo?", new string[] { "Vaticano", "Mônaco", "Nauru", "Tuvalu" }, 0);
            Perguntas[62] = new Pergunta("Quanto tempo a luz do Sol demora para chegar à Terra?", new string[] { "1 segundo", "8 minutos", "1 hora", "1 dia" }, 1);
            Perguntas[63] = new Pergunta("Qual era o nome de Aleijadinho?", new string[] { "Alexandrino Francisco Lisboa", "Manuel Francisco Lisboa", "Francisco Manuel Lisboa", "Antônio Francisco Lisboa" }, 3);
            Perguntas[64] = new Pergunta("As pessoas de qual tipo sanguíneo são consideradas doadores universais?",new string[] { "Tipo A", "Tipo B", "Tipo AB", "Tipo O" },3);
            Perguntas[65] = new Pergunta("Normalmente, quantos litros de sangue uma pessoa tem?", new string[] { "De 2 a 4 litros", "De 4 a 6 litros", "De 6 a 8 litros", "De 8 a 10 litros" }, 1);
            Perguntas[66] = new Pergunta("Qual o número mínimo de jogadores numa partida de futebol?", new string[] { "5", "7", "9", "10" }, 1);
            Perguntas[67] = new Pergunta("Lady Di era o apelido de qual personalidade?", new string[] { "Joana d’Arc", "Grace Kelly", "Diana, a Princesa de Gales", "Chiquinha Gonzaga" }, 2);
            Perguntas[68] = new Pergunta("Qual é o maior ser vivo da Terra?", new string[] { "Um mamífero", "Um fungo", "Um réptil", "Um peixe" }, 1);
            Perguntas[69] = new Pergunta("Qual é a capital dos EUA?", new string[] { "Las Vegas", "California", "Nova York", "Washington D.C." }, 3);
            Perguntas[70] = new Pergunta("Qual é o maior osso do corpo humano?", new string[] { "Fêmur", "Rádio", "Tibia", "Estribo" }, 0);
            Perguntas[71] = new Pergunta("Qual mês do ano tem 28 dias?", new string[] { "Fevereiro", "Dezembro", "Nenhum", "Todos" }, 3);
            Perguntas[72] = new Pergunta("Qual é o alimento preferido do Garfield?", new string[] { "Macarronada", "Lasanha", "Rosbife", "Ração de Cachorro" }, 1);
            Perguntas[73] = new Pergunta("Em que ano começou, e em que ano terminou a guerra fria?", new string[] { "1947-1992", "1945-1992", "1945-1991", "1954-1992" }, 0);
            Perguntas[74] = new Pergunta("Qual foi a primeira civilização a habitar a Mesopotâmia?", new string[] { "Caldeus", "Babilôncos", "Sumérios", "Hititas" }, 2);
            Perguntas[75] = new Pergunta("Quem realmente descobriu o Brasil?", new string[] { "Duarte Pacheco Pereira", "Pedro Álvares Cabral", "D. Pedro II", "D. João IV" }, 0);
            Perguntas[76] = new Pergunta("Qual o nome do brasileiro conhecido como \"Pai da Aviação\"?", new string[] { "José de Alencar", "Santos Dumont", "Osvaldo Cruz", "Castro Alves" }, 1);
            Perguntas[77] = new Pergunta("Qual foi a revolução que alavancou a independência do Brasil?", new string[] { "Revolução Farroupilha", "Revolução Praieira", "Revolução Federalista", "Revolução Pernambucana" }, 3);
            Perguntas[78] = new Pergunta("O que é o Brexit?", new string[] { "Saída da Inglaterra do Reino Unido", "Saída do Reino Unido da União Europeia", "Fim da monarquia no Reino Unido", "Saída do Reino Unido da Zona Euro" }, 1);
            Perguntas[79] = new Pergunta("O acordo internacional de Paris, é um acordo que trata...", new string[] { "da restrição de imigrantes em Paris", "da proteção da França dos atentados terroristas", "do aquecimento global", "do Desenvolvimento Sustentável" }, 2);
            Perguntas[80] = new Pergunta("Por quanto tempo durou a Guerra dos 100 anos?", new string[] { "98 anos", "100 anos", "106 anos", "116 anos" }, 3);
            Perguntas[81] = new Pergunta("Como é chamado o nome das pinturas das cavernas?", new string[] { "Rupestre", "Moderna", "Geométrica", "Paleolitica" }, 0);
            Perguntas[82] = new Pergunta("Com quantos anos de casado se comemora a tradicional \"Bodas de Ouro\"?", new string[] { "25 anos", "50 anos", "75 anos", "100 anos" }, 1);
            Perguntas[83] = new Pergunta("Quantos elementos químicos a tabela periódica possui?", new string[] { "108", "109", "113", "118" }, 3);
            Perguntas[84] = new Pergunta("Quais os países que têm a maior e a menor expectativa de vida do mundo?", new string[] { "Austrália e Afeganistão", "Estados Unidos e Angola", "Japão e Serra Leoa", "Itália e Chade" }, 2);
            Perguntas[85] = new Pergunta("Para qual direção o ônibus abaixo está indo?", new string[] { "Esquerda", "Direita", "Inclinada", "Reta" }, 0, new Uri("pack://application:,,,/ShowDoMilhao;component/Resources/sdm_exercicio5.png"));
            Perguntas[86] = new Pergunta("Qual o valor da área do círculo abaixo?", new string[] { "28,26 cm²", "56,52 cm²", "254,34 cm²", "1017,36 cm²" }, 2, new Uri("pack://application:,,,/ShowDoMilhao;component/Resources/sdm_exercicio4.png"));
            Perguntas[87] = new Pergunta("O que as palavras abaixo tem em comum?", new string[] { "Ambas são substantivos", "Ambas são verbos", "Ambas estão no gênero masculino", "Nada" }, 1, new Uri("pack://application:,,,/ShowDoMilhao;component/Resources/sdm_exercicio2.png"));
            Perguntas[88] = new Pergunta("Qual o perimetro do retângulo abaixo?", new string[] { "24", "28", "40", "80" }, 1, new Uri("pack://application:,,,/ShowDoMilhao;component/Resources/sdm_exercicio3.png"));
            Perguntas[89] = new Pergunta("Qual o valor do ângulo X do triângulo abaixo?", new string[] { "90°", "60°", "45°", "30°" }, 3, new Uri("pack://application:,,,/ShowDoMilhao;component/Resources/sdm_exercicio1.png"));
            Perguntas[90] = new Pergunta("O que quer dizer a logo abaixo?", new string[] { "Playtone", "Youtube", "Vine", "Instagram" }, 1, new Uri("pack://application:,,,/ShowDoMilhao;component/Resources/sdm_exercicio6.png"));
            Perguntas[91] = new Pergunta("Quem é, atualmente(maio/2018), o técnico do Barcelona?", new string[] { "Ernesto Valverde", "Tito Vilanova", "Gerardo Martino", "José Mourinho" }, 0);
            Perguntas[92] = new Pergunta("Quando é celebrado o dia do Profissional de TI?", new string[] { "15 de Novembro", "17 de Novembro", "15 de Outubro", "19 de Outubro" }, 3);
            Perguntas[93] = new Pergunta("Como podemos definir uma sombra?", new string[] { "Ausência de luz", "Ausência de cor", "Ausência de eletricidade", "Ausência de vento" }, 0);
            Perguntas[94] = new Pergunta("Quem foi que criou o modelo conhecido como \"Pudim de Passas\"?", new string[] { "Bohr", "Tesla", "Rutherford", "Thomson" }, 3);
            Perguntas[95] = new Pergunta("Você tem um isqueiro, e os itens abaixo. Qual você acende primeiro?", new string[] { "A vela", "O lampião", "O monte de palha", "Nenhuma das alternativas" }, 3, new Uri("pack://application:,,,/ShowDoMilhao;component/Resources/sdm_exercicio7.png"));
            Perguntas[96] = new Pergunta("Qual desses países não faz fronteira com o Brasil?", new string[] { "Venezuela", "Equador", "Guiana", "Peru" }, 1);
            Perguntas[97] = new Pergunta("Com quantas linhas é possivel unir os pontos abaixo, sem tirar a caneta do papel?", new string[] { "3", "4", "5", "Desafio Impossivel" }, 1, new Uri("pack://application:,,,/ShowDoMilhao;component/Resources/sdm_exercicio8.png"));
            Perguntas[98] = new Pergunta("Qual mensagem o ET Bilu mandou para o mundo?", new string[] { "Beba água", "Procure a sabedoria", "Busquem conhecimento", "Encontre a verdade" }, 2);
            Perguntas[99] = new Pergunta("Qual assunto é muito polêmico, segundo um meme?", new string[] { "Crush", "Mamilos", "Religião", "Terrorismo" }, 2);
            Perguntas[100] = new Pergunta("Se um trem elétrico viaja em direção ao sul, em que direção vai a fumaça?", new string[] { "Norte", "Sul", "Pra Cima", "Nenhuma das Alternativas" }, 3);
            Perguntas[101] = new Pergunta("Quantas vezes é possível subtrair 10 de 100?", new string[] { "1 vez", "5 vezes", "10 vezes", "Infinitas Vezes" }, 0);
            Perguntas[102] = new Pergunta("A mãe de Maria tem 5 filhos: Lalá, Lelé, Lili, Loló e...?", new string[] { "Lila", "Lula", "Lulu", "Nenhuma das anteriores" }, 3);
            Perguntas[103] = new Pergunta("Qual era o monte mais alto do mundo antes do Everest ser descoberto?", new string[] { "Everest", "Kokoram-2", "Lhotse", "Makalu" }, 0);
            Perguntas[104] = new Pergunta("Em que ano o FC. Barcelona foi fundado?", new string[] { "1896", "1898", "1899", "1900" }, 2);
            Perguntas[105] = new Pergunta("Qual o ano do primeiro título Brasileiro do Corinthians?", new string[] { "1990", "1993", "1996", "1998" }, 0);
            Perguntas[106] = new Pergunta("Quantos anagramas tem a palavra: Ana?", new string[] { "1", "3", "10", "30" }, 1);
            Perguntas[107] = new Pergunta("Qual dos verbos abaixo está conjugado no gerúndio?", new string[] { "Entregar", "Respondido", "Acertando", "Errou" }, 2);
            Perguntas[108] = new Pergunta("Em que campeonato de 2016 o Palmeiras se sagrou campeão?", new string[] { "Campeonato Brasileiro", "Campeonato Paulista", "Copa Libertadores", "Nenhum" }, 0);
            Perguntas[109] = new Pergunta("Como é chamado o número que é representado por 0s e 1s?", new string[] { "Decimal", "Binário", "Hexadecimal", "Hexabinário" }, 1);
            Perguntas[110] = new Pergunta("\"Ser ponual\" Significa ser:", new string[] { "Atrasado", "Pontual", "Lento", "Certo"}, 1);
            Perguntas[111] = new Pergunta("Qual o plural de Guarda-Civil?", new string[] { "Guardas-Civil", "Guarda-Civis", "Guardas-Civis", "Guardas-Civeis"}, 2);
            Perguntas[112] = new Pergunta("Qual o presidente foi responsável pela construção de Brasilia?", new string[] { "Juscelino Kubitschek", "Jânio Quadros", "Getúlio Vargas", "Eurico Gaspar Dutra" }, 0);
            Perguntas[113] = new Pergunta("Como chamamos o sono do urso durante o inverno?", new string[] { "Preguiça", "Cochilo", "Desmaio", "Hibernação" }, 3);
            Perguntas[114] = new Pergunta("Qual foi o primeiro nome dado ao \"Show do Milhão\"?", new string[] { "Milionários", "Jogo do Milhão", "Ganhe um Milhão", "Sempre foi Show do Milhão" }, 1);
            Perguntas[115] = new Pergunta("Quem escreveu o livro \"O Guarani\"", new string[] { "Machado de Assis", "Olavo Billac", "José de Alencar", "Carlos Gomes" }, 2);
            Perguntas[116] = new Pergunta("Quem foi o amor de Isolda na obra medieval?", new string[] { "Romeu", "Hamlet", "Tristão", "Rigoletto" }, 2);
            Perguntas[117] = new Pergunta("Complete o dito popular: \"Todos os caminhos levam a...\"", new string[] { "Ruína", "Liberdade", "Nenhum Lugar", "Roma"}, 3);
            Perguntas[118] = new Pergunta("Em que século a Princesa Isabel assinou a Lei Áurea?", new string[] { "XVI", "XVII", "XVIII", "XIX" }, 3);
            Perguntas[119] = new Pergunta("Em qual estado brasileiro o pão de queijo é uma comida típica?", new string[] { "Minas Gerais", "Pernambuco", "Goiás", "Rio de Janeiro" }, 0);
        }
        /// <summary>
        /// Seleciona 10 perguntas aleatórias a serem usadas na rodada
        /// </summary>
        private void PrepararPerguntas()
        {
            int PerguntasPreparadas = 0;
            bool ConfirmarPergunta = false;

            for(var I = 0; PerguntasPreparadas < 10; I++)
            {
                var Index = R.Next(Perguntas.Length);
                for(var J = 0; J <= I && J < 10; J++)
                {
                    // Testa se a pergunta já foi selecionada, para evitar repetição de perguntas
                    if(PerguntasSelecionadas[J] == Perguntas[Index])
                    {
                        ConfirmarPergunta = false;
                        break;
                    }
                    else
                    {
                        ConfirmarPergunta = true;
                    }
                }
                // Confirma que a pergunta não é repetida
                if(ConfirmarPergunta)
                {
                    PerguntasSelecionadas[PerguntasPreparadas] = Perguntas[Index];
                    PerguntasPreparadas++;
                    //Console.WriteLine("Pergunta " + I + ": " + PerguntasSelecionadas[PerguntasPreparadas].Questao);
                }
            }
        }
        /// <summary>
        /// Prepara o placar
        /// </summary>
        private void AtualizarPlacar()
        {
            // Evita clonagem de dados
            this.Scoreboard.Items.Clear();      
            foreach(Player usuario in Placar.GetPlacarOrdenado())
            {
                this.Scoreboard.Items.Add(usuario);
            }
            this.Scoreboard.Items.Refresh();

        }
        /// <summary>
        /// Encerra o jogo, salvando os dados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncerraJogo(object sender, EventArgs e)
        {
            if(IndicadorPergunta.Visibility == Visibility.Hidden)
            {
                // Mostra quanto o jogador ganhou
                IndicadorPergunta.FontSize = 48.0;
                IndicadorPergunta.Margin = new Thickness(128, 171, 0, 0);
                IndicadorPergunta.Content = "Você ganhou R$" + Guest.Pontuacao + "!";
                IndicadorPergunta.Visibility = Visibility.Visible;
                // Salva sua pontuação
                Placar.SalvarPontuacao();
                // Silvio santos lhe deseja parabens caso você tenha ganhado!
                if(NumeroDaPergunta == 11)
                {
                    SilvioSantos = Properties.Resources.sdm_parabens;
                    Player.Stream = SilvioSantos;
                    Player.Play();
                }
                // Remove as perguntas e as alternativas
                Pergunta.Visibility = Visibility.Hidden;
                AlternativaA.Visibility = Visibility.Hidden;
                AlternativaB.Visibility = Visibility.Hidden;
                ImagemPergunta.Visibility = Visibility.Hidden;
                AlternativaC.Visibility = Visibility.Hidden;
                AlternativaD.Visibility = Visibility.Hidden;
                Pontos.Visibility = Visibility.Hidden;
            }
            else
            {
                // Mostra o menu
                AuxForAnimation = 9;
                IndicadorPergunta.FontSize = 48.0;
                // Reseta Pontuação
                Guest.Pontuacao = 0;
                // Atualiza o placar incluindo a nova pontuação
                this.AtualizarPlacar();
                // Atualiza o número da pergunta de volta ao inicio
                _numeroDaPergunta = 1;
                IndicadorPergunta.Visibility = Visibility.Hidden;
                TTwo.Stop();
            }
        }
        /// <summary>
        /// Handler dos botões do menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button Botao = sender as System.Windows.Controls.Button;
            // Botão Iniciar
            if(Botao.Name.Equals("BIniciar"))
            {
                // Cria, caso não tenha sido criada as perguntas, e prepara as perguntas selecionando 10 aleatorias
                if(this.Perguntas[0] == null)
                {
                    this.CriaPerguntas();
                }
                this.PrepararPerguntas();

                System.Windows.MessageBox.Show("Serão feitas 10 perguntas de múltipla escolha, onde na primeira rodada cada pergunta certa vale mais 1000 reais, na segundada rodada cada pergunta certa vale mais 10000 reais e a última pergunta vale 100000 reais! Preparado, "+ Guest.Nome+"?", "Como funciona?", MessageBoxButton.YesNo);
                Player.Stop();
                System.Windows.MessageBox.Show("Ótimo, vamos começar!", "Perfeito!");
                MiniLogo.Visibility = Visibility.Hidden;
                BIniciar.Visibility = Visibility.Hidden;
                SilvioImage.Visibility = Visibility.Hidden;
                BPlacar.Visibility = Visibility.Hidden;
                BSair.Visibility = Visibility.Hidden;
                Titulo.Visibility = Visibility.Hidden;
                TThree.Interval = 1;
                TThree.Start();
            }
            // Botão do placar
            else if (Botao.Name.Equals("BPlacar"))
            {
                this.Height = 744.0;
                this.Width = 627.0;
                this.Copyright.Margin = new Thickness(0, 684, 0, 0);
                this.TelaPrincipal.Visibility = Visibility.Hidden;
                this.TelaPlacar.Visibility = Visibility.Visible;
            }
            // Botão de sair
            else if(Botao.Name.Equals("BSair"))
            {
                if((System.Windows.MessageBox.Show("Deseja sair do jogo?","Já vai embora?",MessageBoxButton.YesNo,MessageBoxImage.Information)) == MessageBoxResult.Yes)
                {
                    Environment.Exit(0);
                }
            }
            else if (Botao.Name.Equals("VoltarMenu"))
            {
                this.TelaPlacar.Visibility = Visibility.Hidden;
                this.TelaPrincipal.Visibility = Visibility.Visible;
                this.Width = 625.0;
                this.Copyright.Margin = new Thickness(0, 390, 0, 0);
                this.Height = 450.0;
            }
        }
        // Muda a cor do background caso o item seja selecionado
        private void ItemSelected(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!Respondeu)
            {
                System.Windows.Controls.Label L = sender as System.Windows.Controls.Label;
                L.Background = new SolidColorBrush(Colors.DarkRed);
            }
        }
        // Volta ao background padrão caso o item seja deselecionado
        private void ItemDeSelected(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!Respondeu)
            {
                System.Windows.Controls.Label L = sender as System.Windows.Controls.Label;
                L.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
