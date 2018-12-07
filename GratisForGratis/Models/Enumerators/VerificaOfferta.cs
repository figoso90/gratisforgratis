using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models.Enumerators
{
    public enum VerificaOfferta
    {
        Ok = 0,
        OffertaNonDisponibile = 1,
        CreditiNonSufficienti = 2,
        VenditoreNonAttivo = 3,
        CompratoreNonAttivo = 4,
        VerificaCartaDiCredito = 5,
        OffertaNonValida = 6
    }
}