using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbxBE.Common.NAV
{
    public class NAV_enums
    {

        public enum enTransactionStatus
        {
            [Description("Létrehozva")]
            CREATED,

            //[Description("számla adatszolgáltatás befogadásra került")]
            [Description("Befogadva")]
            RECEIVED,

            //[Description("számla adatszolgáltatás feldolgozása megkezdődött")]
            [Description("Feldolgozás alatt")]
            PROCESSING,

            [Description("Nincs blokkoló hiba, mentve")]
            SAVED,

            [Description("Elküldve")]       //2.0-ban nem használt
            SENT,

            //[Description("számla adatszolgáltatás feldolgozása sikeresen befejeződött")]
            [Description("Rendben")]
            DONE,

            //[Description("számla adatszolgáltatás feldolgozása sikertelen volt")]
            [Description("Hiba")]
            ABORTED,

            [Description("Hiba")]
            ERROR,

            [Description("**Ismeretlen**")]
            UNKOWN
        }

        public enum enCustUnitOfMeasureType
        {
            [Description("Nincs me")]
            NONE,
            [Description("Darab")]
            PIECE,
            [Description("Kilogramm")]
            KILOGRAM,
            [Description("Tonna")]
            TON,
            [Description("Kilowatt óra")]
            KWH,
            [Description("Nap")]
            DAY,
            [Description("Óra")]
            HOUR,
            [Description("Perc")]
            MINUTE,
            [Description("Hónap")]
            MONTH,
            [Description("Liter")]
            LITER,
            [Description("Kilométer")]
            KILOMETER,
            [Description("Köbméter")]
            CUBIC_METER,
            [Description("Méter")]
            METER,
            [Description("Folyóméter")]
            LINEAR_METER,
            [Description("Karton")]
            CARTON,
            [Description("Doboz")]
            PACK,
            [Description("Saját")]
            OWN
        }

        public enum enCustlineNatureIndicatorType
        {
            [Description("Termék")]
            PRODUCT,
            [Description("Szolgáltatás")]
            SERVICE,
            [Description("Egyéb")]
            OTHER,
            [Description("Nem meghatározott")]
            NOTDEFINED
        }

        public enum enCustAnnulmentVerificationStatus
        {
            [Description("A technikai érvénytelenítés kliens hiba miatt nem hagyható jóvá")]
            NOT_VERIFIABLE,
            [Description("A technikai érvénytelenítés jóváhagyásra vár")]
            VERIFICATION_PENDING,
            [Description("A technikai érvénytelenítés jóváhagyásra került")]
            VERIFICATION_DONE,
            [Description("A technikai érvénytelenítés elutasításra került")]
            VERIFICATION_REJECTED
        }

        public enum enCustAnnulmentOperation
        {
            [Description("ANNUL")]
            ANNUL
        }
        public enum enCustproductStream
        {
            [Description("akkumulátor")]
            BATTERY,
            [Description("csomagolószer")]
            PACKAGING,
            [Description("egyéb kőolajtermék")]
            OTHER_PETROL,
            [Description("az elektromos, elektronikai berendezés")]
            ELECTRONIC,
            [Description("gumiabroncs")]
            TIRE,
            [Description("reklámhordozó papír")]
            COMMERCIAL,
            [Description("egyéb műanyag termék")]
            PLASTIC,
            [Description("egyéb vegyipari termék")]
            OTHER_CHEMICAL,
            [Description("irodai papír")]
            PAPER
        }
        public enum enCustannulmentCode
        {
            [Description("Hibás adattartalom miatti technikai érvénytelenítés")]
            ERRATIC_DATA,
            [Description("Hibás számlaszám miatti technikai érvénytelenítés")]
            ERRATIC_INVOICE_NUMBER,
            [Description("Hibás számla kelte miatti technikai érvénytelenítés")]
            ERRATIC_INVOICE_ISSUE_DATE
        }

        public enum enInvoiceCategory
        {
            [Description("Normál")]
            NORMAL,
            [Description("Egyszerűsített")]
            SIMPLIFIED,
            [Description("Gyűjtő")]
            AGGREGATE
        }

        public enum enCustinvoiceAppearance
        {
            [Description("Papír alapú")]
            PAPER,
            [Description("Elektronikus, nem EDI")]
            ELECTRONIC,
            [Description("EDI")]
            EDI,
            [Description("nem ismert")]
            UNKNOWN
        }

        public enum enCustlineOperation
        {
            [Description("CREATE")]
            CREATE,
            [Description("MODIFY")]
            MODIFY
        }
        public enum enCustproductCodeCategory
        {
            [Description("Vámtarifa szám VTSZ")]
            VTSZ,
            [Description("Szolgáltatás jegyzék szám SZJ")]
            SZJ,
            [Description("KN kód (Kombinált Nómenklatúra, 2658/87/EGK rendelet I.melléklete)")]
            KN,
            [Description("A Jövedéki törvény (2016. évi LXVIII. tv) szerinti eTKO adminisztratív hivatkozási kódja AHK")]
            AHK,
            [Description("A termék 343/2011. (XII. 29) Korm. rendelet 1. sz. melléklet A) cím szerinti csomagolószer-katalógus kódja(CsK kód)")]
            CSK,
            [Description("A termék 343/2011. (XII. 29) Korm. rendelet 1. sz. melléklet B) cím szerinti környezetvédelmi termékkódja(Kt kód)")]
            KT,
            [Description("Építményjegyzék szám")]
            EJ,
            [Description("A vállalkozás által képzett termékkód")]
            OWN,
            [Description("Egyéb")]
            OTHER,
            [Description("EAN")]            //ezt én tettem bele. Az EAN-t nem szabad NAV felé küldeni!
            EAN
        }

        public enum enCustproductFeeOperation
        {
            [Description("REFUND")]
            REFUND,
            [Description("DEPOSIT")]
            DEPOSIT,
            [Description("Gyűjtő")]
            AGGREGATE
        }


        public enum enCusttakeoverReason
        {

            [Description("01")]
            x01,
            [Description("02_aa")]
            x02_aa,
            [Description("02_ab")]
            x02_ab,
            [Description("02_b")]
            x02_b,
            [Description("02_c")]
            x02_c,
            [Description("_aa")]
            x02_d,
            [Description("02_ea")]
            x02_ea,
            [Description(" 02_eb")]
            x02_eb,
            [Description("02_fa")]
            x02_fa,
            [Description("02_fb")]
            x02_fb,
            [Description("02_ga")]
            x02_ga,
            [Description("02_gb")]
            x02_gb
        }

        public enum enCustinvoiceDirection
        {
            [Description("Kimenő")]
            OUTBOUND,
            [Description("Bejövő")]
            INBOUND
        }
        public enum enCustInvoiceOperation
        {
            [Description("CREATE")]
            CREATE,
            [Description("MODIFY")]
            MODIFY,
            [Description("STORNO")]     //nem használjuk, helyette van az ANNUL
            STORNO
        }

        public enum enCustTransactionStatus
        {
            [Description("Létrehozva")]
            CREATED,

            //[Description("számla adatszolgáltatás befogadásra került")]
            [Description("Befogadva")]
            RECEIVED,

            //[Description("számla adatszolgáltatás feldolgozása megkezdődött")]
            [Description("Feldolgozás alatt")]
            PROCESSING,

            [Description("Nincs blokkoló hiba, mentve")]
            SAVED,

            [Description("Elküldve")]       //2.0-ban nem használt
            SENT,

            //[Description("számla adatszolgáltatás feldolgozása sikeresen befejeződött")]
            [Description("Rendben")]
            DONE,

            //[Description("számla adatszolgáltatás feldolgozása sikertelen volt")]
            [Description("Hiba")]
            ABORTED,

            [Description("Hiba")]
            ERROR,

            [Description("**Ismeretlen**")]
            UNKOWN
        }

    }
}
