using Migracion.Models;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Service.Commands
{
    public interface IMarcasPOPCommand
    {
        TResult Insert(mob_rmarcaspopE item);
        TResult Delete(int id);
        TResult Update(mob_rmarcaspopE item);
    }
    public class MarcasPOPCommand : IMarcasPOPCommand
    {
        private readonly ApplicationDbContext _context;
        public MarcasPOPCommand(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public TResult Insert(mob_rmarcaspopE item)
        {
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };
            try
            {
                _context.mob_rmarcaspop.Add(item);
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

        public TResult Delete(int id)
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
                List<mob_rmarcaspopE> model = _context.mob_rmarcaspop.Where(x => x.rmarcapop_idmarca == id).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rmarcaspop.Remove(x);
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

        public TResult Update(mob_rmarcaspopE item)
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
                List<mob_rmarcaspopE> model = _context.mob_rmarcaspop.Where(x => x.rmarcapop_idmarca == item.rmarcapop_idmarca).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rmarcapop_date = DateTime.Now;
                        x.rmarcapop_linea = item.rmarcapop_linea;
                        x.rmarcapop_subl = item.rmarcapop_subl;
                        x.rmarcapop_marca = item.rmarcapop_marca;
                        x.rmarcapop_estadoupd = item.rmarcapop_estadoupd;
                        x.rmarcapop_activo = item.rmarcapop_activo;
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
