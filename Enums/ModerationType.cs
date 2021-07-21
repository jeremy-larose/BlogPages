using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProject.Enums
{
    public enum ModerationType
    {
        [Description("Political Propaganda")]
        Political,
        [Description("Offensive Language")]
        Language,
        [Description("Threatening Speech")]
        Threatening,
        [Description("Sexual Content")]
        Sexual,
        [Description("Drug References")]
        Drugs,
        [Description("Hate Speech")]
        HateSpeech,
        [Description("Shaming")]
        Shaming,
    }
}
