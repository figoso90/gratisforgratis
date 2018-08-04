using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    class Autocomplete
    {
        public int Value { get; set; }
        public string Label { get; set; }
    }

    class AutocompleteGuid
    {
        public Guid Value { get; set; }
        public string Label { get; set; }
    }

    class AutocompleteBaratto : AutocompleteGuid
    {
        public ANNUNCIO Annuncio
        {
            set {
                Label = value.NOME;
                Value = value.TOKEN;
                Prezzo = 0;
                ANNUNCIO_TIPO_SCAMBIO tipoScambio = value.ANNUNCIO_TIPO_SCAMBIO.FirstOrDefault(i => i.TIPO_SCAMBIO == (int)TipoScambio.Spedizione);
                if (tipoScambio != null)
                {
                    Prezzo = tipoScambio.ANNUNCIO_TIPO_SCAMBIO_SPEDIZIONE.FirstOrDefault().SOLDI;
                    Valuta = value.TIPO_VALUTA.SIMBOLO;
                }
            }
        }

        public decimal Prezzo { get; set; }

        public string Valuta { get; set; }
    }
}
