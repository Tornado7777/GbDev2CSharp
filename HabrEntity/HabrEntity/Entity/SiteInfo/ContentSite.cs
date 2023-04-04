using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabrEntity.Entity
{
    internal class ContentSite
    {
        private int _SiteId { get; set; }
        private string _ContentName { get; set; }

        private UserSite _Author { get; set; }

        private DateTime _PublicationTime { get; set; }

        private string _Description { get; set; }

        private List<string> _Tags { get; set; }

        public ContentSite(int SiteId, string ContentName, UserSite Author, string Description, List<string> Tags)
        {
            _SiteId = SiteId;
            _ContentName = ContentName;
            _Author = Author;
            _Description = Description;
            _Tags= Tags;
            _PublicationTime = DateTime.Now;
        }
    }
}
