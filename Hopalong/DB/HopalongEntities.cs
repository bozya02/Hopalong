using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopalong.DB
{
    public partial class HopalongEntities
    {
        private static HopalongEntities _context;

        public static HopalongEntities GetContext()
        {
            if (_context == null)
                _context = new HopalongEntities();

            return _context;
        }
    }
}
