﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using PlantenApplicatie.Data;
using PlantenApplicatie.Domain.Models;
using PlantenApplicatie.UI.View;
using PlantenApplicatie.UI.ViewModel;
using Prism.Commands;

namespace PlantenApplicatie.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand<Window> schermGebruikerToevoegenCommand { get; set; }
        public RelayCommand<Window> CloseMainWindowCommand { get; set; }
        //ICommand om zoekresultaat leeg te maken
        public ICommand ClearResultCommand { get; set; }
        //Senne, Hermes
        public ICommand ZoekViaNaamCommand { get; set; }

        //Jelle & Hemen
        //Command om resultatenscherm op te roepen
        public RelayCommand<Window> ResultaatSchermCommand { get; set; }

        //boolean die ervoor zorgt dat de geselecteerde filters niet leeggemaakt worden
        private bool _loadCheck;

        private PlantenDataService _plantenDataService;
        private Plant _selectedPlant;

        //lists met alle tfgsv in
        private List<TfgsvType> _types;
        private List<TfgsvFamilie> _families;
        private List<TfgsvGeslacht> _geslachten;
        private List<TfgsvSoort> _soorten;
        private List<TfgsvVariant> _varianten;

        private List<Plant> _allPlants;

        //Lijst met alle planten
        private List<Plant> _plantResults;

        //list voor de binding
        public ObservableCollection<TfgsvType> TfgsvTypes { get; set; }
        public ObservableCollection<TfgsvFamilie> TfgsvFamilie { get; set; }
        public ObservableCollection<TfgsvGeslacht> TfgsvGeslacht { get; set; }
        public ObservableCollection<TfgsvSoort> TfgsvSoort { get; set; }
        public ObservableCollection<TfgsvVariant> TfgsvVariant { get; set; }
        public ObservableCollection<Plant> PlantResults { get; set; }

        //geselecteerde filters
        private TfgsvType _selectedType;
        private TfgsvFamilie _selectedFamilie;
        private TfgsvGeslacht _selectedGeslacht;
        private TfgsvSoort _selectedSoort;
        private TfgsvVariant _selectedVariant;

        //Jelle & Hemen
        //Extra lijsten voor gefilterde planten, dit wordt later gebruikt in de functie "ListResults"
        List<Plant> _plantTypeResults;
        List<Plant> _plantFamilieResults;
        List<Plant> _plantGeslachtResults;
        List<Plant> _plantSoortResults;
        //List<Plant> plantVariantResults;

        private string _zoekViaNaamInput = "";

        public MainViewModel(PlantenDataService plantenDataService)
        {//DAO instantiëren
            //Senne, Hermes
            ZoekViaNaamCommand = new DelegateCommand(ZoekViaNaam);

            //Stephanie, Hermes
            ClearResultCommand = new DelegateCommand(ClearResult);

            ResultaatSchermCommand = new RelayCommand<Window>(this.ResultaatScherm);

            //Senne, Maarten, Hermes
            TfgsvTypes = new ObservableCollection<TfgsvType>();
            TfgsvFamilie = new ObservableCollection<TfgsvFamilie>();
            TfgsvGeslacht = new ObservableCollection<TfgsvGeslacht>();
            TfgsvSoort = new ObservableCollection<TfgsvSoort>();
            TfgsvVariant = new ObservableCollection<TfgsvVariant>();
            PlantResults = new ObservableCollection<Plant>();

            //Hemen en Maarten
            schermGebruikerToevoegenCommand = new RelayCommand<Window>(this.SchermGebruikerToevoegenCommand);
            CloseMainWindowCommand = new RelayCommand<Window>(this.CloseMainWindow);
            this._plantenDataService = plantenDataService;
        }

        private void CloseMainWindow(Window window)
        {
            LoginScherm loginscherm = new LoginScherm();
            window.Close();
            loginscherm.ShowDialog();
        }
       
        //Jelle & Hemen
        public Plant SelectedPlant
        {
            get { return _selectedPlant; }
            set
            {
                _selectedPlant = value;
                OnPropertyChanged();
            }
        }

        //Jelle
        //Maken van gebruiker
        public Gebruiker LoggedInGebruiker { get; set; }
        //Jelle
        //Maken van visibility om te linken via databinding met gui
        public Visibility RolButtonsVisibility { get; set; }

        //Jelle
        //functie om gebruiker info te geven om te gebruiken doorheen de viewmodel
        public void LoadLoggedInUser(Gebruiker gebruiker)
        {
            LoggedInGebruiker = gebruiker;
        }
        //Jelle
        //Functie voor de visibility van de speciale buttons die bij sommige rollen niet beschikbaar mogen zijn.
        public void EnableRolButtons()
        {
            switch (LoggedInGebruiker.Rol)
            {
                case "Gebruiker":
                    RolButtonsVisibility = Visibility.Hidden;
                    break;
                case "Data-collector":
                    RolButtonsVisibility = Visibility.Hidden;
                    break;
                case "Manager":
                    RolButtonsVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        //Jelle & Hemen
        public void ResultaatScherm(Window window)
        {
            if (_selectedPlant != null)
            {
                ResultatenWindow resultatenWindow = new ResultatenWindow(_selectedPlant, LoggedInGebruiker);
                window.Close();
                resultatenWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a plant first");
            }

        }

        public void LoadAll()
        {
            //Senne, Maarten, Hermes
            var selectedType = _selectedType;
            LoadTypes();
            SelectedType = selectedType;

            var selectedFamilie = _selectedFamilie;
            LoadFamilies();
            SelectedFamilie = selectedFamilie;

            var selectedGeslacht = _selectedGeslacht;
            LoadGeslachten();
            SelectedGeslacht = selectedGeslacht;

            var selectedSoort = _selectedSoort;
            LoadSoorten();
            SelectedSoort = selectedSoort;

            var selectedVariant = _selectedVariant;
            LoadVarianten();
            SelectedVariant = selectedVariant;
        }

        public void InitializeTfgsv()
        {
            //Senne, Maarten, Hermes
            _loadCheck = true;

            _types = _plantenDataService.GetTfgsvTypes();
            _families = _plantenDataService.GetTfgsvFamilies();
            _geslachten = _plantenDataService.GetTfgsvGeslachten();
            _soorten = _plantenDataService.GetTfgsvSoorten();
            _varianten = _plantenDataService.GetTfgsvVarianten();

            _allPlants = _plantenDataService.GetAllPlants().OrderBy(p => p.Fgsv).ToList();

            _plantResults = _allPlants;
        }

        //Hermes & Stephanie
        public void ClearResult()
        {
            ClearFilters();
            ClearZoekViaNaam();
        }

        public void ClearFilters()
        {
            _selectedType = null;
            _selectedFamilie = null;
            _selectedGeslacht = null;
            _selectedSoort = null;
            _selectedVariant = null;

            PlantResults.Clear();

            //Alle waardes naar default zetten
            InitializeTfgsv();
            LoadAll();
        }
        public void ClearZoekViaNaam()
        {//Senne, Hermes
            ZoekViaNaamInput = "";
        }

        public void LoadTypes()
        {
            //Senne, Maarten, Hermes
            var types = _types;
            TfgsvTypes.Clear();
            foreach (var tfgsvType in types.OrderBy(p=>p.Planttypenaam))
            {
                TfgsvTypes.Add(tfgsvType);
            }
        }

        public void LoadFamilies()
        {
            //Senne, Maarten, Hermes
            var families = _families;
            TfgsvFamilie.Clear();
            foreach (var tfgsvFamily in families.OrderBy(p=>p.Familienaam))
            {
                TfgsvFamilie.Add(tfgsvFamily);
            }
        }

        public void LoadGeslachten()
        {
            //Senne, Maarten, Hermes
            var geslachten = _geslachten;
            TfgsvGeslacht.Clear();
            foreach (var tfgsvGeslacht in geslachten.OrderBy(p=>p.Geslachtnaam))
            {
                TfgsvGeslacht.Add(tfgsvGeslacht);
            }
        }

        public void LoadSoorten()
        {
            //Senne, Maarten, Hermes
            var soorten = _soorten;
            TfgsvSoort.Clear();
            foreach (var tfgsvSoort in soorten.OrderBy(p=>p.Soortnaam))
            {
                TfgsvSoort.Add(tfgsvSoort);
            }
        }

        public void LoadVarianten()
        {
            //Senne, Maarten, Hermes
            var varianten = _varianten;
            TfgsvVariant.Clear();
            foreach (var tfgsvVariant in varianten.OrderBy(p=>p.Variantnaam))
            {
                TfgsvVariant.Add(tfgsvVariant);
            }
        }
        //Jelle & Hemen
        //Functie om de planten eerst te ordenen en dan in de lijst toe te voegen
        public void LoadPlanten()
        {
            var plantResults = _plantResults;
            PlantResults.Clear();
            plantResults = plantResults.OrderBy(p => p.Fgsv).ToList();
            foreach (var plantResult in plantResults)
            {
                PlantResults.Add(plantResult);
            }
        }

        //selected values binding
        //Senne, Hermes, Maarten
        public TfgsvType SelectedType
        {
            //Senne, Maarten, Hermes
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                OnPropertyChanged();
                if (_selectedType != null && _loadCheck)
                {
                    ListResults("Type");
                    UpdateFilters("Type");
                }
            }
        }
        public TfgsvFamilie SelectedFamilie
        {
            //Senne, Maarten, Hermes
            get { return _selectedFamilie; }
            set
            {
                _selectedFamilie = value;
                OnPropertyChanged();
                if (_selectedFamilie != null && _loadCheck)
                {
                    ListResults("Familie");
                    UpdateFilters("Familie");
                }
            }
        }
        public TfgsvGeslacht SelectedGeslacht
        {
            //Senne, Maarten, Hermes
            get { return _selectedGeslacht; }
            set
            {
                _selectedGeslacht = value;
                OnPropertyChanged();
                if (_selectedGeslacht != null && _loadCheck)
                {
                    ListResults("Geslacht");
                    UpdateFilters("Geslacht");
                }
            }
        }
        public TfgsvSoort SelectedSoort
        {
            //Senne, Maarten, Hermes
            get { return _selectedSoort; }
            set
            {
                _selectedSoort = value;
                OnPropertyChanged();
                if (_selectedSoort != null && _loadCheck)
                {
                    ListResults("Soort");
                    UpdateFilters("Soort");
                }
            }
        }
        public TfgsvVariant SelectedVariant
        {
            //Senne, Maarten, Hermes
            get { return _selectedVariant; }
            set
            {
                _selectedVariant = value;
                OnPropertyChanged();
                if (_selectedVariant != null && _loadCheck)
                {
                    ListResults("Variant");
                    UpdateFilters("Variant");
                }
            }
        }

        public string ZoekViaNaamInput
        {//Senne, Hermes
            get { return _zoekViaNaamInput; }
            set
            {
                _zoekViaNaamInput = value;
                OnPropertyChanged();
            }
        }

        private void UpdateFilters(string typefilter)
        {
            //Senne, Maarten, Hermes

            ClearZoekViaNaam();

            //toont enkel de filters volgens gekozen bovenstaande filters
            switch (typefilter)
            {
                case "Type":
                    var fgsv = _plantenDataService.GetFilteredFamilies(_selectedType.Planttypeid);

                    SelectedFamilie = null;
                    SelectedGeslacht = null;
                    SelectedSoort = null;
                    SelectedVariant = null;

                    _families = (List<TfgsvFamilie>)fgsv[0];
                    _geslachten = (List<TfgsvGeslacht>)fgsv[1];
                    _soorten = (List<TfgsvSoort>)fgsv[2];
                    _varianten = (List<TfgsvVariant>)fgsv[3];
                    break;
                case "Familie":
                    var gsv = _plantenDataService.GetFilteredGeslachten(_selectedFamilie.FamileId);

                    SelectedGeslacht = null;
                    SelectedSoort = null;
                    SelectedVariant = null;

                    _geslachten = (List<TfgsvGeslacht>)gsv[0];
                    _soorten = (List<TfgsvSoort>)gsv[1];
                    _varianten = (List<TfgsvVariant>)gsv[2];
                    break;
                case "Geslacht":
                    var sv = _plantenDataService.GetFilteredSoorten(_selectedGeslacht.GeslachtId);

                    SelectedSoort = null;
                    SelectedVariant = null;

                    _soorten = (List<TfgsvSoort>)sv[0];
                    _varianten = (List<TfgsvVariant>)sv[1];
                    break;
                case "Soort":
                    SelectedVariant = null;

                    _varianten = _plantenDataService.GetFilteredVarianten(_selectedSoort.Soortid);
                    break;
            }

            //bool die ervoor zorgt dat er geen oneindige loop is
            _loadCheck = false;
            LoadAll();
            _loadCheck = true;
        }
        //Jelle & Hemen
        //Functie voor de lijsten te ordenenen
        private void ListResults(string typefilter)
        {
            //Switch voor te ordenenen welke combobox is aangepast (komt van Selected functies)
            switch (typefilter)
            {
                //Functie neemt de lijst juist boven de type om mee te filteren, als de type niet bestaat kijkt hij naar het type erboven totdat er een is die bestaat,
                //dit is voor te zorgen dat zo min mogelijk items gefilterd moeten worden
                case "Type":
                    _plantResults = _plantenDataService.GetPlantResults("Type", _selectedType.Planttypeid, _allPlants);
                    _plantTypeResults = _plantResults;
                    break;
                case "Familie":
                    if (_plantTypeResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Familie", _selectedFamilie.FamileId, _plantTypeResults);
                    }
                    else
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Familie", _selectedFamilie.FamileId, _allPlants);
                    }
                    _plantFamilieResults = _plantResults;
                    break;
                case "Geslacht":
                    if (_plantFamilieResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Geslacht", _selectedGeslacht.GeslachtId, _plantFamilieResults);
                    }
                    else if (_plantTypeResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Geslacht", _selectedGeslacht.GeslachtId, _plantTypeResults);
                    }
                    else
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Geslacht", _selectedGeslacht.GeslachtId, _allPlants);
                    }
                    _plantGeslachtResults = _plantResults;
                    break;
                case "Soort":
                    if (_plantGeslachtResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Soort", _selectedSoort.Soortid, _plantGeslachtResults);
                    }
                    else if (_plantFamilieResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Soort", _selectedSoort.Soortid, _plantFamilieResults);
                    }
                    else if (_plantTypeResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Soort", _selectedSoort.Soortid, _plantTypeResults);
                    }
                    else
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Soort", _selectedSoort.Soortid, _allPlants);
                    }
                    _plantSoortResults = _plantResults;
                    break;
                case "Variant":
                    if (_plantSoortResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Variant", _selectedVariant.VariantId, _plantSoortResults);
                    }
                    else if (_plantGeslachtResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Variant", _selectedVariant.VariantId, _plantGeslachtResults);
                    }
                    else if (_plantFamilieResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Variant", _selectedVariant.VariantId, _plantFamilieResults);
                    }
                    else if (_plantTypeResults != null)
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Variant", _selectedVariant.VariantId, _plantTypeResults);
                    }
                    else
                    {
                        _plantResults = _plantenDataService.GetPlantResults("Variant", _selectedVariant.VariantId, _allPlants);
                    }
                    //plantVariantResults = _plantResults;
                    break;
            }
            LoadPlanten();
        }
        private void ZoekViaNaam()
        {//Senne, Hermes
            //clear functie van de filters
            ClearFilters();

            PlantResults.Clear();

            _zoekViaNaamInput = _zoekViaNaamInput.Trim().ToLower();
            if (_zoekViaNaamInput != "")
            {
                foreach (Plant plant in _allPlants)
                {
                    if (plant.Fgsv.Trim().ToLower().Contains(_zoekViaNaamInput))
                    {
                        PlantResults.Add(plant);
                    }
                }
            }
        }
        //Hemen en Maarten
        public void SchermGebruikerToevoegenCommand(Window window)
        {
            GebruikersBeheer beheer = new GebruikersBeheer(LoggedInGebruiker);
            window.Close();
            beheer.ShowDialog();
        }
        
    }
}
