using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models.Enumerators
{
    public enum VerificaAcquisto
    {
        VerificaCartaFallita = -6,
        PagamentoHappyFallito = -5,
        PagamentoSoldiRealiFallito = -4,
        PagamentoSpedizioneFallito = -3,
        ErroreNonRiconosciuto = -2,
        Ok = -1,
        CompratoreNonAttivo = 0,
        VenditoreNonAttivo = 1,
        CreditiNonSufficienti = 2,
        AnnuncioNonDisponibile = 3,
        SpedizioneDaPagare = 4,
        PagamentoSoldiReali = 5,
        AnnuncioPersonale = 6,
        VerificaCartaCredito = 7
    }
}
