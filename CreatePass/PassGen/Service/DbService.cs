using CreatePass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatePass.Service
{
    public class DbService
    {
        public DbService()
        {

        }

        public List<SiteKeyItem> GetSitekeys()
        {
            var sitekeys = new List<SiteKeyItem>();
            using (var db = new CreatePassContext())
            {
                sitekeys = db.SiteKeys.ToList();
            }
            return sitekeys;
        }

        public SiteKeyItem GetSiteKey(int sitekeyId)
        {
            var sitekey = new SiteKeyItem();
            using (var db = new CreatePassContext())
            {
                sitekey = db.SiteKeys.FirstOrDefault(x => x.SiteKeyItemId == sitekeyId);
            }
            return sitekey;
        }

        public async void AddSiteKey(SiteKeyItem sitekey)
        {
            using (var db = new CreatePassContext())
            {
                db.SiteKeys.Add(sitekey);
                await db.SaveChangesAsync();
            }
        }

        public async void DeleteSiteKey(int sitekeyId)
        {
            var sitekey = new SiteKeyItem();
            using (var db = new CreatePassContext())
            {
                sitekey = db.SiteKeys.FirstOrDefault(x => x.SiteKeyItemId == sitekeyId);

                db.SiteKeys.Remove(sitekey);
                await db.SaveChangesAsync();
            }
        }

        public async void DeleteSiteKey(SiteKeyItem sitekey)
        {
            using (var db = new CreatePassContext())
            {
                sitekey = db.SiteKeys.FirstOrDefault(x => x.SiteKeyItemId == sitekey.SiteKeyItemId);

                db.SiteKeys.Remove(sitekey);
                await db.SaveChangesAsync();
            }
        }
    }
}
