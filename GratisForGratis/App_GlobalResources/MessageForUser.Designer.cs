﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GratisForGratis.App_GlobalResources {
    using System;
    
    
    /// <summary>
    ///   Classe di risorse fortemente tipizzata per la ricerca di stringhe localizzate e così via.
    /// </summary>
    // Questa classe è stata generata automaticamente dalla classe StronglyTypedResourceBuilder.
    // tramite uno strumento quale ResGen o Visual Studio.
    // Per aggiungere o rimuovere un membro, modificare il file con estensione ResX ed eseguire nuovamente ResGen
    // con l'opzione /str oppure ricompilare il progetto VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class MessageForUser {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MessageForUser() {
        }
        
        /// <summary>
        ///   Restituisce l'istanza di ResourceManager nella cache utilizzata da questa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GratisForGratis.App_GlobalResources.MessageForUser", typeof(MessageForUser).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Esegue l'override della proprietà CurrentUICulture del thread corrente per tutte le
        ///   ricerche di risorse eseguite utilizzando questa classe di risorse fortemente tipizzata.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Finisci la tua registrazione alla pagina {0} per poter acquistare e vendere. Riceverai ulteriori crediti omaggio!.
        /// </summary>
        public static string MessaggioCompletaRegistrazione {
            get {
                return ResourceManager.GetString("MessaggioCompletaRegistrazione", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Conferma l&apos;e-mail di registrazione. Clicca il seguente link per ricevere nuovamente la MAIL con il LINK di ATTIVAZIONE!.
        /// </summary>
        public static string MessaggioConfermaEmail {
            get {
                return ResourceManager.GetString("MessaggioConfermaEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a Iscriviti subito, in omaggio {0} {1} con cui potrai acquistare quello che vorrai! {2} &lt;br /&gt;Spedizioni dal nostro portale, possibilità di acquistare servizi professionali e oggetti nuovi, baratto suggerito!.
        /// </summary>
        public static string MessaggioPromozione {
            get {
                return ResourceManager.GetString("MessaggioPromozione", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cerca una stringa localizzata simile a &lt;strong&gt;Non hai trovato&lt;/strong&gt; ciò che cercavi?.
        /// </summary>
        public static string SalvaRicerca {
            get {
                return ResourceManager.GetString("SalvaRicerca", resourceCulture);
            }
        }
    }
}
