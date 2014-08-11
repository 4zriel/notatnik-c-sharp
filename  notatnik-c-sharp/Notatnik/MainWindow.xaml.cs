using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace Notatnik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path;
        private bool edited;
        private bool opened;
        private string titleName = "Notes";

        public void Reseter()
        {
            MainTextBox.Text = "";
            MainTextBox.IsEnabled = false;
            Notes.Title = titleName;
            opened = false;
            edited = false;
            this.Opacity = 1;
        }
        public void ReadFile(string filePath)
        {
            StreamReader FileReader = new StreamReader(filePath);
            string tmpText = FileReader.ReadToEnd();
            MainTextBox.Text = tmpText;
            FileReader.Close();
        }
        public void SaveFile(string filePath)
        {
            string tmpText = MainTextBox.Text;
            Reseter();
            StreamWriter FileSaver = new StreamWriter(filePath);
            FileSaver.WriteLine(tmpText);
            FileSaver.Close();
        }

        public void Opener()
        {
            try
            {
                OpenFileDialog openDial = new OpenFileDialog();
                openDial.DefaultExt = "*.txt";
                openDial.FileName = "";
                openDial.Filter = "Text files|*.txt|XML files|*.xml";
                bool? openedDial = openDial.ShowDialog();

                if (openedDial.HasValue && openedDial.Value)
                {
                    this.path = openDial.FileName;
                    MainTextBox.Text = this.path;
                    ReadFile(this.path);
                    opened = true;
                    MainTextBox.IsEnabled = true;
                    MainTextBox.AcceptsReturn = true;
                    MainTextBox.AcceptsTab = true;
                    Notes.Title = titleName + ": " + path;
                    MainTextBox.Opacity = 75;
                }
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Błąd otwarcia pliku!", "Error!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        public void Saver()
        {
            try
            {
                SaveFileDialog saveDial = new SaveFileDialog();
                saveDial.DefaultExt = "*.txt";
                saveDial.FileName = "";
                saveDial.Filter = "Text files|*.txt|XML files|*.xml";
                bool? Dial = saveDial.ShowDialog();
                if (Dial.HasValue && Dial.Value)
                {
                    this.path = saveDial.FileName;
                    this.SaveFile(this.path);
                    Reseter();
                }
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Błąd zapisu!", "Error!", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.opened = false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void Wyjdź_z_programu_Click(object sender, RoutedEventArgs e)
        {
            if (edited)
            {
                MessageBoxResult result = MessageBox.Show("Zapisać zmiany?", "Notes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        this.Saver();
                        this.Close();
                        break;
                    case MessageBoxResult.No:
                        this.Close();
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else
                this.Close();                
        }

        private void Otwórz____Click(object sender, RoutedEventArgs e)
        {
            if (!opened)
            {
                this.Opener();
                Notes.Opacity = 0.75;                
            }
            else
            {
                MessageBox.Show("Nie można otworzyć wiecej niż jednego pliku!", "Notes", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Notes_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.Q))
            {
                if (edited && opened)
                {
                    MessageBoxResult result = MessageBox.Show("Zapisać zmiany?", "Notes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            this.Saver();
                            this.Close();
                            break;
                        case MessageBoxResult.No:
                            this.Close();
                            break;
                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
                else
                    this.Close();
            }
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.O))
            {
                this.Opener();
            }
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.S))
            {
                Saver();
                this.Close();
            }
        }

        private void Zamknij_Click(object sender, RoutedEventArgs e)
        {
            if (edited && opened)
            {
                MessageBoxResult result = MessageBox.Show("Zapisać zmiany?", "Notes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        this.Saver();
                        break;
                    case MessageBoxResult.No:
                        Reseter();
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
            else if (!edited && opened)
                Reseter();
            else
                Reseter();
        }

        private void MainTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            edited = true;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Notes_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (edited && opened)
            {
                MessageBoxResult result = MessageBox.Show("Zapisać zmiany?", "Notes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        this.Saver();
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }
        private void Formatuj_Click(object sender, RoutedEventArgs e)
        {
            if (opened)
            {
                MainTextBox.Text = Regex.Replace(MainTextBox.Text, "\r\n {4}", "\r\n\t");
                edited = true;
            }
        }

    }
}
