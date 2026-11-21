using Migracion.Models;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Service.Commands
{
    public interface IDetallePOPCommand
    {
        TResult Insert(mob_rdetallepopE item);
        TResult Delete(string cmpy, int id, int idmaterial);
    }
    public class DetallePOPCommand : IDetallePOPCommand
    {
        private readonly ApplicationDbContext _context;
        public DetallePOPCommand(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public TResult Insert(mob_rdetallepopE item)
        {
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };
            try
            {
                _context.mob_rdetallepop.Add(item);
                int result = _context.SaveChanges();
                tresult = new TResult()
                {
                    IsSuccess = true,
                    Message = result.ToString(),
                    ExpireDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                tresult = new TResult()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ExpireDate = DateTime.Now
                };
            }
            return tresult;
        }

        public TResult Delete(string cmpy, int id, int idmaterial)
        {
            int result = 0;
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };
            try
            {
                List<mob_rdetallepopE> model = _context.mob_rdetallepop.Where(x => x.rdetallepop_cmpy == cmpy && x.rdetallepop_idpop == id && x.rdetallepop_idmaterial == idmaterial ).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rdetallepop.Remove(x);
                        result = _context.SaveChanges();
                    });
                }

                tresult = new TResult()
                {
                    IsSuccess = true,
                    Message = result.ToString(),
                    ExpireDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                tresult = new TResult()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ExpireDate = DateTime.Now
                };
            }
            return tresult;
        }
    }
}
