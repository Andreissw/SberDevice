using SberDevice.Classes.Replace.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SberDevice.Interface
{
    public interface ISearch
    {
        Datas Datas { get; set; }        
        bool IsPacking();

        bool IsBunch(string FirstSN);
        InfoPacking GetInfoPacking();
    }
}
