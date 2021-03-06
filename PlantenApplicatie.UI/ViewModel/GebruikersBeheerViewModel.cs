﻿using GalaSoft.MvvmLight.Command;
using PlantenApplicatie.Data;
using PlantenApplicatie.Domain.Models;
using PlantenApplicatie.UI.View;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PlantenApplicatie.UI.ViewModel

{
    public class GebruikersBeheerViewModel : ViewModelBase
    {
        public ObservableCollection<Gebruiker> Users { get; set; }
        private PlantenDataService _plantenDataService;
        public ICommand GebruikerVerwijderenCommand { get; set; }
        public RelayCommand<Window> schermGebruikerToevoegenCommand { get; set; }
        public RelayCommand<Window> CloseGebruikersBeheerCommand { get; set; }
        private Gebruiker _selectedGebruiker;

        public GebruikersBeheerViewModel(PlantenDataService plantenDataService)
        {//DAO instantiëren
    

            //Hemen en Maarten
            schermGebruikerToevoegenCommand = new RelayCommand<Window>(this.SchermGebruikerToevoegenCommand);
            CloseGebruikersBeheerCommand = new RelayCommand<Window>(this.CloseGebruikersBeheer);
            GebruikerVerwijderenCommand = new DelegateCommand(VerwijderGebruiker);
            Users = new ObservableCollection<Gebruiker>();
            this._plantenDataService = plantenDataService;


        }
        public Gebruiker SelectedGebruiker
        {
            get { return _selectedGebruiker; }
            set
            {
                _selectedGebruiker = value;
                OnPropertyChanged();
            }
        }

        //Jelle
        //Maken van gebruiker
        public Gebruiker LoggedInGebruiker { get; set; }
        //Jelle
        //functie om gebruiker info te geven om te gebruiken doorheen de viewmodel
        public void LoadLoggedInUser(Gebruiker gebruiker)
        {
            LoggedInGebruiker = gebruiker;
        }

        private void VerwijderGebruiker()
        {
            if (SelectedGebruiker != null)
            {
                if (SelectedGebruiker != LoggedInGebruiker)
                {
                    var gebruiker = _plantenDataService.verwijderGebruiker(SelectedGebruiker.Emailadres);
                    ShowAllUser();
                }
                else
                {
                    MessageBox.Show("Je mag jezelf niet verwijderen!");
                }
            }
        }

        private void CloseGebruikersBeheer(Window window)
        {
            MainWindow main = new MainWindow(LoggedInGebruiker);
            window.Close();
            main.ShowDialog();
        }

        //Hemen en Maarten
        private void SchermGebruikerToevoegenCommand(Window window)
        {
            CreateGebruiker create = new CreateGebruiker(LoggedInGebruiker);
            
            window.Close();
            create.ShowDialog();
            
        }
        public  void ShowAllUser()
        {
            Users.Clear();
            var Gebruikers = _plantenDataService.GetAllUsers();
            foreach (var gebruiker in Gebruikers)
            {
                Users.Add(gebruiker);
            }

        }

    }
}