using Migracion.Models;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Service.Commands
{
    public interface IMovimientoPOPCommand
    {
        TResult Insert(mob_rpope item);
        TResult Delete(string cmpy, int id);
        TResult Update(mob_rpope item);
    }
    public class MovimientoPOPCommand : IMovimientoPOPCommand
    {
        private readonly ApplicationDbContext _context;
        public MovimientoPOPCommand(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public TResult Insert(mob_rpope item)
        {
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };
            try
            {
                _context.mob_rpop.Add(item);
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

        public TResult Delete(string cmpy, int id)
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
                List<mob_rpope> model = _context.mob_rpop.Where(x => x.rpop_cmpy == cmpy && x.rpop_idpop == id).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rpop.Remove(x);
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

        public TResult Update(mob_rpope item)
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
                List<mob_rpope> model = _context.mob_rpop.Where(x=>x.rpop_cmpy == item.rpop_cmpy && x.rpop_idpop == item.rpop_idpop).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rpop_observacion = item.rpop_observacion;
                        x.rpop_updatebyuser = item.rpop_updatebyuser;
                        x.rpop_date = DateTime.Now;
                        int result = _context.SaveChanges();
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
