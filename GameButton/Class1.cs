using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameButton
{
    public class GameButton : Button
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
