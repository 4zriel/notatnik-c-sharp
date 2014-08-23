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
		private List<string> recentFiles = new List<string>();
		private static RegistryKey tmpKey = Registry.LocalMachine.OpenSubKey("Software", true);
		private RegistryKey registryRecentFile = tmpKey.CreateSubKey("Notatnik_C_Sharp");

		public MainWindow()
		{
			this.AllowsTransparency = true;
			InitializeComponent();
			this.opened = false;
			RegistryChecker();
			RecentFilesAddToMenu(recentFiles, true);
		}

		public void RecentFilesAddToMenu(List<string> files, bool isInitialization)
		{
			
			if (isInitialization)
			{
				foreach (var item in files)
				{
					MenuItem lastOpenedFiles = new MenuItem();
					lastOpenedFiles.Header = item.ToString();
					LastOpened.Items.Add(lastOpenedFiles);
					lastOpenedFiles.Click += new RoutedEventHandler(lastOpenedFiles_Click);
				}
				LastOpened.UpdateLayout();
			}
			else
			{
				var tmpLast = files.Last();
				MenuItem lastOpenedFiles = new MenuItem();
				lastOpenedFiles.Header = tmpLast.ToString();
				LastOpened.Items.Add(lastOpenedFiles);
				lastOpenedFiles.Click += new RoutedEventHandler(lastOpenedFiles_Click);
			}
		} 

		public void RegistryAddRead(string openFileName)        
		{
			var hasValue = registryRecentFile.GetValue(openFileName);
			if (hasValue == null)
			{
				this.registryRecentFile.SetValue(openFileName, this.path);
				var tmp = registryRecentFile.GetValue(openFileName);
			}
		}

		public bool RegistryChecker()
		{
			bool addedToList = false;
			string[] tmpArray = registryRecentFile.GetValueNames();
			if (tmpArray.Length != 0)
				{
				foreach (var item in tmpArray)
				{
					if (!(recentFiles.Contains(item)))
					{
						var tmp = registryRecentFile.GetValue(item);
						recentFiles.Add(tmp.ToString());
						addedToList = true;
					}
				}
			}
			return addedToList;
		}

		public void Reseter()
		{
			MainTextBox.Text = "";
			MainTextBox.IsEnabled = false;
			Notes.Title = titleName;
			opened = false;
			edited = false;
			if(RegistryChecker())
				RecentFilesAddToMenu(recentFiles, false);
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

		public void OpenFile(string recentFileName)
		{
			try
			{
				if (recentFileName.Count() == 0)
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
						edited = false;
					}
				}
				else
				{
					this.path = recentFileName;
					MainTextBox.Text = this.path;
					ReadFile(this.path);
					opened = true;
					MainTextBox.IsEnabled = true;
					MainTextBox.AcceptsReturn = true;
					MainTextBox.AcceptsTab = true;
					Notes.Title = titleName + ": " + path;
					edited = false;
					}
			}
			catch
			{
				MessageBoxResult result = MessageBox.Show("Błąd otwarcia pliku!", "Error!", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
		}

		public bool Saver()
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
					return true;
				}
				else
					return false;
			}
			catch
			{
				MessageBoxResult result = MessageBox.Show("Błąd zapisu!", "Error!", MessageBoxButton.OK, MessageBoxImage.Stop);
				return false;
			}
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void MenuItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			if (!opened)
			{
				this.OpenFile("");
			}
			else
			{
				MessageBox.Show("Nie można otworzyć wiecej niż jednego pliku!", "Notes", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void Wyjdź_z_programu_Click(object sender, RoutedEventArgs e)
		{
			if (edited)
			{
				if (AskBeforeClose())
					this.Close();
			}
			else
				this.Close();                
		}

		private void Otwórz____Click(object sender, RoutedEventArgs e)
		{
			if (!opened)
			{
				this.OpenFile("");              
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
					if (AskBeforeClose())
						this.Close();
				}
				else
					this.Close();
			}
			if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.O))
			{
				this.OpenFile("");
			}
			if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.S))
			{
				Saver();
				this.Close();
			}
		}

		/// <summary>
		/// Ask before close
		/// </summary>
		/// <returns>If someone cancel ask window - return false</returns>
		private bool AskBeforeClose()
		{
			MessageBoxResult result = MessageBox.Show("Zapisać zmiany?", "Notes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
			bool cancel = false;
			if (result == MessageBoxResult.Yes)
			{
				if (!this.Saver())
					cancel = true;
			}
			else
				if (result == MessageBoxResult.Cancel)
					cancel = true;
			edited = cancel;
			return !cancel;
		}

		private void Zamknij_Click(object sender, RoutedEventArgs e)
		{
			if (edited && opened)
			{
				if (AskBeforeClose())
				{
					RegistryAddRead(this.path);
					Reseter();
				}
			}
			else if (!edited && opened)
			{
				RegistryAddRead(this.path);
				Reseter();
			}

			else
			{
				RegistryAddRead(this.path);
				Reseter();
			}
		}

		private void MainTextBox_KeyDown(object sender, KeyEventArgs e)
		{
		}
		public void lastOpenedFiles_Click(Object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			if (menuItem != null)
			{
				if (!opened)
					this.OpenFile(menuItem.Header.ToString());
				else
					MessageBox.Show("Nie można otworzyć wiecej niż jednego pliku!", "Notes", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
				//brak pliku?
				MessageBox.Show("Brak pliku!!", "Notes", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		private void Notes_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (edited && opened)
			   e.Cancel = !AskBeforeClose();
		}
		private void Formatuj_Click(object sender, RoutedEventArgs e)
		{
			if (opened)
			{
				MainTextBox.Text = Regex.Replace(MainTextBox.Text, "\r\n {4}", "\r\n\t");
				edited = true;
			}
		}

		private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			edited = true;
		}
	}
}
