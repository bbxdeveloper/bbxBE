using System.ComponentModel;

namespace bbxBE.Common.Enums
{
    public enum enVatExemption
    {
        //Alanyi adómentes
        [Description("AAM")]
        AAM,

        //"tárgyi adómentes" ill. a tevékenység közérdekű vagy speciális jellegére tekintettel adóme
        [Description("TAM")]
        TAM,

        //KBAET adómentes Közösségen belüli termékértékesítés, új közlekedési eszköz nélkül
        [Description("KBAET")]
        KBAET,


        //KBAUK adómentes Közösségen belüli új közlekedési eszköz értékesítés 
        [Description("KBAUK")]
        KBAUK,

        //EAM adómentes termékértékesítés a Közösség területénNAV Online Számla rendszer 120. oldal kívülre(termékexport harmadik országba)
        [Description("EAM")]
        EAM,


        // NAM egyéb nemzetközi ügyletekhez kapcsolódó jogcímen megállapított adómentesség
        [Description("NAM")]
        NAM,


        //UNKNOWN 3.0 előtti számlára hivatkozó, illetve előzmény nélküli módosító és sztornó számlák esetén használható, ha nem megállapítható az érték.
        [Description("UNKNOWN")]
        UNKNOWN

    }


}
