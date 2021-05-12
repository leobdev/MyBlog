using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Enums
{
    public enum ModerationType
    {
        [Display(Name = "Political propaganda")]
        PoliticalPropaganda,
        [Display(Name = "Offensive language")]
        OffensiveLanguage,
        [Display(Name = "Drug references")]
        Drugs,
        [Display(Name = "Threatening Comments")]
        Treatening,
        [Display(Name = "Sexual Content")]
        Sexual




    }
}
