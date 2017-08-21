﻿using System;
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
using Gabriel.Cat.Extension;
using PokemonGBAFrameWork;
using Microsoft.Win32;
using System.Drawing;
using Gabriel.Cat;

namespace TrainerEditorUniversal
{
	/// <summary>
	/// Lógica de interacción para MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		RomData rom;
		MenuItem itemActivarDesactivar;
		Script scriptActual;
		public MainWindow()
		{
			ContextMenu menu = new ContextMenu();
			MenuItem item;
			InitializeComponent();

			
			item = new MenuItem();
			item.Header = "Sobre";
			item.Click += (s, e) =>{
				if(MessageBox.Show("Este programa esta bajo licencia GNU, se ha hecho para facilitar la creación de scripts con el shinytzer créditos a Hackmew por la rutina, ¿Quieres ver el código fuente?","Sobre la app",MessageBoxButton.YesNo,MessageBoxImage.Information)==MessageBoxResult.Yes)
					System.Diagnostics.Process.Start("https://github.com/TetradogPokemonGBA/TrainerShinyUniversal");
				
			};
			menu.Items.Add(item);
			
			item = new MenuItem();
			item.Header = "Cambiar Rom";
			item.Click += (s, e) => PideRom();
			menu.Items.Add(item);

			itemActivarDesactivar = new MenuItem();
			itemActivarDesactivar.Header = "Activar";
			itemActivarDesactivar.Click += (s, e) => ActivarDesactivar();
			menu.Items.Add(itemActivarDesactivar);

			ContextMenu = menu;

			PideRom();
			if (rom == null)
				this.Close();
		}

		private void ActivarDesactivar()
		{
			if (Shinyzer.EstaActivado(rom.Rom))
				Shinyzer.Desactivar(rom.Rom,rom.Edicion,rom.Compilacion);
			else
				Shinyzer.Activar(rom.Rom,rom.Edicion,rom.Compilacion);
			Guardar();
			PonTexto();

		}

		private void PonTexto()
		{
			itemActivarDesactivar.Header = Shinyzer.EstaActivado(rom.Rom) ? "Desactivar" : "Activar";
		}

		private void PideRom()
		{
			OpenFileDialog opn = new OpenFileDialog();
			RomGba romGBA;
			EntrenadorPreview entrenador=null;
			opn.Filter = "GBA|*.gba";

			if (opn.ShowDialog().GetValueOrDefault())
			{

				romGBA = new RomGba(opn.FileName);
				
				try{
					rom = new RomData(romGBA);
					InicializaCampos();

					for (int i = 0; i < rom.Entrenadores.Count; i++)
					{
						entrenador = new EntrenadorPreview(i, rom.Entrenadores[i], rom.ClasesEntrenadores);
						entrenador.MouseLeftButtonUp +=(s,e)=> PonEntrenador(s as EntrenadorPreview);
						ugEntrenadores.Children.Add(entrenador);
						cmbEntrenadores.Items.Add(rom.Entrenadores[i]);

					}

					if (entrenador != null)
						PonEntrenador(entrenador);
					Title = "Universal Shiny Trainer:"+ rom.Rom.Nombre;
					if (!Shinyzer.EstaActivado(rom.Rom))
					{
						if (MessageBox.Show("No esta instalada la rutina Shinitzer de HackMew, quieres instalarla?", "Atención", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
						{ Shinyzer.Activar(rom.Rom,rom.Edicion,rom.Compilacion); Guardar(); }
					}
					PonTexto();
					stkObjetosEntrenador.Children.Clear();
					cmbEntrenadores.SelectedIndex=0;
					
				}catch{}

			}
		}
		private void Guardar()
		{
			rom.Rom.Save();
		}

		private void InicializaCampos()
		{
			ugEntrenadores.Children.Clear();
			ugEquipoEntrenador.Children.Clear();
			cmbEntrenadores.Items.Clear();
			txtScript.Text = "";
		}



		public void PonEntrenador(EntrenadorPreview entrenadorPreview)
		{
			PonEntrenador(entrenadorPreview.Entrenador);
		}
		public void PonEntrenador(Entrenador entrenador)
		{
			PokemonEntrenador pokemonEntrenador;
			System.Windows.Controls.Image img;
			txtNombreEntrenador.Text = entrenador.Nombre;
			txtNombreEntrenador.Foreground=entrenador.EsUnaEntrenadora?System.Windows.Media.Brushes.HotPink:System.Windows.Media.Brushes.Blue;
			if (entrenador.SpriteIndex < rom.ClasesEntrenadores.Count)
				imgEntrenador.SetImage(rom.ClasesEntrenadores[entrenador.SpriteIndex].Sprite);
			else imgEntrenador.SetImage(new Bitmap(16, 16));

			ugEquipoEntrenador.Children.Clear();
			for (int i = 0; i < entrenador.EquipoPokemon.Equipo.Length; i++)
				if (entrenador.EquipoPokemon.Equipo[i] != null)
			{
				pokemonEntrenador = new PokemonEntrenador(rom, entrenador.EquipoPokemon.Equipo[i],entrenador);
				pokemonEntrenador.ShinyChanged += (s, e) => GenerarScript(pokemonEntrenador.Entrenador);
				ugEquipoEntrenador.Children.Add(pokemonEntrenador);
			}
			
			if(rom.Edicion.AbreviacionRom!=AbreviacionCanon.AXP&&rom.Edicion.AbreviacionRom!=AbreviacionCanon.AXV){
				stkObjetosEntrenador.Children.Clear();
				if(entrenador.Item1>0){
					img=new System.Windows.Controls.Image();
					img.SetImage(rom.Objetos[entrenador.Item1].Sprite);
					stkObjetosEntrenador.Children.Add(img);
				}
				if(entrenador.Item2>0){
					img=new System.Windows.Controls.Image();
					img.SetImage(rom.Objetos[entrenador.Item2].Sprite);
					stkObjetosEntrenador.Children.Add(img);
				}
				if(entrenador.Item3>0){
					img=new System.Windows.Controls.Image();
					img.SetImage(rom.Objetos[entrenador.Item3].Sprite);
					stkObjetosEntrenador.Children.Add(img);
				}
				if(entrenador.Item4>0){
					img=new System.Windows.Controls.Image();
					img.SetImage(rom.Objetos[entrenador.Item4].Sprite);
					stkObjetosEntrenador.Children.Add(img);
				}}
			txtInteligencia.Text = "AI:" + entrenador.Inteligencia;
			txtMoneyClass.Text = rom.ClasesEntrenadores[entrenador.TrainerClass].Nombre;
			txtMusica.Text = "Musica:" + entrenador.MusicaBatalla;
			GenerarScript(entrenador);

		}

		private void GenerarScript(Entrenador entrenador)
		{
			const bool ACABAENEND=true;
			PokemonEntrenador[] pokemonEquipo = ugEquipoEntrenador.Children.OfType<PokemonEntrenador>().ToArray();
			bool[] isShiny = new bool[pokemonEquipo.Length];
			int offsetScript;
			byte[] bytesScript;
			for (int i = 0; i < isShiny.Length; i++)
				isShiny[i] = pokemonEquipo[i].IsShiny;
			
			scriptActual=Shinyzer.SimpleScriptBattleShinyTrainer(rom.Rom,rom.Entrenadores.IndexOf(entrenador),entrenador, isShiny);
			bytesScript=scriptActual.GetDeclaracion(rom.Rom,ACABAENEND);
			offsetScript=rom.Rom.Data.SearchArray(bytesScript);
			if(offsetScript>0){
				txtOffsetScript.Text=(Hex)offsetScript;
				btnInsertOrRemoveScript.Content="Quitar";
			}else{
				txtOffsetScript.Text="";
				btnInsertOrRemoveScript.Content="Insertar";
			}
			txtBinScript.Text=(Hex)bytesScript;
			txtScript.Text = scriptActual.GetDeclaracionXSE(ACABAENEND,"Entrenador"+entrenador.Nombre);
		}

		

		private void cmbEntrenadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbEntrenadores.SelectedItem != null)
				PonEntrenador(cmbEntrenadores.SelectedItem as Entrenador);
		}
		void BtnInsertOrRemoveScript_Click(object sender, RoutedEventArgs e)
		{
			const bool ACABAENEND=true;
			byte[] bytesScript=scriptActual.GetDeclaracion(rom.Rom,ACABAENEND);
			int offsetScript=rom.Rom.Data.SearchArray(bytesScript);
			if(offsetScript>0)
			{
				rom.Rom.Data.Remove(offsetScript,bytesScript.Length);
				btnInsertOrRemoveScript.Content="Insertar";
				txtOffsetScript.Text="";
			}
			else
			{
				
				btnInsertOrRemoveScript.Content="Quitar";
				offsetScript=rom.Rom.Data.SearchEmptyBytes(bytesScript.Length);
				rom.Rom.Data.SetArray(offsetScript,bytesScript);
				txtOffsetScript.Text=(Hex)offsetScript;
			}
			try{
			rom.Rom.Save();
			}catch{
				if(MessageBox.Show("No se ha podido guardar los datos,cierre cualquier otro programa que use esta rom y continua","Atención, imposible escribir en la ROM",MessageBoxButton.YesNo,MessageBoxImage.Error)==MessageBoxResult.Yes)
					try{
					rom.Rom.Save();
				}catch{
					MessageBox.Show("Mejor reinicia y repite de nuevo.","Continua igual...");
				}
			}
				
			   
		}
	}
}
