﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PlantenApplicatie.Data;
using PlantenApplicatie.Domain.Models;

namespace PlantenApplicatie.Data
{ 
    //Data Access Object / DAO in singleton
    public class PlantenDataService
    {
        private static readonly PlantenDataService instance = new PlantenDataService();
        private Planten2021Context context;

        //Singleton patroon wordt hier aangemaakt
        public static PlantenDataService Instance()
        {
            return instance;
        }

        //Initalisatie van verbinding met de databank
        private PlantenDataService()
        {
            this.context = new Planten2021Context();
        }

        //Mainwindow
        public List<TfgsvType> GetTfgsvTypes()
        {
            return context.TfgsvType.ToList();
        }
        public List<TfgsvFamilie> GetTfgsvFamilies()
        {
            return context.TfgsvFamilie.ToList();
        }
        public List<TfgsvGeslacht> GetTfgsvGeslachten()
        {
            return context.TfgsvGeslacht.ToList();
        }
        public List<TfgsvSoort> GetTfgsvSoorten()
        {
            return context.TfgsvSoort.Where(t=>t.Soortnaam!=null).ToList();
        }
        public List<TfgsvVariant> GetTfgsvVarianten()
        {
            return context.TfgsvVariant.Where(t => t.Variantnaam != null).ToList();
        }

        //Hier worden aan de hand van TFGSV de juiste onderdelen gefilterd voor het zoekscherm
        public Object[] GetFilteredFamilies(long typeId)
        {
            //Senne, Hermes en Maarten
            Object[] fgsv = new object[4];
            fgsv[0] = context.TfgsvFamilie.Where(p => p.TypeTypeid == typeId).ToList();
            fgsv[1] = context.TfgsvGeslacht.Where(p => p.FamilieFamile.TypeTypeid == typeId).ToList();
            fgsv[2] = context.TfgsvSoort.Where(p => p.GeslachtGeslacht.FamilieFamile.TypeTypeid == typeId).ToList();

            //Bug variant heeft geen soort om te koppelen
            //context.TfgsvVariant.ToList();
            //Jelle & Stephanie
            //Boventstaande bug kwam doordat de databank geen relatie had gelegd tussen soort en variant, doordat dit is gefixt werkt dit nu wel
            fgsv[3] = context.TfgsvVariant.Where(p => p.SoortSoort.GeslachtGeslacht.FamilieFamile.TypeTypeid == typeId).ToList();

            return fgsv;
        }

        public Object[] GetFilteredGeslachten(long familieId)
        {
            //Senne, Hermes en Maarten
            Object[] gsv = new object[3];
            gsv[0] = context.TfgsvGeslacht.Where(p => p.FamilieFamileId == familieId).ToList();
            gsv[1] = context.TfgsvSoort.Where(p => p.GeslachtGeslacht.FamilieFamileId == familieId).ToList();

            //Bug variant heeft geen soort om te koppelen
            //context.TfgsvVariant.ToList();
            //Jelle & Stephanie
            //Boventstaande bug kwam doordat de databank geen relatie had gelegd tussen soort en variant, doordat dit is gefixt werkt dit nu wel
            gsv[2] = context.TfgsvVariant.Where(p => p.SoortSoort.GeslachtGeslacht.FamilieFamileId == familieId).ToList();

            return gsv;
        }

        public Object[] GetFilteredSoorten(long geslachtId)
        {
            //Senne, Hermes en Maarten
            Object[] sv = new object[2];
            sv[0] = context.TfgsvSoort.Where(p => p.GeslachtGeslachtId == geslachtId).ToList();

            //Bug variant heeft geen soort om te koppelen
            //context.TfgsvVariant.ToList();
            //Jelle & Stephanie
            //Boventstaande bug kwam doordat de databank geen relatie had gelegd tussen soort en variant, doordat dit is gefixt werkt dit nu wel
            sv[1] = context.TfgsvVariant.Where(p => p.SoortSoort.GeslachtGeslachtId == geslachtId).ToList();

            return sv;
        }

        public List<TfgsvVariant> GetFilteredVarianten(long soortId)
        {
            //Senne, Hermes en Maarten
            //Bug variant heeft geen soort om te koppelen
            //context.TfgsvVariant.ToList();
            //Jelle & Stephanie
            //Boventstaande bug kwam doordat de databank geen relatie had gelegd tussen soort en variant, doordat dit is gefixt werkt dit nu wel
            return context.TfgsvVariant.Where(p => p.SoortSoortid == soortId).ToList();
        }

        //Geeft alle planten
        public List<Plant> GetAllPlants()
        {
            return context.Plant.ToList();
        }

        //Jelle & Hemen
        //Functie filtert de planten
        public List<Plant> GetPlantResults(string type, long id, List<Plant> plantResults)
        {
            //Switch voor de juiste id te weten waarmee gefilterd moet worden
            switch (type)
            {
                case "Type":
                    return plantResults.Where(p => p.TypeId == id).ToList();
                case "Familie":
                    return plantResults.Where(p => p.FamilieId == id).ToList();
                case "Geslacht":
                    return plantResults.Where(p => p.GeslachtId == id).ToList();
                case "Soort":
                    return plantResults.Where(p => p.SoortId == id).ToList();
                case "Variant":
                    return plantResults.Where(p => p.VariantId == id).ToList();
                default:
                    return null;
            }
        }

        //Geeft de plant en al zijn informatie
        public Plant GetPlantWithId(long Id)
        {
            return context.Plant.SingleOrDefault(p => p.PlantId == Id);
        }
        //Geeft de plant en zijn uiterlijke kenmerken + habitus
        public Fenotype GetFenotype(long Id)
        {
            return context.Fenotype.FirstOrDefault(p => p.PlantId == Id);
        }
        //Geeft de plant en zijn behoeftes
        public Abiotiek GetAbiotiek(long Id)
        {
            return context.Abiotiek.SingleOrDefault(a => a.PlantId == Id);
        }

        public List<AbiotiekMulti> GetAbiotiekMulti(long Id)
        {
            return context.AbiotiekMulti.Where(a => a.PlantId == Id).ToList();
        }

        //Geeft de plant en zijn ontwikkelsnelheid & strategie
        public Commensalisme GetCommensalisme(long Id)
        {
            return context.Commensalisme.FirstOrDefault(c => c.PlantId == Id);
        }

        //Geeft de plant & zijn specifieke eigenschappen
        public ExtraEigenschap GetExtraEigenschap(long Id)
        {
            return context.ExtraEigenschap.SingleOrDefault(e => e.PlantId == Id);
        }
        //Geeft de plant en de het onnderhoud per maand
        /*public BeheerMaand GetBeheerMaand(long Id)
        {
            return context.BeheerMaand.SingleOrDefault(b => b.PlantId == Id);
        }*/
        //Jelle
        public List<BeheerMaand> GetBeheerMaand(long Id)
        {
            return context.BeheerMaand.Where(b => b.PlantId == Id).ToList(); 
        }


        //Stephanie & Jelle
        public List<FenotypeMulti> GetFenoMultiKleur(long Id)
        {
            return context.FenotypeMulti.Where(m => m.PlantId == Id).ToList();
        }
        //Stephanie & Jelle
        public List<AbiotiekMulti> GetAbioHabitats(long Id)
        {
            return context.AbiotiekMulti.Where(h => h.PlantId == Id).ToList();
        }

        //Editwindow

        //Filters
        public TfgsvType GetFilterType(int? plantId)
        {
            return context.TfgsvType.FirstOrDefault(f => f.Planttypeid == plantId);
        }

        public TfgsvFamilie GetFilterFamilie(int? plantId)
        {
            return context.TfgsvFamilie.FirstOrDefault(p => p.FamileId == plantId);
        }

        public TfgsvGeslacht GetFilterGeslacht(int? plantId)
        {
            return context.TfgsvGeslacht.FirstOrDefault(p => p.GeslachtId == plantId);
        }

        public TfgsvSoort GetFilterSoort(int? plantId)
        {
            return context.TfgsvSoort.FirstOrDefault(p => p.Soortid == plantId);
        }

        public TfgsvVariant GetFilterVariant(int? plantId)
        {
            return context.TfgsvVariant.FirstOrDefault(p => p.VariantId == plantId);
        }

        public string GetPlantdichtheidMax(long plantId)
        {
            if (context.Plant.FirstOrDefault(p => p.PlantId == plantId)==null)
            {
                return null;
            }
            return context.Plant.FirstOrDefault(p => p.PlantId == plantId).PlantdichtheidMax.ToString();
        }

        public string GetPlantdichtheidMin(long plantId)
        {
            if (context.Plant.FirstOrDefault(p => p.PlantId == plantId) == null)
            {
                return null;
            }
            return context.Plant.FirstOrDefault(p => p.PlantId == plantId).PlantdichtheidMin.ToString();
        }
        //Fenotype
        public string GetFenoBladgrootteTot(long plantId)
        {
            if (context.Fenotype.FirstOrDefault(f=>f.PlantId==plantId)==null)
            {
                return null;
            }

            return context.Fenotype.FirstOrDefault(f => f.PlantId == plantId).Bladgrootte.ToString();
        }
        public string GetFenoMaxBladHoogte(long id)
        {
            if (context.Fenotype.FirstOrDefault(f => f.PlantId == id) == null)
            {
                return null;
            }
            return context.Fenotype.FirstOrDefault(f => f.PlantId == id).MaxBladhoogte.ToString();
        }
        public string GetFenoMaxBloeiHoogte(long id)
        {
            if (context.Fenotype.FirstOrDefault(f => f.PlantId == id) == null)
            {
                return null;
            }
            return context.Fenotype.FirstOrDefault(f => f.PlantId == id).MaxBloeihoogte.ToString();
        }
        public string GetFenoMinBloeiHoogte(long id)
        {
            if (context.Fenotype.FirstOrDefault(f => f.PlantId == id) == null)
            {
                return null;
            }
            return context.Fenotype.FirstOrDefault(f => f.PlantId == id).MinBloeihoogte.ToString();
        }
        public FenotypeMulti GetFenotypeMulti(long id)
        {
            return context.FenotypeMulti.FirstOrDefault(f => f.PlantId == id);
        }
        public FenoMaand GetFenoMaxBladHoogteMaand(long id)
        {
            return context.FenoMaand.FirstOrDefault(f =>
                f.Maand == context.FenotypeMulti.Where(m => m.Eigenschap == "blad-max")
                    .FirstOrDefault(i => i.PlantId == id).Maand);
        }
        public FenoMaand GetFenoMaxBloeiHoogteMaand(long id)
        {
            return context.FenoMaand.FirstOrDefault(f =>
                f.Maand == context.FenotypeMulti.Where(m => m.Eigenschap == "bloei-max")
                    .FirstOrDefault(i => i.PlantId == id).Maand);
        }
        public FenoMaand GetFenoMinBloeiHoogteMaand(long id)
        {
            return context.FenoMaand.FirstOrDefault(f =>
                f.Maand == context.FenotypeMulti.Where(m => m.Eigenschap == "bloei-min")
                    .FirstOrDefault(i => i.PlantId == id).Maand);
        }
        public FenoKleur GetFenoBladKleur(long id)
        {
            return context.FenoKleur.FirstOrDefault(f =>
                f.NaamKleur == context.FenotypeMulti.Where(m => m.Eigenschap == "blad")
                    .FirstOrDefault(i => i.PlantId == id).Waarde);
        }
        public FenoMaand GetFenoBladMaand(long id)
        {
            return context.FenoMaand.FirstOrDefault(f =>
                f.Maand == context.FenotypeMulti.Where(m => m.Eigenschap == "blad")
                    .FirstOrDefault(i => i.PlantId == id).Maand);
        }
        public FenoKleur GetFenoBloeiKleur(long id)
        {
            return context.FenoKleur.FirstOrDefault(f =>
                f.NaamKleur == context.FenotypeMulti.Where(m => m.Eigenschap == "bloei")
                    .FirstOrDefault(i => i.PlantId == id).Waarde);
        }
        public FenoMaand GetFenoBloeiMaand(long id)
        {
            return context.FenoMaand.FirstOrDefault(f =>
                f.Maand == context.FenotypeMulti.Where(m => m.Eigenschap == "bloei")
                    .FirstOrDefault(i => i.PlantId == id).Maand);
        }
        public List<FenoBloeiwijze> GetFenoBloeiwijze()
        {
            return context.FenoBloeiwijze.ToList();
        }
        public List<FenoHabitus> GetFenoHabitus()
        {
            return context.FenoHabitus.OrderBy(f => f.Naam).ToList();
        }
        public List<FenoBladgrootte> GetFenoBladgrootte()
        {
            return context.FenoBladgrootte.ToList();
        }
        public List<FenoKleur> GetFenoKleur()
        {
            return context.FenoKleur.ToList();
        }
        public List<FenoMaand> GetFenoMaand()
        {
            return context.FenoMaand.ToList();
        }
        public List<FenoBladvorm> GetFenoBladvorm()
        {
            return context.FenoBladvorm.OrderBy(f => f.Vorm).ToList();
        }
        public List<FenoRatioBloeiBlad> GetFenoRatio()
        {
            return context.FenoRatioBloeiBlad.OrderBy(f => f.Waarde).ToList();
        }
        public List<FenoSpruitfenologie> GetFenoSpruit()
        {
            return context.FenoSpruitfenologie.OrderBy(f => f.Fenologie).ToList();
        }
        public List<FenoLevensvorm> GetFenoLevensvorm()
        {
            return context.FenoLevensvorm.OrderBy(f => f.Levensvorm).ToList();
        }

        //Abio
        public List<AbioBezonning> GetAbioBezonning()
        {
            return context.AbioBezonning.ToList();
        }
        public List<AbioGrondsoort> GetAbioGrondsoort()
        {
            return context.AbioGrondsoort.ToList();
        }
        public List<AbioVoedingsbehoefte> GetAbioVoedingsbehoefte()
        {
            return context.AbioVoedingsbehoefte.ToList();
        }
        public List<AbioVochtbehoefte> GetAbioVochtbehoefte()
        {
            return context.AbioVochtbehoefte.ToList();
        }
        public List<AbioReactieAntagonischeOmg> GetAbioReactieAntagonischeOmg()
        {
            return context.AbioReactieAntagonischeOmg.ToList();
        }
        public List<AbioHabitat> GetHabitats()
        {
            return context.AbioHabitat.ToList();
        }

        //Commersialisme

        public List<CommensalismeMulti> GetCommLevensvormFromPlant(long id)
        {
            return context.CommensalismeMulti.Where(m => m.Eigenschap == "Levensvorm")
                .Where(i => i.PlantId == id).ToList();
        }

        public List<CommensalismeMulti> GetCommSocialbiliteitFromPlant(long id)
        {
            return context.CommensalismeMulti.Where(m => m.Eigenschap == "Sociabiliteit").Where(i => i.PlantId == id)
                .ToList();
        }

        public List<Commensalisme> GetCommStrategieFromPlant(long id)
        {
            return context.Commensalisme.Where(c => c.PlantId == id).ToList();
        }
        public List<CommOntwikkelsnelheid> GetCommOntwikkelSnelheid()
        {
            return context.CommOntwikkelsnelheid.OrderBy(c => c.Snelheid).ToList();
        }
        public List<CommLevensvorm> GetCommLevensvorm()
        {
            return context.CommLevensvorm.OrderBy(c => c.Levensvorm).ToList();
        }
        public List<CommSocialbiliteit> GetCommSocialbiliteit()
        {
            return context.CommSocialbiliteit.ToList();
        }
        public List<CommStrategie> GetCommStrategie()
        {
            return context.CommStrategie.ToList();
        }

        public long NewCommId()
        {
            return context.Commensalisme.Max(c => c.Id) + 1;
        }
        //Extra Eigenschappen
        public List<ExtraNectarwaarde> GetExtraNectarwaarde()
        {
            return context.ExtraNectarwaarde.OrderBy(n => n.Waarde).ToList();
        }
        public List<ExtraPollenwaarde> GetExtraPollenwaarde()
        {
            return context.ExtraPollenwaarde.OrderBy(p => p.Waarde).ToList();
        }
        public ExtraNectarwaarde GetExtraNectarwaardeFromPlant(long id)
        {
            if (context.ExtraEigenschap.FirstOrDefault(x => x.PlantId == id).Nectarwaarde == null)
            {
                return null;
            }
            return context.ExtraNectarwaarde.FirstOrDefault(e =>
                e.Waarde == context.ExtraEigenschap.FirstOrDefault(x => x.PlantId == id).Nectarwaarde);
        }
        public ExtraPollenwaarde GetExtraPollenwaardeFromPlant(long id)
        {
            if (context.ExtraEigenschap.FirstOrDefault(x => x.PlantId == id).Pollenwaarde == null)
            {
                return null;
            }
            return context.ExtraPollenwaarde.FirstOrDefault(e =>
                e.Waarde == context.ExtraEigenschap.FirstOrDefault(x => x.PlantId == id).Pollenwaarde);
        }
        public bool GetExtraBijvriendelijk(long id)
        {
            if (context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Bijvriendelijke == null)
            {
                return false;
            }
            return (bool)context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Bijvriendelijke;
        }
        public bool GetExtraEetbaar(long id)
        {
            if (context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Eetbaar == null)
            {
                return false;
            }
            return (bool)context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Eetbaar;

        }
        public bool GetExtraVlindervriendelijk(long id)
        {
            if (context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Vlindervriendelijk == null)
            {
                return false;
            }
            return (bool)context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Vlindervriendelijk;
        }
        public bool GetExtraGeurend(long id)
        {
            if (context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Geurend == null)
            {
                return false;
            }
            return (bool)context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Geurend;
        }
        public bool GetExtraVorstgevoelig(long id)
        {
            if (context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Vorstgevoelig == null)
            {
                return false;
            }
            return (bool)context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Vorstgevoelig;
        }
        public bool GetExtraKruidgebruik(long id)
        {
            if (context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Kruidgebruik == null)
            {
                return false;
            }
            return (bool)context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == id).Kruidgebruik;
        }
        //Nieuwe beheersbehandeling
        public List<BeheerDaden> GetAllBeheerDaden()
        {
            return context.BeheerDaden.OrderBy(b => b.Beheerdaad).ToList();
        }
        public List<BeheerMaand> GetBeheerDadenFromPlant(long id)
        {
            //var beheerMaandFromPlant= context.BeheerMaand.Where(b => b.PlantId == id).ToList();
            //var allBeheerdaden = GetAllBeheerDaden();
            //List<BeheerDaden> beheerDaden = new List<BeheerDaden>();
            //foreach (var maand in beheerMaandFromPlant)
            //{
            //    beheerDaden.Add(allBeheerdaden.FirstOrDefault(b=>b.Beheerdaad==maand.Beheerdaad));
            //}

            //return beheerDaden;

            return context.BeheerMaand.Where(b => b.PlantId == id).ToList();
        }

        public string AddNewBeheerDaad(string beheerdaad)
        {
            string result = null;
            foreach (var beheerDaden in GetAllBeheerDaden())
            {
                if (beheerDaden.Beheerdaad.ToLower().Trim() == beheerdaad.ToLower().Trim())
                {
                    result = "Waarde bestaat al";
                    return result;
                }
            }
            int newId = context.BeheerDaden.Max(b => b.Id) + 1;
            context.BeheerDaden.Add(new BeheerDaden() { Id = newId, Beheerdaad = beheerdaad });
            context.SaveChanges();
            return result;
        }

        
        public string AddBeheerToPlant(long plantId, string beheerdaad, string omschrijving, bool jan, bool feb,
            bool mrt, bool apr, bool mei, bool jun, bool jul, bool aug, bool sept, bool okt, bool nov, bool dec,
            string frequentie, string m2u)
        {
            string result;
            foreach (var beheerMaand in context.BeheerMaand.Where(p => p.PlantId == plantId).ToList())
            {
                if (beheerMaand.Beheerdaad.ToLower().Trim() == beheerdaad.ToLower().Trim())
                {
                    result = "Waarde is al toegevoegd aan de plant";
                    return result;
                }
            }

            long newId = context.BeheerMaand.Max(b => b.Id) + 1;

            var beheermaand = new BeheerMaand()
            {
                PlantId = plantId,
                Beheerdaad = beheerdaad,
                Omschrijving = omschrijving,
                Jan = jan,
                Feb = feb,
                Mrt = mrt,
                Apr = apr,
                Mei = mei,
                Jun = jun,
                Jul = jul,
                Aug = aug,
                Sept = sept,
                Okt = okt,
                Nov = nov,
                Dec = dec,
                FrequentiePerJaar = int.Parse(frequentie),
                M2u = double.Parse(m2u)
            };
            context.BeheerMaand.Add(beheermaand);
            context.SaveChanges();
            result = "Beheermaand toegevoegd aan plant";
            return result;
        }

        //Bestaande beheersbehandeling aanpassen
        public void EditBeheerFromPlant(long plantId, string beheerdaad, string omschrijving, bool jan, bool feb,
            bool mrt, bool apr, bool mei, bool jun, bool jul, bool aug, bool sept, bool okt, bool nov, bool dec,
            string frequentie, string m2u)
        {
            var allBeheerDadenFromPlant = GetBeheerDadenFromPlant(plantId);
            var dbBeheerMaand = allBeheerDadenFromPlant.FirstOrDefault(b => b.Beheerdaad == beheerdaad);

            dbBeheerMaand.Jan = jan;
            dbBeheerMaand.Feb = feb;
            dbBeheerMaand.Mrt = mrt;
            dbBeheerMaand.Apr = apr;
            dbBeheerMaand.Mei = mei;
            dbBeheerMaand.Jun = jun;
            dbBeheerMaand.Jul = jul;
            dbBeheerMaand.Aug = aug;
            dbBeheerMaand.Sept = sept;
            dbBeheerMaand.Okt = okt;
            dbBeheerMaand.Nov = nov;
            dbBeheerMaand.Dec = dec;
            dbBeheerMaand.Omschrijving = omschrijving;
            dbBeheerMaand.FrequentiePerJaar = int.Parse(frequentie);
            dbBeheerMaand.M2u = double.Parse(m2u);

            context.SaveChanges();
        }

        public void DeleteBeheerFromPlant(BeheerMaand beheerMaand)
        {
            context.BeheerMaand.Remove(beheerMaand);
            context.SaveChanges();
        }

        //Opslaan
        public List<FenotypeMulti> EditPlantFenoMulti(long plantId, FenoMaand bladMaxMaand, FenoMaand bloeiMinMaand, FenoMaand bloeiMaxMaand, FenoMaand bladMaand,
            FenoKleur bladKleur, FenoMaand bloeiMaand, FenoKleur bloeiKleur)
        {
            long fenoId = context.FenotypeMulti.Max(f => f.Id) + 1;
            List<FenotypeMulti> resultList = new List<FenotypeMulti>();

            var dbBladMax = context.FenotypeMulti.Where(f => f.Eigenschap == "blad-max")
                .FirstOrDefault(f => f.PlantId == plantId);
            var dbBloeiMin = context.FenotypeMulti.Where(f => f.Eigenschap == "bloei-min")
                .FirstOrDefault(f => f.PlantId == plantId);
            var dbBloeiMax = context.FenotypeMulti.Where(f => f.Eigenschap == "bloei-max")
                .FirstOrDefault(f => f.PlantId == plantId);
            var dbBlad = context.FenotypeMulti.Where(f => f.Eigenschap == "blad")
                .FirstOrDefault(f => f.PlantId == plantId);
            var dbBloei = context.FenotypeMulti.Where(f => f.Eigenschap == "bloei")
                .FirstOrDefault(f => f.PlantId == plantId);
            if (bladMaxMaand==null)
            {
                bladMaxMaand = new FenoMaand() {Maand = null};
            }
            if (bloeiMinMaand == null)
            {
                bloeiMinMaand = new FenoMaand() { Maand = null };
            }
            if (bloeiMaxMaand == null)
            {
                bloeiMaxMaand = new FenoMaand() { Maand = null };
            }
            if (bladMaand == null)
            {
                bladMaand = new FenoMaand() { Maand = null };
            }
            if (bladKleur==null)
            {
                bladKleur = new FenoKleur() {NaamKleur = null};
            }
            if (bloeiMaand == null)
            {
                bloeiMaand = new FenoMaand() { Maand = null };
            }
            if (bloeiKleur == null)
            {
                bloeiKleur = new FenoKleur() { NaamKleur = null };
            }

            if (dbBladMax==null)
            {
                resultList.Add(new FenotypeMulti()
                    { Id = fenoId, PlantId = plantId, Eigenschap = "blad-max", Maand = bladMaxMaand.Maand });
                fenoId++;
            }
            else
            {
                dbBladMax.Maand = bladMaxMaand.Maand;
            }

            if (dbBloeiMin==null)
            {
                resultList.Add(new FenotypeMulti()
                    { Id = fenoId, PlantId = plantId, Eigenschap = "bloei-min", Maand = bloeiMinMaand.Maand });
                fenoId++;
            }
            else
            {
                dbBloeiMin.Maand = bloeiMinMaand.Maand;
            }

            if (dbBloeiMax==null)
            {
                resultList.Add(new FenotypeMulti()
                    { Id = fenoId, PlantId = plantId, Eigenschap = "bloei-max", Maand = bloeiMaxMaand.Maand });
                fenoId++;
            }
            else
            {
                dbBloeiMax.Maand = bloeiMaxMaand.Maand;

            }

            if (dbBlad==null)
            {
                resultList.Add(new FenotypeMulti()
                {
                    Id = fenoId,
                    PlantId = plantId,
                    Eigenschap = "blad",
                    Maand = bladMaand.Maand,
                    Waarde = bladKleur.NaamKleur
                });
                fenoId++;
            }
            else
            {
                dbBlad.Maand = bladMaand.Maand;
                dbBlad.Waarde = bladKleur.NaamKleur;
            }

            if (dbBloei==null)
            {
                resultList.Add(new FenotypeMulti()
                {
                    Id = fenoId,
                    PlantId = plantId,
                    Eigenschap = "bloei",
                    Maand = bloeiMaand.Maand,
                    Waarde = bloeiKleur.NaamKleur
                });
            }
            else
            {
                dbBloei.Maand = bloeiMaand.Maand;
                dbBloei.Waarde = bloeiKleur.NaamKleur;
            }

            return resultList;
        }

        public string EditPlantData(long plantId, Fenotype fenotype, List<FenotypeMulti> fenotypeMulti ,Abiotiek abiotiek,
            List<AbiotiekMulti> abiotiekMulti,
            List<Commensalisme> commensalisme, List<CommSocialbiliteit> commSocialbiliteit, List<CommLevensvorm> commLevensvorm, ExtraEigenschap extraEigenschap,
            TfgsvType type, TfgsvFamilie familie, TfgsvGeslacht geslacht,
            TfgsvSoort soort, TfgsvVariant variant, string plantDichtheidMin, string plantDichtheidMax)
        {
            string result = "Plant werd aangepast";
            try
            {
                //fenotype
                var dbFenotype = context.Fenotype.FirstOrDefault(f => f.PlantId == fenotype.PlantId);
                dbFenotype = fenotype;

                //abiotiek
                var dbAbiotiek = context.Abiotiek.FirstOrDefault(a => a.PlantId == abiotiek.PlantId);
                dbAbiotiek = abiotiek;
                
                context.AbiotiekMulti.RemoveRange(GetAbiotiekMulti(plantId));
                context.AbiotiekMulti.AddRange(abiotiekMulti);
                //commensalisme
                context.Commensalisme.RemoveRange(GetCommStrategieFromPlant(plantId));
                context.Commensalisme.AddRange(commensalisme);

                var dbSocial = context.CommensalismeMulti.Where(c => c.Eigenschap == "Sociabiliteit");
                var dbLeven = context.CommensalismeMulti.Where(c => c.Eigenschap == "Levensvorm");

                context.CommensalismeMulti.RemoveRange(dbSocial.Where(c => c.PlantId == plantId));
                context.CommensalismeMulti.RemoveRange(dbLeven.Where(c => c.PlantId == plantId));

                foreach (var socialbiliteit in commSocialbiliteit)
                {
                    context.CommensalismeMulti.Add(new CommensalismeMulti()
                        { PlantId = plantId, Eigenschap = "Sociabiliteit", Waarde = socialbiliteit.Sociabiliteit });
                }

                foreach (var levensvorm in commLevensvorm)
                {
                    context.CommensalismeMulti.Add(new CommensalismeMulti()
                        { PlantId = plantId, Eigenschap = "Levensvorm", Waarde = levensvorm.Levensvorm });
                }
                //extra eigenschappen
                var dbExtra = context.ExtraEigenschap.FirstOrDefault(e => e.PlantId == extraEigenschap.PlantId);
                dbExtra = extraEigenschap;

                //plant
                var dbPlant = GetPlantWithId(plantId);
                dbPlant.Type = type.Planttypenaam;
                dbPlant.Familie = familie.Familienaam;
                dbPlant.Geslacht = geslacht.Geslachtnaam;
                dbPlant.Soort = soort.Soortnaam;
                dbPlant.Variant = variant.Variantnaam;

                string fgsv = familie.Familienaam + " " + geslacht.Geslachtnaam;
                if (soort.Soortnaam != null)
                {
                    fgsv += " " + soort.Soortnaam;
                }

                if (variant.Variantnaam!=null)
                {
                    fgsv += " " + variant.Variantnaam;
                }
                dbPlant.Fgsv = fgsv;

                if (plantDichtheidMin==String.Empty)
                {
                    dbPlant.PlantdichtheidMin = null;
                }
                else
                {
                    dbPlant.PlantdichtheidMin = short.Parse(plantDichtheidMin);
                }
                if (plantDichtheidMax == String.Empty)
                {
                    dbPlant.PlantdichtheidMax = null;
                }
                else
                {
                    dbPlant.PlantdichtheidMax = short.Parse(plantDichtheidMax);
                }

                dbPlant.TypeId = (int?) type.Planttypeid;
                dbPlant.FamilieId = (int?) familie.FamileId;
                if (soort.Soortid==0)
                {
                    dbPlant.SoortId = null;
                }
                else
                {
                    dbPlant.SoortId = (int?) soort.Soortid;
                }

                if (variant.VariantId==0)
                {
                    dbPlant.VariantId = null;
                }
                else
                {
                    dbPlant.VariantId = (int?) variant.VariantId;
                }

                context.FenotypeMulti.AddRange(fenotypeMulti);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                result = e +"\nEr is een fout opgetreden tijdens het opslaan, gelieve contact op te nemen met de beheerders van Plantify";
            }
            
            return result;
        }

        public Fenotype OpslaanGetFenotype(long id)
        {
            if (GetFenotype(id)!=null)
            {
                return GetFenotype(id);
            }

            context.Fenotype.Add(new Fenotype() {PlantId = id});
            context.SaveChanges();
            return GetFenotype(id);
        }
        //Hemen &Maarten 
        public Gebruiker addGebruiker(string rol, string email, byte[] HashPaswoord,string voornaam , string achternaam, string vivesnr)
        {
            Gebruiker gebruiker = new Gebruiker
            {
                Rol = rol,
                Emailadres = email,
                HashPaswoord = HashPaswoord,
                Voornaam = voornaam,
                 Achternaam= achternaam ,
                 Vivesnr = vivesnr


            };
            context.Gebruiker.Add(gebruiker);
            context.SaveChanges();
            return gebruiker;
        }
        public List<Rol> GetRollen()
        {
            return context.Rol.ToList();
        }
        public Gebruiker getGebruikerViaEmail(string email)
        {
            return context.Gebruiker.SingleOrDefault(g => g.Emailadres == email);
        }
        public Foto getFotoViaPlantId(long Id)
        {
            return context.Foto.SingleOrDefault(f => f.Plant == Id);
        }
        public List<Gebruiker> GetAllUsers()
        {
            return context.Gebruiker.ToList();
        }
        public Gebruiker verwijderGebruiker(string email)
        {
            var gebruiker = getGebruikerViaEmail(email);
            context.Gebruiker.Remove(gebruiker);
            context.SaveChanges();
            return gebruiker;
        }
       

        //Jelle & Stephanie
        public List<CommensalismeMulti> GetCommMulti(long plantId)
        {
            return context.CommensalismeMulti.Where(p => p.PlantId == plantId).ToList();
        }

        public void ChangeGebruikerPassword(Gebruiker gebruiker, byte[] password)
        {
            gebruiker.HashPaswoord = password;
            context.SaveChanges();
        }
    }
}