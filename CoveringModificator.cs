using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetIntervals.Base;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TargetIntervals
{
    public enum GraphicType
    { 
        Free,           //Произвольный график начисления ОД и %%
        FinalPay,       //Выплата в конце
        Different,      //Выплаты ОД равными долями
        Annuitet,       //Аннуитетный график выплат
        AnnuitetRecalc  //Аннуитетный график выплат, с пересчетом
    }

    
}
