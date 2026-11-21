using Migracion.Models;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Migracion.Service.Commands
{
    public interface IMaterialPOPCommand
    {
        TResult Insert(mob_rmaterialpopE item);
        TResult Delete(int id);
        TResult Update(mob_rmaterialpopE item);


    }
    public class MaterialPOPCommand : IMaterialPOPCommand
    {
        private readonly ApplicationDbContext _context;
        public MaterialPOPCommand(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public TResult Insert(mob_rmaterialpopE item)
        {
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };
            try
            {
                _context.mob_rmaterialpop.Add(item);
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
                List<mob_rmaterialpopE> model = _context.mob_rmaterialpop.Where(x => x.rmaterialpop_idmaterial == id).ToList();
               
                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rmaterialpop.Remove(x);
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

        public TResult Update(mob_rmaterialpopE item)
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
                List<mob_rmaterialpopE> model = _context.mob_rmaterialpop.Where(x=>x.rmaterialpop_idmaterial == item.rmaterialpop_idmaterial).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rmaterialpop_des = item.rmaterialpop_des;
                        x.rmaterialpop_image = item.rmaterialpop_image;
                        x.rmarcapop_activo = item.rmarcapop_activo;
                        x.rmaterialpop_updatebyuser = item.rmaterialpop_updatebyuser;
                        x.rmaterialpop_date = DateTime.Now;
                        x.rmaterialpop_estadoupd = item.rmaterialpop_estadoupd;
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
