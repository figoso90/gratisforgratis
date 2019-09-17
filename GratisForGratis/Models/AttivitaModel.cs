using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public class AttivitaModel : PERSONA_ATTIVITA
    {
        #region PROPRIETA

        public int Punti { get; set; }

        public int PuntiSospesi { get; set; }

        public ATTIVITA Attivita { get; set; }

        public List<ATTIVITA_EMAIL> Email { get; set; }

        public List<ATTIVITA_TELEFONO> Telefono { get; set; }

        public List<FotoModel> Foto { get; set; }

        #endregion

        #region COSTRUTTORI

        public AttivitaModel()
        {
            this.SetValoriBase();
        }

        public AttivitaModel(ATTIVITA model)
        {
            this.ATTIVITA = model;
            this.SetValoriBase();
        }

        public AttivitaModel(PERSONA_ATTIVITA model)
        {
            foreach (PropertyInfo prop in model.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(model, null), null);

            this.SetValoriBase();
        }

        #endregion

        #region METODI PRIVATI

        private void SetValoriBase()
        {
            this.Punti = 0;
            this.PuntiSospesi = 0;
            this.Email = new List<ATTIVITA_EMAIL>();
            this.Telefono = new List<ATTIVITA_TELEFONO>();
            this.Foto = new List<FotoModel>();
        }

        #endregion
    }
}
