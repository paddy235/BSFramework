using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Application.Service.DrugManage;
using System.Data;
using BSFramework.Util.WebControl;

namespace BSFramework.Application.Busines.DrugManage
{
    public class DrugBLL
    {
        private IDrugService service = new DrugService();
        private IDrugStockService drugStock = new DrugStockService();
        private IDrugOutService drugOut = new DrugOutService();
        private IGlassService gs = new GlassService();
        private IGlassStockService gss = new GlassStockService();
        private IDrugStockOutService dsoservice = new DrugStockOutService();
        private IDrugInventoryService dis = new DrugInventoryService();
        private IDrugGlassWareService ware = new DrugGlassWareService();
        public DrugStockOutEntity GetStockOutEntity(string id)
        {
            return dsoservice.GetEntity(id);
        }
        public void SaveStockOut(string Id, DrugStockOutEntity entity)
        {
            try
            {
                dsoservice.SaveDrugStockOut(Id, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<DrugStockOutEntity> GetStockOutList(string deptid, string name, string level)
        {
            return dsoservice.GetStockOutList(deptid, name, level);
        }
        public IEnumerable<DrugEntity> GetList(string deptid)
        {
            return service.GetList(deptid);
        }
        public DrugEntity GetEntity(string id)
        {
            return service.GetEntity(id);
        }
        public IEnumerable<DrugEntity> GetPageList(string name, string deptid, int page, int pagesize, out int total)
        {
            return service.GetPageList(name, deptid, page, pagesize, out total);
        }
        public bool DelDrug(string drugId, string deptid)
        {
            DrugStockOutEntity entity = dsoservice.GetEntity(drugId);
            var list = service.GetList(deptid).Where(x => x.DrugName == entity.DrugName && x.DrugLevel == entity.DrugLevel && x.BZId == "deptid").ToList();
            foreach (DrugEntity d in list)
            {
                service.DelDrug(d);
            }
            if (entity != null)
            {
                if (dsoservice.DelDrugStcokOut(entity)) return true;
                else return false;
            }
            else return false;
        }
        public bool DelDrugNew(string drugId, string deptid)
        {

            DrugEntity entity = service.GetEntity(drugId);
            var list = dsoservice.GetStockOutList(deptid, entity.DrugName, entity.DrugLevel).Where(x => x.BZId == deptid).ToList();
            //foreach (DrugStockOutEntity d in list)
            //{
            //    dsoservice.DelDrugStcokOut(d);
            //}
            //if (entity != null)
            //{
            //    if (service.DelDrug(entity)) return true;
            //    else return false;
            //}
            //else return false;

            if (list.Count > 0)
            {
                return false;
            }
            else
            {
                return service.DelDrug(entity);
            }
        }
        public string DelDrugNew1(string drugId, string deptid)
        {
            DrugEntity entity = service.GetEntity(drugId);
            var list = dsoservice.GetStockOutList(deptid, entity.DrugName, entity.DrugLevel).Where(x => x.BZId == deptid).ToList();
            decimal all = 0;
            foreach (DrugStockOutEntity g in list)
            {
                all += g.Total;
            }


            if (all > 0)
            {
                return "1";
            }
            else
            {
                service.DelDrug(entity);
                return "0";
            }
        }
        public void SaveDrug(string Id, DrugEntity entity)
        {
            try
            {
                service.SaveDrug(Id, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 药品出库
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="entity"></param>
        public void SaveDrugOut(string deptid, string Id, DrugStockEntity entity)
        {
            try
            {
                //出库时余量=余量-出库总量
                //总量不变
                DrugEntity drug = service.GetEntity(entity.DrugId);
                drug.Surplus = drug.Surplus - (decimal)entity.DrugNum * int.Parse(drug.Spec);
                drug.DrugNum = Convert.ToInt32(drug.DrugNum - entity.DrugNum);
                drug.OutNum += Convert.ToInt32(entity.DrugNum);
                drug.State = "1";
                drug.OutSurplus += (decimal)entity.DrugNum * Convert.ToDecimal(drug.Spec);
                entity.DrugUnit = drug.Unit;
                entity.DrugName = drug.DrugName;
                entity.DrugLevel = drug.DrugLevelName + drug.DrugLevel;
                //entity.Surplus = drug.Surplus;
                entity.Type = "1";
                entity.StockNum = drug.DrugNum;
                entity.BZId = drug.BZId;
                service.SaveDrug(drug.Id, drug);
                drugStock.SaveDrugStock(Id, entity);

                //若出库信息已存在该类药品，更新余量；否则新增一条
                DrugStockOutEntity dso = new DrugStockOutEntity();
                var dsolist = this.GetStockOutList(deptid, entity.DrugName, drug.DrugLevel).Where(x => x.BZId == drug.BZId);
                if (dsolist.Count() > 0)
                {
                    dso = dsolist.SingleOrDefault();
                    dso.DrugInventoryId = drug.DrugInventoryId;
                    dso.OutTotal += (decimal)entity.DrugNum * int.Parse(drug.Spec);
                    dso.Total += (decimal)entity.DrugNum * int.Parse(drug.Spec);
                    dso.Warn = Convert.ToDecimal(drug.Warn);
                    this.SaveStockOut(dso.Id, dso);
                }
                else
                {
                    dso.DrugInventoryId = drug.DrugInventoryId;
                    dso.Id = Guid.NewGuid().ToString();
                    dso.OutTotal += (decimal)entity.DrugNum * int.Parse(drug.Spec);
                    dso.Total += (decimal)entity.DrugNum * int.Parse(drug.Spec);
                    dso.DrugName = entity.DrugName;
                    dso.DrugUnit = entity.DrugUnit;
                    dso.DrugLevel = drug.DrugLevel;
                    dso.DrugLevelName = drug.DrugLevelName;
                    dso.DrugId = entity.DrugId;
                    dso.CreateDate = DateTime.Now;
                    dso.BZId = drug.BZId;
                    dso.CreateUserId = entity.CreateUserId;
                    dso.CreateUserName = entity.CreateUserName;
                    dso.Warn = Convert.ToDecimal(drug.Warn);
                    this.SaveStockOut(dso.Id, dso);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 药品取用
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="entity"></param>
        public void SaveDrugOutNew(string Id, DrugOutEntity entity)
        {
            try
            {
                //更新出库信息  （该id取的出库信息id）
                DrugStockOutEntity drug = dsoservice.GetEntity(entity.DrugId);
                drug.Total = drug.Total - entity.OutNum;
                dsoservice.SaveDrugStockOut(drug.Id, drug);

                //保存取用记录
                entity.Id = Guid.NewGuid().ToString();
                entity.DrugUnit = drug.DrugUnit;
                entity.DrugName = drug.DrugName;
                entity.DrugLevel = drug.DrugLevelName + "(" + drug.DrugLevel + ")";
                entity.Surplus = drug.Total;
                entity.OutNum = entity.OutNum;
                drugOut.SaveDrugOut(Id, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 药品入库
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="entity"></param>
        public void SaveDrugStock(string Id, DrugStockEntity entity)
        {
            //入库前先要更新药品总量
            try
            {
                //入库时余量=余量+入库总量
                //总量=之前的余量+入库量
                DrugEntity drug = service.GetEntity(entity.DrugId);
                decimal inNum = (decimal)entity.DrugNum * entity.DrugUSL;
                decimal surnum = drug.Surplus.Value;
                drug.Surplus += inNum;
                drug.Total = surnum + inNum;
                drug.DrugNum += entity.DrugNum;
                entity.DrugUnit = drug.Unit;
                entity.DrugName = drug.DrugName;
                entity.DrugLevel = drug.DrugLevelName + drug.DrugLevel;
                entity.Type = "0";
                entity.StockNum = drug.DrugNum;
                entity.BZId = drug.BZId;
                service.SaveDrug(drug.Id, drug);
                drugStock.SaveDrugStock(Id, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<DrugOutEntity> GetOutList(string deptid, DateTime? from, DateTime? to, string DrugName, int page, int pagesize, out int total)
        {
            return drugOut.GetOutList(deptid, from, to, DrugName, page, pagesize, out total);
        }
        /// <summary>
        /// 根据条件获取药品取用记录条数
        /// </summary>
        /// <param name="from">起始时间</param>
        /// <param name="to">结束时间</param>
        /// <param name="DrugName">药品名称</param>
        /// <returns></returns>
        public int GetOutListCount(string deptid, DateTime? from, DateTime? to, string DrugName)
        {
            return drugOut.GetOutListCount(deptid, from, to, DrugName);
        }
        public IEnumerable<DrugStockEntity> GetStockList(string[] deptid, DateTime? from, DateTime? to, string DrugName, int page, int pagesize, out int total)
        {
            return drugStock.GetStockList(deptid, from, to, DrugName, page, pagesize, out total);
        }

        public IEnumerable<DrugInventoryEntity> GetDrugInventoryList()
        {
            return dis.GetList();
        }
        public IEnumerable<DrugGlassWareEntity> GetDrugGlassWareList()
        {
            return dis.GetDrugGlassWareList();
        }
        public IEnumerable<DrugInventoryEntity> GetDrugInventoryPageList(string name, string deptid, int page, int pagesize, out int total)
        {
            return dis.GetPageList(name, deptid, page, pagesize, out total);
        }
        public List<DrugInventoryEntity> GetDrugPageList(string deptcode, Pagination pagination, string queryJson)
        {
            return dis.GetDrugPageList(deptcode, pagination, queryJson);
        }
        public void SaveDrugInventory(string Id, DrugInventoryEntity entity)
        {
            dis.SaveDrugInventory(Id, entity);
        }
        public void SaveDrugGlassWare(string Id, DrugGlassWareEntity entity)
        {
            dis.SaveDrugGlassWare(Id, entity);
        }

        public DrugInventoryEntity GetDrugInventoryEntity(string keyValue)
        {
            return dis.GetEntity(keyValue);
        }
        public DrugGlassWareEntity GetDrugGlassWareEntity(string keyValue)
        {
            return dis.GetDrugGlassWareEntity(keyValue);
        }


        public bool DelDrugInventory(DrugInventoryEntity entity)
        {
            return dis.DelDrugInventory(entity);
        }
        public bool DeleteGlassWare(DrugGlassWareEntity entity)
        {
            return dis.DeleteGlassWare(entity);
        }


        public IEnumerable<GlassEntity> GetGlassList()
        {
            return gs.GetList();
        }
        public IEnumerable<GlassEntity> GetGlassPageList(string name, string deptid, int page, int pagesize, out int total)
        {
            return gs.GetPageList(name, deptid, page, pagesize, out total);
        }
        public IEnumerable<GlassStockEntity> GetGlassStockList(string deptid)
        {
            return gss.GetList(deptid);
        }
        public IEnumerable<GlassStockEntity> GetGlassStockPageList(DateTime? from, DateTime? to, string name, string deptid, string glassid, int page, int pagesize, out int total)
        {
            return gss.GetPageList(from, to, name, deptid, glassid, page, pagesize, out total);
        }

        public void DelGlass(string id)
        {
            GlassEntity obj = gs.GetEntity(id);
            gs.DelGlass(obj);
        }

        public void SaveGlass(string id, GlassEntity obj)
        {
            gs.SaveGlass(id, obj);
        }

        public void SaveGlassStock(string id, GlassStockEntity obj)
        {
            gss.SaveGlassStock(id, obj);
        }

        public GlassEntity GetGlass(string id)
        {
            return gs.GetEntity(id);
        }

        public IEnumerable<DrugGlassWareEntity> GetPageList(Pagination pagination, string queryJson, string type)
        {
            return ware.GetPageList(pagination, queryJson, type);
        }

        IInstrumentService instrument = new InstrumentService();
        public IEnumerable<InstrumentEntity> GetInstrumentList()
        {
            return instrument.GetList();
        }
        public void SaveInstrument(string Id, InstrumentEntity entity)
        {
            instrument.SaveInstrument(Id, entity);
        }
        public InstrumentEntity GetInstrument(string keyValue)
        {
            return instrument.GetEntity(keyValue);
        }
        public bool DelInstrument(InstrumentEntity entity)
        {
            return instrument.DelInstrument(entity);
        }

        IInstrumentBDService instrumentbd = new InstrumentBDService();
        public IEnumerable<InstrumentBDEntity> GetInstrumentBDList()
        {
            return instrumentbd.GetList();
        }
        public void SaveInstrumentBD(string Id, InstrumentBDEntity entity)
        {
            instrumentbd.SaveInstrument(Id, entity);
        }
        public InstrumentBDEntity GetInstrumentBDEntity(string keyValue)
        {
            return instrumentbd.GetEntity(keyValue);
        }
        public bool DelInstrumentBD(InstrumentBDEntity entity)
        {
            return instrumentbd.DelInstrument(entity);
        }
        IInstrumentCheckService instrumentck = new InstrumentCheckService();
        public IEnumerable<InstrumentCheckEntity> GetInstrumentCheckList()
        {
            return instrumentck.GetList();
        }
        public void SaveInstrumentCheck(string Id, InstrumentCheckEntity entity)
        {
            instrumentck.SaveInstrument(Id, entity);
        }
        public InstrumentCheckEntity GetInstrumentCheckEntity(string keyValue)
        {
            return instrumentck.GetEntity(keyValue);
        }
        public bool DelInstrumentCheck(InstrumentCheckEntity entity)
        {
            return instrumentck.DelInstrument(entity);
        }

        public void AddDrugStockOut2(DrugOutEntity data, float used)
        {
            service.AddDrugStockOut2(data, used);
        }

        public void Update(List<DrugEntity> drugs)
        {
            service.Update(drugs);
        }

        public void Import(List<DrugEntity> list)
        {
            service.Import(list);
        }
    }
}
