using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlyadskiyBot
{
    class TgContext : DbContext
    {
        public DbSet<MoneyUser> MoneyUsers { get; set; }
    }
    public  class Repository
    {
        object locker = new object();
         TgContext context = new TgContext();
        public  List<MoneyUser> MoneyUsers { get  { return context.MoneyUsers.ToList();  } }

        public  void InsertUser(MoneyUser user)
        {
            context.MoneyUsers.Add(user);
            context.SaveChangesAsync();
        }
        public  void SaveCurrency(string cur, long tgid)
        {
            context.MoneyUsers.Where(tg => tg.TgId == tgid).FirstOrDefault().Currencies = cur;
            context.SaveChangesAsync();
        }
        public  void SaveSeed(int seed, long tgid)
        {
            context.MoneyUsers.Where(tg => tg.TgId == tgid).FirstOrDefault().Seed = seed;
            context.SaveChangesAsync();
        }
        public async void SaveDate(long tgid, DateTime time)
        {
            object o = new object();
            lock (o)
            {
                context.MoneyUsers.Where(tg => tg.TgId == tgid).FirstOrDefault().LastUpdate = time;
            }
            await context.SaveChangesAsync();
        }
    }
    public class MoneyUser
    {
        public int MoneyUserId { get; set; }
        public long TgId { get; set; }

        public int Seed { get; set; }

        public string Currencies { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
