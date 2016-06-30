using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatePass.Model
{
    public class SiteKeyItem
    {
        public int SiteKeyItemId { get; set; }

        public string Url_PlainText { get; set; }

        public string Url_Encrypted { get; set; }

        public string Url_Hashed { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
