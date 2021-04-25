using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZespolLib;

namespace ZespolServer.Controllers
{
    [Route("Zespol")]
    [ApiController]
    public class ZespolController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ZespolController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddZespol(Zespol Zespol)
        {
            List<Osoba> Osoby = new List<Osoba>(await dbContext.CzlonkowieZespolow.ToListAsync());
            foreach (Osoba KierownikZespolu in await dbContext.KierownicyZespolow.ToListAsync())
            {
                Osoby.Add(KierownikZespolu);
            }

            List<Osoba> NoweOsoby = new List<Osoba> {Zespol.Kierownik};
            foreach (var osoba in Zespol.Czlonkowie)
            {
                NoweOsoby.Add(osoba);
            }

            foreach (var osoba in NoweOsoby)
            {
                Osoba StaraOsoba = Osoby.FirstOrDefault(os => os.PESEL == osoba.PESEL);

                if (StaraOsoba != default(Osoba))
                {
                    if (Zespol.Kierownik.PESEL == StaraOsoba.PESEL)
                    {
                        Zespol.Kierownik =
                            await dbContext.KierownicyZespolow.FirstAsync(kierownik =>
                                kierownik.PESEL == Zespol.Kierownik.PESEL);
                    }
                    else
                    {
                        int index = Zespol.Czlonkowie.FindIndex(os => os.PESEL == StaraOsoba.PESEL);
                        Zespol.Czlonkowie[index] =
                            await dbContext.CzlonkowieZespolow.FirstAsync(czlonek => czlonek.PESEL == StaraOsoba.PESEL);
                    }
                }
            }

            ZespolCluster zespolCluster = await
                dbContext.ZespolClusters.FirstOrDefaultAsync(cluster => cluster.Nazwa == Zespol.Nazwa);

            if (zespolCluster == default(ZespolCluster))
            {
                zespolCluster = new ZespolCluster {Nazwa = Zespol.Nazwa};
                await dbContext.ZespolClusters.AddAsync(zespolCluster);
                await dbContext.SaveChangesAsync();
                zespolCluster = await dbContext.ZespolClusters.FirstOrDefaultAsync(cluster => cluster.Nazwa == Zespol.Nazwa);
            }

            zespolCluster.Zespol = Zespol;
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetZespoly()
        {
            List<ZespolCluster> Zespoly = await dbContext.ZespolClusters
                .Include(c => c.Zespol)
                .Include(c => c.ZespolHistory)
                .Include(c => c.Zespol.Czlonkowie)
                .Include(c => c.Zespol.Kierownik)
                .ToListAsync();
            return Ok(Zespoly);
        }
    }
}
