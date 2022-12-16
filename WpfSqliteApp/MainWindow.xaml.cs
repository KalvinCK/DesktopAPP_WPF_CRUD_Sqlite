using MaterialDesignThemes.Wpf.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace WpfSqliteApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Tuple<int, string>? ItemSelected;
        public MainWindow()
        {
            Uri iconUri = new Uri("pack://application:,,,/Resources/win.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);


            // define se a nossa janela pode ser redimensionada
            this.ResizeMode = ResizeMode.CanResize;

            // seta o estado da janela 
            this.WindowState = WindowState.Normal;

            // set a posicao de inicio da janela
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // define se sua aplicação sera exibida na barra de tarefas
            this.ShowInTaskbar = true;

            // define se sua janela fica por cima de todas as outras
            this.Topmost = false;

            InitializeComponent();

            ItemSelected = null;

            Read();

        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
        private void IncludeAccept_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
        }
        private void ListGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListGames.SelectedItem != null)
            {
                //selectedItem = (ListGames.SelectedItem as Game)!.Id;
                ItemSelected = new Tuple<int, string>( (ListGames.SelectedItem as Game)!.Id, (ListGames.SelectedItem as Game)!.Name );
            }
            else
            {
                ItemSelected = null;
            }
        }
        // Buttons clicks
        private void IncludeButton(object sender, RoutedEventArgs e)
        {
            CreateData();
        }
        private void UpdateButton(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }
        private void DeleteButton(object sender, RoutedEventArgs e)
        {
            DeleteData();

        }
        private void ClearText()
        {
            Nome.Text = string.Empty;
            Desenvolvedora.Text = string.Empty;

            Ano.Text = string.Empty;
            Mes.Text = string.Empty;
            Dia.Text = string.Empty;
            
        }
        public DateTime currentDate { get => new DateTime(int.Parse(Ano.Text), int.Parse(Mes.Text), int.Parse(Dia.Text)); }

        private bool CheckAttributes()
        {
            if( Nome.Text != string.Empty && Nome.Text.Length > 1 &&
                Desenvolvedora.Text != string.Empty && Desenvolvedora.Text.Length > 1 &&
                Ano.Text != string.Empty && Mes.Text != string.Empty && Dia.Text != string.Empty &&
                Ano.Text.Length == 4 && int.Parse(Mes.Text) <= 12 && int.Parse(Dia.Text) <= 31)
            {
                return true;
            }
            else
            {
                CustomDialog customDialog = new CustomDialog("Os valores estão incorretos, corriga-os e tente novamente");
                customDialog.ShowDialog();

                return false;
            }
        }
        
        private bool IsExists(ref Game game)
        {
            using (var db = new GameContext())
            {

                if (db.Games.Contains(game))
                {
                    CustomDialog customDialog = new CustomDialog("Este Game ja esta cadastrado");
                    customDialog.ShowDialog();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        private string TransformsUpper(string text)
        {

            var textFormated = string.Empty;

            foreach (var index in text.Split(" "))
            {
                string result = index;

                if (!char.IsNumber(index[0]) || Regex.IsMatch($"{index[0]}", (@"[!""#$%&'()*+,-./:;?@[\\\]_`{|}~]")) )
                {
                    result = char.ToUpper(index[0]) + index[1..];
                }
                textFormated += $"{result} ";
            }
            return textFormated;
        }
        private static bool CheckItemSelected()
        {
            if (ItemSelected == null)
            {
                CustomDialog customDialog = new CustomDialog("Nemhum item esta selecionado");
                customDialog.ShowDialog();

            }
     
            return (ItemSelected != null) ? true : false;
        }

        // Crud Bando de dados
        //-------------------------------------------------------------------------
        private void Read()
        {
            ListGames.ItemsSource = new GameContext().Games.ToList();
        }
        private void CreateData()
        {
            if(CheckAttributes())
            {
                Game game = new Game
                {
                    Name = Nome.Text,
                    Developer = Desenvolvedora.Text,
                    ReleaseDate = currentDate
                };

                if(IsExists(ref game))
                {
                    using (var db = new GameContext())
                    {
                        game.Name = TransformsUpper(game.Name);
                        game.Developer = TransformsUpper(game.Developer);
                        
                        db.Add(game);
                        db.SaveChanges();
                    }

                    ClearText();
                    Read();
                }
            }
           

        }
        private void UpdateData()
        {
    
            if(CheckItemSelected())
            {
                if(CheckAttributes())
                {
                    using (var db = new GameContext())
                    {
                        var _game = new Game
                        {
                            Name = Nome.Text,
                            Developer = Desenvolvedora.Text,
                            ReleaseDate = currentDate,
                        };

                        if(IsExists(ref _game))
                        {
                            var game = db.Games.Single(g => g.Id == ItemSelected!.Item1);

                            game.Name = _game.Name;
                            game.Developer = _game.Developer;
                            game.ReleaseDate = _game.ReleaseDate;

                            game.Name = TransformsUpper(game.Name);
                            game.Developer = TransformsUpper(game.Developer);

                            db.Update(game);
                            db.SaveChanges();

                            ClearText();        
                            Read(); 
                        }

                    }
                }
            }
        }
        private void DeleteData()
        {
    
            if(CheckItemSelected())
            {

                using (var db = new GameContext())
                {
                    var game = db.Games.Single(g => g.Id == ItemSelected!.Item1);
                    db.Remove(game);
                    db.SaveChanges();
                }
                Read();
            }
        }
    }
}
