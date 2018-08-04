using PayPal.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GratisForGratis.Models.ViewModels
{
    public class PayPalIndexViewModel/* : AcquistoViewModel*/
    {
        #region PROPRIETA
        [Required]
        public string Token { get; set; }

        [Required]
        public string Guid { get; set; }

        public string PayerID { get; set; }

        public AzionePayPal Azione { get; set;}
        #endregion

        #region COSTRUTTORI
        public PayPalIndexViewModel() { }

        public PayPalIndexViewModel(AcquistoViewModel viewModel, AnnuncioModel annuncio)
        {
            Token = viewModel.Token;

            if (viewModel.TipoScambio == TipoScambio.Spedizione && viewModel.TipoCarta != TipoCartaCredito.PayPal)
            {
                MetodoPagamento carta = new MetodoPagamento();
                carta.TipoCarta = viewModel.TipoCarta.ToString();
                carta.Numero = viewModel.NumeroCarta;
                carta.Cvv2 = ((int)viewModel.Cvv2).ToString();
                carta.Nome = viewModel.NomeTitolareCarta;
                carta.Cognome = viewModel.CognomeTitolareCarta;
                carta.MeseScadenza = (int)viewModel.MeseScadenzaCarta;
                carta.AnnoScadenza = (int)viewModel.AnnoScadenzaCarta;
            }
        }
        #endregion

        /*#region METODI PUBBLICI
        public void Copy<T>(T viewModel) where T : AcquistoViewModel
        {
            PropertyInfo[] properties = viewModel.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (this.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(viewModel));
                }
            }
        }
        #endregion*/
    }

    public class PayPalErroreViewModel/* : AcquistoViewModel*/
    {
        #region PROPRIETA
        [Required]
        public string Guid { get; set; }

        public string Messaggio { get; set; }

        public object Data { get; set; }
        #endregion

        /*#region METODI PUBBLICI
        public void Copy<T>(T viewModel) where T : AcquistoViewModel
        {
            PropertyInfo[] properties = viewModel.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (this.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(viewModel));
                }
            }
        }
        #endregion*/
    }
}