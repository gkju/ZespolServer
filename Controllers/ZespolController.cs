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
            Zespol.Id = Guid.NewGuid().ToString();
            
            if (dbContext.KierownicyZespolow.AsNoTracking()
                .FirstOrDefault(kierownik => kierownik.PESEL == Zespol.Kierownik.PESEL) != default(KierownikZespolu))
            {
                dbContext.KierownicyZespolow.Update(Zespol.Kierownik);
            }
            else
            {
                dbContext.KierownicyZespolow.Add(Zespol.Kierownik);
            }

            await dbContext.SaveChangesAsync();

            foreach (var czlonek in Zespol.Czlonkowie)
            {
                if (dbContext.CzlonkowieZespolow.AsNoTracking().FirstOrDefault(czl => czl.PESEL == czlonek.PESEL) !=
                    default(CzlonekZespolu))
                {
                    dbContext.CzlonkowieZespolow.Update(czlonek);
                }
                else
                {
                    dbContext.CzlonkowieZespolow.Add(czlonek);
                }
                await dbContext.SaveChangesAsync();
            }

            ZespolCluster zespolCluster = await
                dbContext.ZespolClusters.FirstOrDefaultAsync(cluster => cluster.Nazwa == Zespol.Nazwa);

            if (zespolCluster == default(ZespolCluster))
            {
                zespolCluster = new ZespolCluster {Nazwa = Zespol.Nazwa};
                await dbContext.ZespolClusters.AddAsync(zespolCluster);
                await dbContext.SaveChangesAsync();
                dbContext.Entry(zespolCluster).State = EntityState.Detached;
                zespolCluster = await dbContext.ZespolClusters.FirstOrDefaultAsync(cluster => cluster.Nazwa == Zespol.Nazwa);
            }

            await dbContext.SaveChangesAsync();

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
