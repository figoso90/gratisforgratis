using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public static class Utility
    {
        static private byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
        static private byte[] iv16Bit = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        private const string chiave = "AxTYQWCvGTFRbgLL"; //16 byte
        private const string iv = "QWExcfTyUxxLOafO"; //16 byte

        static Regex LineEnding = new Regex(@"(\r\n|\r|\n)+");


        public static string UlrEncode(string parametri)
        {
            return HttpUtility.UrlEncode(parametri.Replace(" ", "-").Replace("&", "").Replace("=", "").Replace("?", "").Replace("%", "").Replace("/", ""));
        }

        public static string GetValue(string key)
        {
            return HttpContext.GetGlobalResourceObject("lingua", key) as string;
        }

        public static void setCulture(string culture)
        {
            HttpContext.Current.Session["culture"] = culture;
        }

        // CREARE DA UN SERVIZIO WEB UNO USERCONTROL DA AGGIUNGERE IN PAGINA
        public static string RenderUserControl(string path, Object[] parametri)
        {
            List<Type> tipiParametri = new List<Type>();
            foreach (object parametro in parametri)
            {
                tipiParametri.Add(parametro.GetType());
            }
            Page pagina = new Page();
            Control userControl = pagina.LoadControl(path);
            // Find the relevant constructor
            ConstructorInfo costruttore = userControl.GetType().BaseType.GetConstructor(tipiParametri.ToArray());

            //And then call the relevant constructor
            if (costruttore == null)
            {
                throw new MemberAccessException("The requested constructor was not found on : " + userControl.GetType().BaseType.ToString());
            }
            else
            {
                costruttore.Invoke(userControl, parametri);
            }

            pagina.Controls.Add(userControl);
            using (StringWriter output = new StringWriter())
            {
                HttpContext.Current.Server.Execute(pagina, output, false);
                return output.ToString();
            }
        }

        public static bool verificaCellulare(string cellulare)
        {
            Regex pattern = new Regex(@"^([+]39)?((38[{8,9}|0])|(34[{7-9}|0])|(36[6|8|0])|(33[{3-9}|0])|(32[{8,9}]))([\d]{7})$");
            if (pattern.IsMatch(cellulare))
            {
                return true;
            }
            return false;
        }

        public static bool verificaTelefono(string telefono)
        {
            // verifica via più indirizzo civico
            Regex pattern = new Regex(@"^[a-zA-Z0-9&#192;&#193;&#194;&#195;&#196;&#197;&#198;&#199;&#200;&#201;&#202;&#203;&#204;&#205;&#206;&#207;&#208;&#209;&#210;&#211;&#212;&#213;&#214;&#216;&#217;&#218;&#219;&#220;&#221;&#223;&#224;&#225;&#226;&#227;&#228;&#229;&#230;&#231;&#232;&#233;&#234;&#235;&#236;&#237;&#238;&#239;&#241;&#242;&#243;&#244;&#245;&#246;&#248;&#249;&#250;&#251;&#252;&#253;&#255;\.\,\-\/\']+[a-zA-Z0-9&#192;&#193;&#194;&#195;&#196;&#197;&#198;&#199;&#200;&#201;&#202;&#203;&#204;&#205;&#206;&#207;&#208;&#209;&#210;&#211;&#212;&#213;&#214;&#216;&#217;&#218;&#219;&#220;&#221;&#223;&#224;&#225;&#226;&#227;&#228;&#229;&#230;&#231;&#232;&#233;&#234;&#235;&#236;&#237;&#238;&#239;&#241;&#242;&#243;&#244;&#245;&#246;&#248;&#249;&#250;&#251;&#252;&#253;&#255;\.\,\-\/\' ]+$");
            if (pattern.IsMatch(telefono))
            {
                return true;
            }
            return false;
        }

        public static bool verificaUrl(string url)
        {
            // verifica via più indirizzo civico
            Regex pattern = new Regex(@"^(http|https|ftp)\://(((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])|([a-zA-Z0-9_\-\.])+\.(com|net|org|edu|int|mil|gov|arpa|biz|aero|name|coop|info|pro|museum|uk|me))((:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*)$");
            if (pattern.IsMatch(url))
            {
                return true;
            }
            return false;
        }

        public static bool verificaIndirizzo(string indirizzo)
        {
            // verifica via più indirizzo civico
            Regex pattern = new Regex(@"^[a-zA-Z0-9&#192;&#193;&#194;&#195;&#196;&#197;&#198;&#199;&#200;&#201;&#202;&#203;&#204;&#205;&#206;&#207;&#208;&#209;&#210;&#211;&#212;&#213;&#214;&#216;&#217;&#218;&#219;&#220;&#221;&#223;&#224;&#225;&#226;&#227;&#228;&#229;&#230;&#231;&#232;&#233;&#234;&#235;&#236;&#237;&#238;&#239;&#241;&#242;&#243;&#244;&#245;&#246;&#248;&#249;&#250;&#251;&#252;&#253;&#255;\.\,\-\/\']+[a-zA-Z0-9&#192;&#193;&#194;&#195;&#196;&#197;&#198;&#199;&#200;&#201;&#202;&#203;&#204;&#205;&#206;&#207;&#208;&#209;&#210;&#211;&#212;&#213;&#214;&#216;&#217;&#218;&#219;&#220;&#221;&#223;&#224;&#225;&#226;&#227;&#228;&#229;&#230;&#231;&#232;&#233;&#234;&#235;&#236;&#237;&#238;&#239;&#241;&#242;&#243;&#244;&#245;&#246;&#248;&#249;&#250;&#251;&#252;&#253;&#255;\.\,\-\/\' ]+$");
            if (pattern.IsMatch(indirizzo))
            {
                return true;
            }
            return false;
        }

        public static MvcHtmlString Nl2br(String text, bool isXhtml = true)
        {
            var encodedText = HttpUtility.HtmlEncode(text);
            var replacement = isXhtml ? "<br />" : "<br>";
            return MvcHtmlString.Create(LineEnding.Replace(encodedText, replacement));
        }

        public static decimal cambioValuta(decimal? punti = 0, string valuta = null)
        {
            TIPO_VALUTA tipoValuta = (HttpContext.Current.Application["tipoValuta"] as List<TIPO_VALUTA>).SingleOrDefault(m => m.SIMBOLO == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol);
            valuta = tipoValuta.CODICE.ToUpper();
            if (string.IsNullOrWhiteSpace(valuta))
                valuta = "EUR";
            punti = (punti == null) ? 0 : punti;
            return (decimal)punti * Convert.ToDecimal(WebConfigurationManager.AppSettings["Conversione" + valuta.ToUpper()].ToString());
        }

        public static string RandomString(int size, bool lowerCase = false)
        {
            const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                //ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(CHARS[random.Next(CHARS.Length)]);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public static bool IsUtenteAttivo(int tipoAzione, TempDataDictionary tempData = null)
        {
            PersonaModel utente = (HttpContext.Current.Session["utente"] as PersonaModel);
            bool reindirizza = true;
            if (utente.Persona.STATO == (int)Stato.INATTIVO)
            {
                reindirizza = false;
                if (tempData != null)
                    tempData["completaRegistrazione"] = (tipoAzione == 0) ? Language.PubblicaAnnuncioCompletaRegistrazione : Language.AcquistaCompletaRegistrazione;
            }

            if (utente.Email.SingleOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione).STATO == (int)Stato.INATTIVO)
            {
                reindirizza = false;
                if (tempData != null)
                    tempData["confermaEmail"] = (tipoAzione == 0) ? Language.PubblicaAnnuncioConfermaEmail : Language.AcquistaConfermaEmail;
            }

            return reindirizza;
        }

        /*
        public static string RandomString2(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(System.Linq.Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }*/


        #region criptaggio con primo metodo
        public static string AesEncryption(string dataToEncrypt)
        {
            var bytes = Encoding.Default.GetBytes(dataToEncrypt);
            using (var aes = new AesCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                using (var encryptor = aes.CreateEncryptor(key, iv16Bit))
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    var cipher = ms.ToArray();
                    return Convert.ToBase64String(cipher);
                }
            }
        }

        public static string AesDecryption(string dataToDecrypt)
        {
            var bytes = Convert.FromBase64String(dataToDecrypt);
            using (var aes = new AesCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                using (var decryptor = aes.CreateDecryptor(key, iv16Bit))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                    var cipher = ms.ToArray();
                    return Encoding.UTF8.GetString(cipher);
                }
            }
        }
        #endregion criptaggio con primo metodo

        #region criptaggio con secondo metodo(passando una stringa)
        public static string Encode(string S)
        {
            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 128;
            rjm.BlockSize = 128;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            Byte[] input = Encoding.UTF8.GetBytes(S);
            Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0,
            input.Length);
            return Convert.ToBase64String(output);
        }

        public static string DecodeToString(string S)
        {
            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 128;
            rjm.BlockSize = 128;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            try
            {
                Byte[] input = Convert.FromBase64String(S);
                Byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0,
                input.Length);
                return Encoding.UTF8.GetString(output);
            }
            catch
            {
                return S;
            }
        }
        #endregion criptaggio con secondo metodo(passando una stringa)

        #region criptaggio con secondo metodo(passando un intero)
        public static string Encode(int interoDaCodificare)
        {
            String numeroConvertito = Convert.ToString(interoDaCodificare);
            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 128;
            rjm.BlockSize = 128;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            //Double temp = Convert.ToDouble(interoDaCodificare);
            Byte[] input = Encoding.UTF8.GetBytes(numeroConvertito);
            //Byte[] input = BitConverter.GetBytes(temp);
            Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0,
            input.Length);
            return Convert.ToBase64String(output);
        }

        public static int DecodeToInt(string stringaDaDecodificare)
        {
            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 128;
            rjm.BlockSize = 128;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            try
            {
                Byte[] input = Convert.FromBase64String(stringaDaDecodificare);
                Byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0,
                input.Length);
                string temp = Encoding.UTF8.GetString(output);
                return Convert.ToInt32(temp);
            }
            catch
            {
                return 0;
            }
        }
        #endregion criptaggio con secondo metodo(passando un intero)

        #region criptaggio con terzo metodo

        public static string Encrypt(string plainText, string passPhrase = chiave, string saltValue = iv, string hashAlgorithm = "MD5", int passwordIterations = 3, string initVector = "@1B2c3D4e5F6g7H8", int keySize = 256)
        {

            string cipherText = string.Empty;



            try
            {

                //Create byte arrays of our strings so that we can use them 

                //with the .NET Rijndael classes.

                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);

                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);



                PasswordDeriveBytes password = new PasswordDeriveBytes(

                    passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);



                RijndaelManaged symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;



                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes,

                    initVectorBytes);



                MemoryStream memoryStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memoryStream,

                    encryptor, CryptoStreamMode.Write);



                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);



                cryptoStream.FlushFinalBlock();



                byte[] cipherTextBytes = memoryStream.ToArray();



                memoryStream.Close();

                cryptoStream.Close();



                cipherText = Convert.ToBase64String(cipherTextBytes);



            }

            catch
            {

                cipherText = string.Empty;

            }



            return cipherText;

        }

        public static string Decrypt(string cipherText, string passPhrase = chiave, string saltValue = iv, string hashAlgorithm = "MD5", int passwordIterations = 3, string initVector = "@1B2c3D4e5F6g7H8", int keySize = 256)
        {

            string plainText = string.Empty;



            try

            {

                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);

                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);



                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);



                PasswordDeriveBytes password = new PasswordDeriveBytes(

                                                                passPhrase,

                                                                saltValueBytes,

                                                                hashAlgorithm,

                                                                passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);



                RijndaelManaged symmetricKey = new RijndaelManaged();



                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(

                                                             keyBytes,

                                                             initVectorBytes);



                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);



                CryptoStream cryptoStream = new CryptoStream(memoryStream,

                                                              decryptor,

                                                              CryptoStreamMode.Read);

                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes,

                                                               0,

                                                               plainTextBytes.Length);

                memoryStream.Close();

                cryptoStream.Close();



                plainText = Encoding.UTF8.GetString(plainTextBytes,

                                                               0,

                                                               decryptedByteCount);

            }

            catch (CryptographicException)

            {

                throw;

            }



            return plainText;

        }

        public static string EncryptId(int id)
        {
            return Uri.EscapeDataString(Encrypt(id.ToString()));
        }

        public static int DecryptId(string id)
        {
            return Convert.ToInt32(Decrypt(Uri.UnescapeDataString(id)));
        }

        public static Boolean CheckFormatoFile(HttpPostedFileBase file, Models.TipoMedia formato = Models.TipoMedia.FOTO)
        {
            if (file != null && file.ContentLength > 0)
            {
                string estensione = new FileInfo(Path.GetFileName(file.FileName)).Extension;
                string nomeFileUnivoco = System.Guid.NewGuid().ToString() + estensione;
                FileMedia media = Models.VerificaUpload.getIstanziaFile(estensione.ToLower());
                // formare stringa esadecimale
                string hexFile = string.Empty;
                Stream stream = file.InputStream;
                BinaryReader binary = new BinaryReader(stream);
                byte[] firmaFile = binary.ReadBytes(media.NumBytesID);
                foreach (byte B in firmaFile)
                {
                    hexFile += B.ToString("X2");
                }
                if (media != null && media.checkFormato(hexFile) && media.TipoFile == formato)
                {
                    //log.Debug("Tipo file verificato.");
                    return true;
                }
                else
                {
                    // lanciare eccezione file non corrispondente all'estensione
                    throw new InvalidDataException(App_GlobalResources.Language.ErrorFormatFile);
                }
            }
            return false;
        }

        #endregion

        #region CREARE ISTANZA SOCKET SERVER

        /*public static void createIstanzaSocket()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            Socket newsock = Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(localEndPoint);
            newsock.Listen(10);
            Socket client = newsock.Accept();
        }*/

        #endregion CREARE ISTANZA SOCKET SERVER
    }
}