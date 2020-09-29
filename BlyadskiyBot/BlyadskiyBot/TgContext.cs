using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlyadskiyBot
{
    class TgContext:DbContext
    {
        public DbSet<MoneyUser> MoneyUsers { get; set; }
    }
    class Repository
    {
        static TgContext context = new TgContext();
        public List<MoneyUser> MoneyUsers { get { return context.MoneyUsers.ToList(); } }

        public void InsertUser(MoneyUser user)
        {
            context.MoneyUsers.Add(user);
            context.SaveChanges();
        }
        public void SaveCurrency(string cur,long tgid)
        {
            context.MoneyUsers.Where(tg => tg.TgId == tgid).FirstOrDefault().Currencies = cur;
            context.SaveChanges();
        }
        public void SaveSeed(int seed, long tgid)
        {
            context.MoneyUsers.Where(tg => tg.TgId == tgid).FirstOrDefault().Seed = seed;
            context.SaveChanges();
        }
    }
    class MoneyUser
    {
        public int MoneyUserId { get; set; }
        public long TgId { get; set; }

        public int Seed { get; set; }

        public string Currencies { get; set; }
    }
}
