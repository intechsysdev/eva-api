using Migracion.Models;
using Migracion.Models.Dtos;
using Migracion.Persistence.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Migracion.Service.Commands
{
    public interface IExhibicionCommand
    {
        TResult Insert(string cmpy, List<mob_rexhibicionE> lstExhibciones);
        TResult InsertPhoto(string cmpy, List<mob_rexhibicionfotoE> lstExhibcionesFotos);

        //Insert Maestros
        int InsertStatus(string cmpy, List<mob_rstatusexhibicionE> lstExhibcionesStatus);
        int InsertCategory(string cmpy, List<mob_rcategoriaexhibicionE> lstExhibcionesCategory);
        int InsertClass(string cmpy, List<mob_rclasexhibicionE> lstExhibcionesClass);
        int InsertType(string cmpy, List<mob_rtipoexhibicionE> lstExhibcionesType);
        int InsertClassType(string cmpy, List<mob_rclasetipoeexhibicionE> lstExhibcionesClassType);

        //Delete Maestros
        bool DeleteStatus(string id);
        bool DeleteCategory(string id);
        bool DeleteClass(string id);
        bool DeleteType(string id);
        bool DeleteClassType(int idclass, int idType);

        //Update Maestros
        bool UpdatetStatus(mob_rstatusexhibicionE item);
        bool UpdatetCategory(mob_rcategoriaexhibicionE item);
        bool UpdatetClass(mob_rclasexhibicionE item);
        bool UpdatetType(mob_rtipoexhibicionE item);
        bool UpdatetClassType(mob_rclasetipoeexhibicionE item);
        bool UpdateStatuExhibicion(UpdateExhibicionStatus item);
    }
    public class ExhibicionCommand : IExhibicionCommand
    {
        private readonly ApplicationDbContext _context;
        public ExhibicionCommand(
          ApplicationDbContext context)
        {
            _context = context;
        }

        public TResult Insert(string cmpy, List<mob_rexhibicionE> lstExhibciones)
        {
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };
            try
            {
                lstExhibciones.ForEach(x =>
                {
                    List<mob_rexhibicionE> item = _context.mob_rexhibicion.Where(y => y.rexh_key == x.rexh_key).ToList();
                    if (item.Count > 0)
                    {
                        tresult = new TResult()
                        {
                            IsSuccess = true,
                            Message = "Existe",
                            ExpireDate = DateTime.Now
                        };
                    }
                    else
                    {
                        _context.mob_rexhibicion.Add(x);
                        int result = _context.SaveChanges();
                        tresult = new TResult()
                        {
                            IsSuccess = true,
                            Message = result.ToString(),
                            ExpireDate = DateTime.Now
                        };
                    }
                });


            }
            catch (Exception ex)
            {
                tresult = new TResult()
                {
                    IsSuccess = false,
                    Message = JsonSerializer.Serialize(lstExhibciones),
                    ExpireDate = DateTime.Now
                };
            }
            return tresult;
        }

        public TResult InsertPhoto(string cmpy, List<mob_rexhibicionfotoE> lstExhibcionesFotos)
        {
            TResult tresult = new TResult()
            {
                IsSuccess = false,
                Message = "",
                ExpireDate = DateTime.Now
            };
            try
            {
                lstExhibcionesFotos.ForEach(x =>
                {
                    List<mob_rexhibicionfotoE> item = _context.mob_rexhibicionfotos.Where(y => y.rfotoexh_key == x.rfotoexh_key).ToList();
                    if (item.Count > 0)
                    {
                        TResult tresult = new TResult()
                        {
                            IsSuccess = true,
                            Message = "Existe",
                            ExpireDate = DateTime.Now
                        };
                    }
                    else
                    {
                        _context.mob_rexhibicionfotos.Add(x);
                        int result = _context.SaveChanges();
                        TResult tresult = new TResult()
                        {
                            IsSuccess = false,
                            Message = result.ToString(),
                            ExpireDate = DateTime.Now
                        };
                    }
                });
            }
            catch (Exception ex)
            {
                tresult = new TResult()
                {
                    IsSuccess = false,
                    Message = JsonSerializer.Serialize(lstExhibcionesFotos),
                    ExpireDate = DateTime.Now
                };
            }
            return tresult;
        }

        public int InsertStatus(string cmpy, List<mob_rstatusexhibicionE> lstExhibcionesStatus)
        {
            int result = 0;
            try
            {
                _context.mob_rstatusexhibicion.AddRange(lstExhibcionesStatus);
                result = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int InsertCategory(string cmpy, List<mob_rcategoriaexhibicionE> lstExhibcionesCategory)
        {
            int result = 0;
            try
            {
                _context.mob_rcategoriaexhibicion.AddRange(lstExhibcionesCategory);
                result = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int InsertClass(string cmpy, List<mob_rclasexhibicionE> lstExhibcionesClass)
        {
            int result = 0;
            try
            {
                _context.mob_rclasexhibicion.AddRange(lstExhibcionesClass);
                result = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int InsertType(string cmpy, List<mob_rtipoexhibicionE> lstExhibcionesType)
        {
            int result = 0;
            try
            {
                _context.mob_rtipoexhibicion.AddRange(lstExhibcionesType);
                result = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int InsertClassType(string cmpy, List<mob_rclasetipoeexhibicionE> lstExhibcionesClassType)
        {
            int result = 0;
            try
            {
                _context.mob_rclasetipoeexhibicion.AddRange(lstExhibcionesClassType);
                result = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool DeleteStatus(string id)
        {
            bool result = false;
            try
            {
                List<mob_rstatusexhibicionE> model = _context.mob_rstatusexhibicion.Where(x => x.rstatusexh_id == Convert.ToInt32(id)).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rstatusexhibicion.Remove(x);
                        int result = _context.SaveChanges();
                    });

                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool DeleteCategory(string id)
        {
            bool result = false;
            try
            {
                List<mob_rcategoriaexhibicionE> model = _context.mob_rcategoriaexhibicion.Where(x => x.rcateexh_id == Convert.ToInt32(id)).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rcategoriaexhibicion.Remove(x);
                        int result = _context.SaveChanges();
                    });
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool DeleteClass(string id)
        {
            bool result = false;
            try
            {
                List<mob_rclasexhibicionE> model = _context.mob_rclasexhibicion.Where(x => x.rclasexh_id == Convert.ToInt32(id)).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rclasexhibicion.Remove(x);
                        _context.SaveChanges();
                    });

                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool DeleteType(string id)
        {
            bool result = false;
            try
            {
                List<mob_rtipoexhibicionE> model = _context.mob_rtipoexhibicion.Where(x => x.rtipoexh_id == Convert.ToInt32(id)).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rtipoexhibicion.Remove(x);
                        _context.SaveChanges();
                    });

                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool DeleteClassType(int idclass, int idType)
        {
            bool result = false;
            try
            {
                List<mob_rclasetipoeexhibicionE> model = _context.mob_rclasetipoeexhibicion.Where(x => x.rclastipoexh_idtipo == idType && x.rclastipoexh_idclase == idclass).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        _context.mob_rclasetipoeexhibicion.Remove(x);
                        _context.SaveChanges();
                    });
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool UpdatetStatus(mob_rstatusexhibicionE item)
        {
            bool result = false;
            try
            {
                List<mob_rstatusexhibicionE> model = _context.mob_rstatusexhibicion.Where(x => x.rstatusexh_id == item.rstatusexh_id).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rstatusexh_abr = item.rstatusexh_abr;
                        x.rstatusexh_des = item.rstatusexh_des;
                        x.rstatusexh_date = item.rstatusexh_date;
                        x.rstatusexh_time = item.rstatusexh_time;
                        x.rstatusexh_estadoupd = item.rstatusexh_estadoupd;
                        int result = _context.SaveChanges();
                    });

                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool UpdatetCategory(mob_rcategoriaexhibicionE item)
        {
            bool result = false;
            try
            {
                List<mob_rcategoriaexhibicionE> model = _context.mob_rcategoriaexhibicion.Where(x => x.rcateexh_id == item.rcateexh_id).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rcateexh_abr = item.rcateexh_abr;
                        x.rcateexh_des = item.rcateexh_des;
                        x.rcateexh_updatebyuser = item.rcateexh_updatebyuser;
                        x.rcateexh_date = item.rcateexh_date;
                        x.rcateexh_time = item.rcateexh_time;
                        x.rcateexh_estadoupd = item.rcateexh_estadoupd;
                        int result = _context.SaveChanges();
                    });
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool UpdatetClass(mob_rclasexhibicionE item)
        {
            bool result = false;
            try
            {
                List<mob_rclasexhibicionE> model = _context.mob_rclasexhibicion.Where(x => x.rclasexh_id == item.rclasexh_id).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rclasexh_abr = item.rclasexh_abr;
                        x.rclasexh_des = item.rclasexh_des;
                        x.rclasexh_updatebyuser = item.rclasexh_updatebyuser;
                        x.rclasexh_date = item.rclasexh_date;
                        x.rclasexh_time = item.rclasexh_time;
                        x.rclasexh_estadoupd = item.rclasexh_estadoupd;
                        int result = _context.SaveChanges();
                    });

                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool UpdatetClassType(mob_rclasetipoeexhibicionE item)
        {
            bool result = false;
            try
            {
                List<mob_rclasetipoeexhibicionE> model = _context.mob_rclasetipoeexhibicion.Where(x => x.rclastipoexh_idclase == item.rclastipoexh_idclase && x.rclastipoexh_idtipo == item.rclastipoexh_idtipo).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rclastipoexh_updatebyuser = item.rclastipoexh_updatebyuser;
                        x.rclastipoexh_date = item.rclastipoexh_date;
                        x.rclastipoexh_time = item.rclastipoexh_time;
                        x.rclastipoexh_estadoupd = item.rclastipoexh_estadoupd;
                        int result = _context.SaveChanges();
                    });

                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool UpdatetType(mob_rtipoexhibicionE item)
        {
            bool result = false;
            try
            {
                List<mob_rtipoexhibicionE> model = _context.mob_rtipoexhibicion.Where(x => x.rtipoexh_id == item.rtipoexh_id).ToList();

                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rtipoexh_abr = item.rtipoexh_abr;
                        x.rtipoexh_des = item.rtipoexh_des;
                        x.rtipoexh_updatebyuser = item.rtipoexh_updatebyuser;
                        x.rtipoexh_date = item.rtipoexh_date;
                        x.rtipoexh_time = item.rtipoexh_time;
                        x.rtipoexh_estadoupd = item.rtipoexh_estadoupd;
                        int result = _context.SaveChanges();
                    });

                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool UpdateStatuExhibicion(UpdateExhibicionStatus item)
        {
            bool result = false;
            try
            {
                List<mob_rexhibicionE> model = _context.mob_rexhibicion.Where(x => x.rexh_key == item.cod_exhi).ToList();
                if (model != null)
                {
                    model.ForEach(x =>
                    {
                        x.rexh_idstatus = item.rstatusexh_id;
                        x.rexh_note = item.rexh_observacion;
                        x.rexh_updatebyuser = item.rexh_updatebyuser;
                        x.rexh_dateupdate = DateTime.Now;
                        int result = _context.SaveChanges();
                    });
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
